import { useEffect, useMemo, useRef, useState } from 'react'

// ── 서버(ws://localhost:8080/ws) 메시지 타입 ─────────────────────────
type Dino = {
  name: string
  side: number
  hp: number
  maxHp: number
  alive: boolean
  attr: string
  atk: number
  def: number
  spd: number
  lv: number
  defending: boolean
}
type Action = { idx: number; name: string; needsTarget: boolean; targetSide: string }
type Msg = {
  type: 'info' | 'roster' | 'tick' | 'end' | 'reset' | 'turn'
  log?: string
  turn?: number
  dinos?: Dino[]
  win?: boolean
  waves?: number
  total?: number
  turns?: number
  text?: string
  actor?: string
  actions?: Action[]
}

const WS_URL = 'ws://localhost:8080/ws'

const ATTR_COLOR: Record<string, string> = {
  정오: '#f6b93b',
  밤: '#5b6ee1',
  새벽: '#2ec4b6',
  월식: '#a55eea',
}
const attrColor = (a: string) => ATTR_COLOR[a] ?? '#888'
const hpColor = (r: number) => (r > 0.5 ? '#2ecc71' : r > 0.25 ? '#f1c40f' : '#e74c3c')

type LogLine = { kind: 'sys' | 'you' | 'battle'; text: string }
type Turn = { actor: string; actions: Action[] }
type Selecting = { idx: number; side: string } | null

