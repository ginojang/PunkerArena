// WebSocket 전투 서버: go run ./cmd/server (localhost:8080)
// 웹 클라이언트(React)가 ws://localhost:8080/ws 로 접속. 클라는 CLI 명령(텍스트)을 보내고,
// 서버는 전투를 한 줄씩 스트리밍(JSON)한다. 그래픽/콘솔은 클라가 렌더.
package main

import (
	"fmt"
	"log"
	"math/rand"
	"net/http"
	"strings"
	"sync"
	"time"

	"github.com/gorilla/websocket"

	"punker/logic-server/battle"
	"punker/logic-server/data"
	"punker/logic-server/scenario"
)

const (
	addr   = "localhost:8080"
	csvDir = "data/csv"
)

var upgrader = websocket.Upgrader{
	CheckOrigin: func(r *http.Request) bool { return true }, // 로컬 개발: 오리진 허용
}

func attrName(a battle.Attribute) string {
	switch a {
	case battle.Noon:
		return "정오"
	case battle.Night:
		return "밤"
	case battle.Dawn:
		return "새벽"
	case battle.Eclipse:
		return "월식"
	}
	return "-"
}

// dinoJSON: 클라 렌더용 다이노 스냅샷.
type dinoJSON struct {
	Name  string  `json:"name"`
	Side  int     `json:"side"`
	HP    float64 `json:"hp"`
	MaxHP float64 `json:"maxHp"`
	Alive bool    `json:"alive"`
	Attr  string  `json:"attr"`
	Atk   float64 `json:"atk"`
	Def   float64 `json:"def"`
	Spd   float64 `json:"spd"`
	Lv    int     `json:"lv"`
	Defn  bool    `json:"defending"`
}

func snapshot(dinos []*battle.Dino) []dinoJSON {
	out := make([]dinoJSON, 0, len(dinos))
	for _, d := range dinos {
		out = append(out, dinoJSON{
			Name: d.Name, Side: d.Side, HP: d.HP, MaxHP: d.MaxHP, Alive: d.Alive(),
			Attr: attrName(d.Attribute), Atk: d.Attack, Def: d.Defence, Spd: d.Aux,
			Lv: d.Level, Defn: d.Defending,
		})
	}
	return out
}

type msg struct {
	Type  string     `json:"type"`
	Log   string     `json:"log,omitempty"`
	Turn  int        `json:"turn,omitempty"`
	Dinos []dinoJSON `json:"dinos,omitempty"`
	Win   bool       `json:"win,omitempty"`
	Waves int        `json:"waves,omitempty"`
	Total int        `json:"total,omitempty"`
	Turns int        `json:"turns,omitempty"`
	Text  string     `json:"text,omitempty"`
}

func main() {
	tables, err := data.LoadTables(csvDir)
	if err != nil {
		log.Fatalf("테이블 로드 실패: %v (logic-server/ 에서 실행하세요)", err)
	}

	http.HandleFunc("/ws", func(w http.ResponseWriter, r *http.Request) {
		conn, err := upgrader.Upgrade(w, r, nil)
		if err != nil {
			return
		}
		defer conn.Close()
		serve(conn, tables)
	})
	http.HandleFunc("/", func(w http.ResponseWriter, r *http.Request) {
		fmt.Fprintln(w, "PunkerArena logic-server — connect a client to ws://"+addr+"/ws")
	})

	log.Printf("전투 WS 서버 시작: ws://%s/ws", addr)
	log.Fatal(http.ListenAndServe(addr, nil))
}

// serve: 한 연결의 명령 루프. 명령(텍스트) 읽고 → 전투 스트리밍(JSON). 단일 고루틴(동시쓰기 없음).
func serve(conn *websocket.Conn, tables *data.Tables) {
	var mu sync.Mutex
	send := func(m msg) error {
		mu.Lock()
		defer mu.Unlock()
		return conn.WriteJSON(m)
	}
	delay := 35 * time.Millisecond

	send(msg{Type: "info", Text: "연결됨. 명령: start / reset / speed <ms> / help"})

	for {
		_, raw, err := conn.ReadMessage()
		if err != nil {
			return
		}
		fields := strings.Fields(strings.TrimSpace(string(raw)))
		if len(fields) == 0 {
			continue
		}
		switch fields[0] {
		case "start", "run", "s":
			runStage(tables, delay, send)
		case "reset", "clear", "r":
			send(msg{Type: "reset"})
		case "speed":
			if len(fields) > 1 {
				if v := atoiSafe(fields[1]); v > 0 {
					delay = time.Duration(v) * time.Millisecond
					send(msg{Type: "info", Text: fmt.Sprintf("스트리밍 간격 = %dms", v)})
				}
			}
		case "help", "h", "?":
			send(msg{Type: "info", Text: "start=전투시작, reset=초기화, speed <ms>=속도, help=도움말"})
		default:
			send(msg{Type: "info", Text: "알 수 없는 명령: " + fields[0] + " (help 참고)"})
		}
	}
}

// runStage: 데모 스테이지를 새로 구성하고 로그 1줄씩 스트리밍.
func runStage(tables *data.Tables, delay time.Duration, send func(msg) error) {
	rand.Seed(1) // 전투 판정 재현
	_, _, stage := scenario.Demo(tables)

	send(msg{Type: "roster", Dinos: snapshot(append(append([]*battle.Dino{}, stage.Squad...), stage.Waves[0]...))})

	stage.OnLog = func(b *battle.Battle, line string) {
		send(msg{Type: "tick", Turn: b.Turn, Log: line, Dinos: snapshot(b.Dinos)})
		time.Sleep(delay)
	}
	res := stage.Run(500)
	send(msg{Type: "end", Win: res.Win, Waves: res.WavesClear, Total: res.TotalWaves, Turns: res.Turns})
}

func atoiSafe(s string) int {
	n := 0
	for _, c := range s {
		if c < '0' || c > '9' {
			return 0
		}
		n = n*10 + int(c-'0')
	}
	return n
}
