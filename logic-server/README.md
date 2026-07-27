# logic-server (Go)

PunkerArena 전투 로직 서버 — **Go로 신설**. 언어 결정: C# 재사용을 버리고 Go 채택(단일바이너리·헤드리스 반복검증 우선). 관련 결정 배경: 루트 `Server_Battle_Core_Reference.md`, `Battle_Rules_Analysis.md`.

## 목표
- 서버권위 턴제 웨이브 전투를 **순수 Go로** 구현 (규칙은 `FruttiDino_RPG/Server/.../ServerWaveCore` 이식).
- **헤드리스로 `go run`** 해서 룰을 반복검증 (Unity 없이).
- 추후 WebSocket 계층(WebGL 호환) 추가 → Unity 클라와 연결. 매칭/소셜 필요 시 Nakama를 앞단에.

## 구조 (현재: 첫 마일스톤)
```
logic-server/
  battle/          전투 코어 (순수 Go, 테스트/헤드리스 가능)
    stat.go        Dino 스탯셋(DataDinoCore 이식) + 상수(TableData.Stat)
    damage.go      진짜 데미지 공식: ATK*(50/(DEF+50)) ±1/16, 크리, 회피, 속성 RPS(+30%)
    sim.go         단일 전투 루프: 속도(aux)순 턴 → 자동공격 → 사망 → 전멸 승패
    wave.go        스테이지=편대(HP 이월) vs 순차 적 웨이브 체이닝
  cmd/sim/main.go  헤드리스 스테이지(3웨이브) 러너
```

## 실행
```
cd logic-server
go run ./cmd/sim      # 3v3 오토배틀 로그 + 승패 출력
```

## 이식 현황 (ServerWaveCore 대비)
- [x] 기본 데미지 곡선 `ATK*(50/(DEF+50))` ±분산, `ATK<DEF→1`
- [x] 크리 데미지, 크리/회피 판정(aux·luck 기반)
- [x] 속성 가위바위보(NOON<NIGHT<DAWN 순환, ECLIPSE 중립, 승리 +30%)
- [x] 속도(aux)순 턴 순서 + 전멸 승패
- [x] 방어(Defence)모드 완화 84~91% + 관통(경감 무시). 원본 버그 2개(경감 /100 누락, 관통 ×0.05/0.1 감소) 의도대로 수정
- [x] 웨이브 체이닝: 편대(side 0) HP 이월·사망 유지 vs 순차 신규 적 웨이브. 전 웨이브 클리어=승, 편대 전멸=패
- [ ] 스킬·트리거 / 레벨 성장 / CC·저항
- [ ] WebSocket 서버 + 클라 프로토콜

상수/공식 출처는 `Server_Battle_Core_Reference.md` 참고.
