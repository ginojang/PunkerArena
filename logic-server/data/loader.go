// Package data: Table/csv(원본 밸런스 테이블)의 vendored 사본을 읽어 전투 엔티티를 생성한다.
// 스탯/성장 공식은 원본 ServerWaveCore(TestMortalCreator.CreateDino/ProcessLevelUp) 이식.
package data

import (
	"encoding/csv"
	"fmt"
	"os"
	"path/filepath"
	"strconv"
	"strings"
)

// BaseRow: DinoBaseTBL 한 행(전투에 쓰는 컬럼만).
type BaseRow struct {
	Idx       int
	Class     int     // 파츠 매칭용 클래스
	Attribute int     // 1=Noon 2=Night 3=Dawn 4=Eclipse
	Talent    int     // 1 육식 / 2 초식 / 3 잡식
	Role      int     // 역할(딜러/탱커 등)
	InitCoef  float64 // 초기값 계수
	HpCoef    float64 // 체력 계수
	AtkCoef   float64 // 공격 계수
	DefCoef   float64 // 방어 계수
	SpdCoef   float64 // 순발력 계수
	Skill     int     // 몸통 대표 스킬 id
}

// PartRow: DinoPartsTBL 한 행. 몸통 계수에 더해지는 계수 + 파츠 전용 부스탯 + 스킬.
type PartRow struct {
	Idx, Class, Type                      int
	InitCoef, HpCoef, AtkCoef, DefCoef, SpdCoef float64
	HitRate, AvoidRate, CritRate, CritDmg float64
	ResRate, PenRate, Luck                float64
	Skill                                 int
}

// subSum: 이 파츠의 부스탯 총합(기본 파츠셋 선택 기준).
func (p PartRow) subSum() float64 {
	return p.HitRate + p.AvoidRate + p.CritRate + p.CritDmg + p.ResRate + p.PenRate + p.Luck
}

// RankRow: DinoStatGrowthRankTBL 한 행. 성장 랭크별 레벨업 상수 범위.
type RankRow struct {
	Rank    int
	MarkRef float64 // GrowthBase 경계(초과 시 이 랭크). rank6=0(폴백)
	Min     float64 // constValue 하한
	Max     float64 // constValue 상한
}

// Tables: 로드된 밸런스 테이블 모음.
type Tables struct {
	Bases map[int]BaseRow
	Parts []PartRow // DinoPartsTBL 전체
	Ranks []RankRow // Rank 오름차순

	// 스킬 테이블 (skill.go)
	Skills      map[int]SkillDef
	SkillLevels map[int]map[int]SkillLevel // idx → lv → 수치
	Buffs       map[int]BuffDef
	Ccs         map[int]CcDef

	Strings map[int]string // StringTBL: id → 한국어 이름
}

// str: StringTBL 이름 조회(없으면 빈 문자열).
func (t *Tables) str(id int) string { return t.Strings[id] }

// readCSV: BOM 제거 + 빈 줄 스킵. 첫 줄은 헤더로 반환 rows[0].
func readCSV(path string) ([][]string, error) {
	f, err := os.Open(path)
	if err != nil {
		return nil, err
	}
	defer f.Close()
	r := csv.NewReader(f)
	r.FieldsPerRecord = -1 // 후행 콤마 등 가변 컬럼 허용
	rows, err := r.ReadAll()
	if err != nil {
		return nil, err
	}
	if len(rows) > 0 && len(rows[0]) > 0 {
		rows[0][0] = strings.TrimPrefix(rows[0][0], "\ufeff") // UTF-8 BOM
	}
	return rows, nil
}

func atoi(s string) int   { v, _ := strconv.Atoi(strings.TrimSpace(s)); return v }
func atof(s string) float64 { v, _ := strconv.ParseFloat(strings.TrimSpace(s), 64); return v }

