# web-client (React + TypeScript)

PunkerArena 전투를 **한 화면**으로 보는 웹 클라이언트 — SVG 도형 전장 + 콘솔 로그 + CLI 입력.
서버(`../logic-server`)와 모두 **localhost**에서 동작한다.

## 실행 (서버 + 클라, 둘 다 localhost)

```bash
# 1) 전투 WS 서버 (터미널 A)
cd ../logic-server
go run ./cmd/server          # ws://localhost:8080/ws

# 2) 웹 클라이언트 (터미널 B)
cd web-client
npm install                  # 최초 1회
npm run dev                  # http://localhost:5173
```

브라우저에서 http://localhost:5173 접속 → CLI에 `start` 입력.

## 화면
- **좌: 전장(SVG)** — 아군(원)/적(사각) 도형, 속성 색, HP 바, 방어모드 링, 피해 플래시.
- **우: 콘솔** — 전투 로그 실시간 스트리밍(서버가 한 줄씩 전송).
- **하단: CLI** — 명령 입력.

## 명령
| 명령 | 동작 |
|------|------|
| `start` | 데모 스테이지 전투 시작(스트리밍) |
| `reset` | 화면·로그 초기화 |
| `speed <ms>` | 스트리밍 간격(밀리초) |
| `cls` | 콘솔만 지우기(클라 로컬) |
| `help` | 도움말 |

## 프로토콜
- 클라 → 서버: 평문 명령 텍스트.
- 서버 → 클라: JSON `{type: info|roster|tick|end|reset, log, turn, dinos[], win, ...}`.
- 서버 주소는 `src/App.tsx`의 `WS_URL`.