export default function App() {
  const [dinos, setDinos] = useState<Dino[]>([])
  const [log, setLog] = useState<LogLine[]>([])
  const [turn, setTurn] = useState(0)
  const [status, setStatus] = useState('연결 중…')
  const [verdict, setVerdict] = useState<string | null>(null)
  const [connected, setConnected] = useState(false)
  const [flash, setFlash] = useState<Record<string, number>>({})
  const [pending, setPending] = useState<Turn | null>(null)
  const [selecting, setSelecting] = useState<Selecting>(null)

  const wsRef = useRef<WebSocket | null>(null)
  const prevHp = useRef<Record<string, number>>({})
  const logEndRef = useRef<HTMLDivElement | null>(null)
  const inputRef = useRef<HTMLInputElement | null>(null)

  const addLog = (line: LogLine) => setLog((l) => [...l.slice(-400), line])

  const applyDinos = (next: Dino[]) => {
    const hit: Record<string, number> = {}
    for (const d of next) {
      const prev = prevHp.current[d.name]
      if (prev !== undefined && d.hp < prev - 0.01) hit[d.name] = Date.now()
      prevHp.current[d.name] = d.hp
    }
    if (Object.keys(hit).length) setFlash((f) => ({ ...f, ...hit }))
    setDinos(next)
  }

  useEffect(() => {
    let stop = false
    const connect = () => {
      const ws = new WebSocket(WS_URL)
      wsRef.current = ws
      ws.onopen = () => {
        setConnected(true)
        setStatus('연결됨 — "start"로 조작, "watch"로 관전')
      }
      ws.onclose = () => {
        setConnected(false)
        setStatus('연결 끊김 — 서버 확인 후 재시도…')
        if (!stop) setTimeout(connect, 1500)
      }
      ws.onerror = () => ws.close()
      ws.onmessage = (e) => {
        const m: Msg = JSON.parse(e.data)
        switch (m.type) {
          case 'info':
            addLog({ kind: 'sys', text: m.text ?? '' })
            break
          case 'roster':
            prevHp.current = {}
            setVerdict(null)
            setTurn(0)
            setPending(null)
            setSelecting(null)
            if (m.dinos) applyDinos(m.dinos)
            addLog({ kind: 'sys', text: '── 편성 로드 · 전투 시작 ──' })
            break
          case 'tick':
            if (typeof m.turn === 'number') setTurn(m.turn)
            if (m.dinos) applyDinos(m.dinos)
            if (m.log) addLog({ kind: 'battle', text: m.log })
            break
          case 'turn':
            if (typeof m.turn === 'number') setTurn(m.turn)
            if (m.dinos) applyDinos(m.dinos)
            setPending({ actor: m.actor ?? '', actions: m.actions ?? [] })
            setSelecting(null)
            addLog({ kind: 'sys', text: `▶ ${m.actor} 의 턴 — 행동 선택` })
            break
          case 'end': {
            const v = m.win ? '🏆 WIN — 스테이지 클리어' : '💀 LOSE — 편대 전멸'
            setVerdict(`${v} (웨이브 ${m.waves}/${m.total}, ${m.turns}턴)`)
            setPending(null)
            setSelecting(null)
            addLog({ kind: 'sys', text: `=== ${v} ===` })
            break
          }
          case 'reset':
            setDinos([])
            setLog([])
            setVerdict(null)
            setTurn(0)
            setPending(null)
            setSelecting(null)
            prevHp.current = {}
            break
        }
      }
    }
    connect()
    return () => {
      stop = true
      wsRef.current?.close()
    }
  }, [])

  useEffect(() => {
    logEndRef.current?.scrollIntoView({ behavior: 'auto' })
  }, [log])

  const send = (cmd: string, echo = true) => {
    const ws = wsRef.current
    if (!ws || ws.readyState !== WebSocket.OPEN) {
      addLog({ kind: 'sys', text: '(서버 미연결)' })
      return
    }
    ws.send(cmd)
    if (echo) addLog({ kind: 'you', text: `> ${cmd}` })
  }

  const onSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    const el = inputRef.current
    if (!el) return
    const cmd = el.value.trim()
    el.value = ''
    if (!cmd) return
    if (cmd === 'cls') return setLog([])
    send(cmd)
  }

  // 액션 버튼 클릭.
  const onAction = (a: Action) => {
    if (!pending) return
    if (a.needsTarget) {
      setSelecting({ idx: a.idx, side: a.targetSide })
      addLog({ kind: 'sys', text: `· ${a.name}: ${a.targetSide === 'ally' ? '아군' : '적'} 대상을 클릭` })
    } else {
      send(`choose ${a.idx}`)
      setPending(null)
    }
  }

  // SVG 다이노 클릭(타겟 선택 중일 때).
  const onDinoClick = (d: Dino) => {
    if (!selecting || !d.alive) return
    const wantSide = selecting.side === 'ally' ? 0 : 1
    if (d.side !== wantSide) return
    send(`choose ${selecting.idx} ${d.name}`)
    setPending(null)
    setSelecting(null)
  }

  const allies = useMemo(() => dinos.filter((d) => d.side === 0), [dinos])
  const enemies = useMemo(() => dinos.filter((d) => d.side === 1), [dinos])
  const selectableSide = selecting ? (selecting.side === 'ally' ? 0 : 1) : -1

  return (
    <div className="app">
      <header className="topbar">
        <span className="brand">PunkerArena</span>
        <span className="tag">Battle Console</span>
        <span className={`dot ${connected ? 'on' : 'off'}`} />
        <span className="status">{status}</span>
        <span className="turn">Turn {turn}</span>
      </header>

      <main className="stage">
        <section className="field">
          <Battlefield
            allies={allies}
            enemies={enemies}
            flash={flash}
            actor={pending?.actor ?? null}
            selectableSide={selectableSide}
            onDinoClick={onDinoClick}
          />
          {verdict && <div className="verdict">{verdict}</div>}

          {pending && (
            <div className="actionbar">
              <span className="actor">
                ▶ {pending.actor}
                {selecting ? ' — 대상 클릭' : ' — 행동 선택'}
              </span>
              <div className="btns">
                {pending.actions.map((a) => (
                  <button
                    key={a.idx}
                    className={selecting?.idx === a.idx ? 'act sel' : 'act'}
                    onClick={() => onAction(a)}
                  >
                    {a.name}
                    {a.needsTarget && <span className="tgt"> ⌖</span>}
                  </button>
                ))}
                <button className="act auto" onClick={() => { send('choose auto'); setPending(null); setSelecting(null) }}>
                  자동 ⚙
                </button>
              </div>
            </div>
          )}
        </section>

        <section className="console">
          <div className="log">
            {log.map((l, i) => (
              <div key={i} className={`line ${l.kind}`}>
                {l.text}
              </div>
            ))}
            <div ref={logEndRef} />
          </div>
          <form className="cli" onSubmit={onSubmit}>
            <span className="prompt">λ</span>
            <input ref={inputRef} autoFocus spellCheck={false} placeholder="start · watch · reset · speed 20 · help · cls" />
          </form>
          <div className="hint">
            <b>start</b> 조작 · <b>watch</b> 관전(AI) · <b>reset</b> 초기화 · <b>speed &lt;ms&gt;</b> · <b>cls</b> 콘솔지우기 · 턴엔 버튼/타겟 클릭
          </div>
        </section>
      </main>
    </div>
  )
}