// LoadTables: dir(=.../data/csv) 아래의 필요한 CSV를 로드.
func LoadTables(dir string) (*Tables, error) {
	t := &Tables{Bases: map[int]BaseRow{}}

	// DinoBaseTBL: idx,class_name,category,max_lv,minting_lv,class,grade,attribute,talent,role,
	//              init_coef,hp_coef,atk_coef,def_coef,spd_coef,skill,...
	baseRows, err := readCSV(filepath.Join(dir, "DinoBaseTBL.csv"))
	if err != nil {
		return nil, fmt.Errorf("DinoBaseTBL: %w", err)
	}
	for _, r := range baseRows[1:] {
		if len(r) < 16 || strings.TrimSpace(r[0]) == "" {
			continue
		}
		b := BaseRow{
			Idx: atoi(r[0]), Class: atoi(r[5]), Attribute: atoi(r[7]), Talent: atoi(r[8]), Role: atoi(r[9]),
			InitCoef: atof(r[10]), HpCoef: atof(r[11]), AtkCoef: atof(r[12]),
			DefCoef: atof(r[13]), SpdCoef: atof(r[14]), Skill: atoi(r[15]),
		}
		t.Bases[b.Idx] = b
	}

	// DinoPartsTBL: idx,file,name,desc,parts_class,grade,parts_type,init,hp,atk,def,spd,
	//               hit,avd,cri,cri_dmg,res,def_pen,luk,charm,skill,res_icon
	partRows, err := readCSV(filepath.Join(dir, "DinoPartsTBL.csv"))
	if err != nil {
		return nil, fmt.Errorf("DinoPartsTBL: %w", err)
	}
	for _, r := range partRows[1:] {
		if len(r) < 21 || strings.TrimSpace(r[0]) == "" {
			continue
		}
		t.Parts = append(t.Parts, PartRow{
			Idx: atoi(r[0]), Class: atoi(r[4]), Type: atoi(r[6]),
			InitCoef: atof(r[7]), HpCoef: atof(r[8]), AtkCoef: atof(r[9]), DefCoef: atof(r[10]), SpdCoef: atof(r[11]),
			HitRate: atof(r[12]), AvoidRate: atof(r[13]), CritRate: atof(r[14]), CritDmg: atof(r[15]),
			ResRate: atof(r[16]), PenRate: atof(r[17]), Luck: atof(r[18]), Skill: atoi(r[20]),
		})
	}

	// DinoStatGrowthRankTBL: rank,mark_ref_value,min_const_value,max_const_value
	rankRows, err := readCSV(filepath.Join(dir, "DinoStatGrowthRankTBL.csv"))
	if err != nil {
		return nil, fmt.Errorf("DinoStatGrowthRankTBL: %w", err)
	}
	for _, r := range rankRows[1:] {
		if len(r) < 4 || strings.TrimSpace(r[0]) == "" {
			continue
		}
		t.Ranks = append(t.Ranks, RankRow{
			Rank: atoi(r[0]), MarkRef: atof(r[1]), Min: atof(r[2]), Max: atof(r[3]),
		})
	}
	if len(t.Ranks) == 0 {
		return nil, fmt.Errorf("성장 랭크 테이블이 비었음")
	}

	// StringTBL: id,kr,en,... (스킬/CC 실제 이름) — 스킬 로드 전에 준비.
	t.Strings = map[int]string{}
	strRows, err := readCSV(filepath.Join(dir, "StringTBL.csv"))
	if err != nil {
		return nil, fmt.Errorf("StringTBL: %w", err)
	}
	for _, r := range strRows[1:] {
		if len(r) < 2 || strings.TrimSpace(r[0]) == "" {
			continue
		}
		name := strings.TrimSpace(r[1]) // kr
		if name == "" && len(r) >= 3 {
			name = strings.TrimSpace(r[2]) // en 폴백
		}
		if name != "" {
			t.Strings[atoi(r[0])] = name
		}
	}

	if err := t.loadSkills(dir); err != nil {
		return nil, err
	}
	return t, nil
}

// rankOf: GrowthBase로 성장 랭크 선택. [이식] GetRankValue — 경계 초과하는 첫 랭크.
func (t *Tables) rankOf(growthBase float64) RankRow {
	for _, rr := range t.Ranks {
		if growthBase > rr.MarkRef {
			return rr
		}
	}
	return t.Ranks[len(t.Ranks)-1] // 폴백(최저 랭크)
}
