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
type Msg = {
  type: 'info' | 'roster' | 'tick' | 'end' | 'reset'
  log?: string
  turn?: number
  dinos?: Dino[]
  win?: boolean
  waves?: number
  total?: number
  turns?: number
  text?: string
}

const WS_URL = 'ws://localhost:8080/ws'

// 속성별 색.
const ATTR_COLOR: Record<string, string> = {
  정오: '#f6b93b', // Noon
  밤: '#5b6ee1', // Night
  새벽: '#2ec4b6', // Dawn
  월식: '#a55eea', // Eclipse
}
const attrColor = (a: string) => ATTR_COLOR[a] ?? '#888'
const hpColor = (r: number) => (r > 0.5 ? '#2ecc71' : r > 0.25 ? '#f1c40f' : '#e74c3c')

type LogLine = { kind: 'sys' | 'you' | 'battle'; text: string }

export default function App() {
  const [dinos, setDinos] = useState<Dino[]>([])
  const [log, setLog] = useState<LogLine[]>([])
  const [turn, setTurn] = useState(0)
  const [status, setStatus] = useState('연결 중…')
  const [verdict, setVerdict] = useState<string | null>(null)
  const [connected, setConnected] = useState(false)
  const [flash, setFlash] = useState<Record<string, number>>({}) // name → 피해 표시 타임스탬프

  const wsRef = useRef<WebSocket | null>(null)
  const prevHp = useRef<Record<string, number>>({})
  const logEndRef = useRef<HTMLDivElement | null>(null)
  const inputRef = useRef<HTMLInputElement | null>(null)

  const addLog = (line: LogLine) => setLog((l) => [...l.slice(-400), line])

  // 다이노 상태 갱신 + 피해 플래시 감지.
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

  // WebSocket 연결.
  useEffect(() => {
    let stop = false
    const connect = () => {
      const ws = new WebSocket(WS_URL)
      wsRef.current = ws
      ws.onopen = () => {
        setConnected(true)
        setStatus('연결됨 — "start" 입력')
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
            if (m.dinos) applyDinos(m.dinos)
            addLog({ kind: 'sys', text: '── 편성 로드 · 전투 시작 ──' })
            break
          case 'tick':
            if (typeof m.turn === 'number') setTurn(m.turn)
            if (m.dinos) applyDinos(m.dinos)
            if (m.log) addLog({ kind: 'battle', text: m.log })
            break
          case 'end': {
            const v = m.win ? '🏆 WIN — 스테이지 클리어' : '💀 LOSE — 편대 전멸'
            setVerdict(`${v} (웨이브 ${m.waves}/${m.total}, ${m.turns}턴)`)
            addLog({ kind: 'sys', text: `=== ${v} ===` })
            break
          }
          case 'reset':
            setDinos([])
            setLog([])
            setVerdict(null)
            setTurn(0)
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

  // 콘솔 자동 스크롤.
  useEffect(() => {
    logEndRef.current?.scrollIntoView({ behavior: 'auto' })
  }, [log])

  const send = (cmd: string) => {
    const ws = wsRef.current
    if (!ws || ws.readyState !== WebSocket.OPEN) {
      addLog({ kind: 'sys', text: '(서버 미연결)' })
      return
    }
    ws.send(cmd)
    addLog({ kind: 'you', text: `> ${cmd}` })
  }

  const onSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    const el = inputRef.current
    if (!el) return
    const cmd = el.value.trim()
    el.value = ''
    if (!cmd) return
    if (cmd === 'cls') {
      setLog([])
      return
    }
    send(cmd)
  }

  const allies = useMemo(() => dinos.filter((d) => d.side === 0), [dinos])
  const enemies = useMemo(() => dinos.filter((d) => d.side === 1), [dinos])

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
          <Battlefield allies={allies} enemies={enemies} flash={flash} />
          {verdict && <div className="verdict">{verdict}</div>}
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
            <input
              ref={inputRef}
              autoFocus
              spellCheck={false}
              placeholder="start · reset · speed 20 · help · cls"
            />
          </form>
          <div className="hint">
            명령: <b>start</b> 전투 시작 · <b>reset</b> 초기화 · <b>speed &lt;ms&gt;</b> 속도 · <b>cls</b> 콘솔 지우기
          </div>
        </section>
      </main>
    </div>
  )
}

// ── SVG 전장: 아군(좌) vs 적(우), 도형 + HP 바 ──────────────────────
function Battlefield({
  allies,
  enemies,
  flash,
}: {
  allies: Dino[]
  enemies: Dino[]
  flash: Record<string, number>
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
    const r = 30
    return (
      <g key={d.name} opacity={d.alive ? 1 : 0.28} transform={`translate(${x}, ${y})`}>
        {/* 몸체: 아군=원, 적=사각(회전) */}
        {d.side === 0 ? (
          <circle r={r} fill={attrColor(d.attr)} stroke={recentlyHit ? '#fff' : '#0d1117'} strokeWidth={recentlyHit ? 4 : 2} />
        ) : (
          <rect x={-r} y={-r} width={r * 2} height={r * 2} rx={8} fill={attrColor(d.attr)} stroke={recentlyHit ? '#fff' : '#0d1117'} strokeWidth={recentlyHit ? 4 : 2} />
        )}
        {d.defending && <circle r={r + 6} fill="none" stroke="#8fd3ff" strokeWidth={2} strokeDasharray="4 4" />}
        {!d.alive && (
          <text textAnchor="middle" dy={7} fontSize={22} fill="#fff">
            ✕
          </text>
        )}
        {/* HP 바 */}
        <rect x={-40} y={r + 8} width={80} height={9} rx={4} fill="#222b36" />
        <rect x={-40} y={r + 8} width={80 * ratio} height={9} rx={4} fill={hpColor(ratio)} />
        {/* 라벨 */}
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