// ── SVG 전장 ─────────────────────────────────────────────────────
function Battlefield({
  allies,
  enemies,
  flash,
  actor,
  selectableSide,
  onDinoClick,
}: {
  allies: Dino[]
  enemies: Dino[]
  flash: Record<string, number>
  actor: string | null
  selectableSide: number
  onDinoClick: (d: Dino) => void
}) {
  const W = 720
  const rows = Math.max(allies.length, enemies.length, 1)
  const rowH = 96
  const H = Math.max(rows * rowH + 40, 240)
  const now = Date.now()

  const node = (d: Dino, x: number, i: number) => {
    const y = 50 + i * rowH
    const ratio = Math.max(0, d.hp / d.maxHp)
    const recentlyHit = now - (flash[d.name] ?? 0) < 350
    const isActor = d.name === actor
    const selectable = d.side === selectableSide && d.alive
    const r = 30
    return (
      <g
        key={d.name}
        opacity={d.alive ? 1 : 0.28}
        transform={`translate(${x}, ${y})`}
        style={{ cursor: selectable ? 'pointer' : 'default' }}
        onClick={() => onDinoClick(d)}
      >
        {isActor && <circle r={r + 11} fill="none" stroke="#ffd479" strokeWidth={3} opacity={0.9} />}
        {selectable && <circle r={r + 8} fill="none" stroke="#ff6b6b" strokeWidth={3} strokeDasharray="5 4" />}
        {d.side === 0 ? (
          <circle r={r} fill={attrColor(d.attr)} stroke={recentlyHit ? '#fff' : '#0d1117'} strokeWidth={recentlyHit ? 4 : 2} />
        ) : (
          <rect x={-r} y={-r} width={r * 2} height={r * 2} rx={8} fill={attrColor(d.attr)} stroke={recentlyHit ? '#fff' : '#0d1117'} strokeWidth={recentlyHit ? 4 : 2} />
        )}
        {d.defending && <circle r={r + 5} fill="none" stroke="#8fd3ff" strokeWidth={2} strokeDasharray="4 4" />}
        {!d.alive && (
          <text textAnchor="middle" dy={7} fontSize={22} fill="#fff">
            ✕
          </text>
        )}
        <rect x={-40} y={r + 8} width={80} height={9} rx={4} fill="#222b36" />
        <rect x={-40} y={r + 8} width={80 * ratio} height={9} rx={4} fill={hpColor(ratio)} />
        <text textAnchor="middle" y={-r - 12} fontSize={13} fill="#e6edf3" fontWeight={600}>
          {d.name}
        </text>
        <text textAnchor="middle" y={r + 32} fontSize={11} fill="#9aa7b4">
          Lv{d.lv} · {Math.round(d.hp)}/{Math.round(d.maxHp)}
        </text>
      </g>
    )
  }

  return (
    <svg className="svg" viewBox={`0 0 ${W} ${H}`} preserveAspectRatio="xMidYMin meet">
      <line x1={W / 2} y1={20} x2={W / 2} y2={H - 20} stroke="#1e2733" strokeWidth={2} strokeDasharray="6 8" />
      <text x={W * 0.25} y={26} textAnchor="middle" fontSize={12} fill="#6ee7a8" fontWeight={700}>
        편대 (side 0)
      </text>
      <text x={W * 0.75} y={26} textAnchor="middle" fontSize={12} fill="#ff8b8b" fontWeight={700}>
        적 웨이브 (side 1)
      </text>
      {allies.map((d, i) => node(d, W * 0.25, i))}
      {enemies.map((d, i) => node(d, W * 0.75, i))}
    </svg>
  )
}
