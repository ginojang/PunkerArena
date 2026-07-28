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
	Attribute int     // 1=Noon 2=Night 3=Dawn 4=Eclipse
	Talent    int     // 1 육식 / 2 초식 / 3 잡식
	Role      int     // 역할(딜러/탱커 등)
	InitCoef  float64 // 초기값 계수
	HpCoef    float64 // 체력 계수
	AtkCoef   float64 // 공격 계수
	DefCoef   float64 // 방어 계수
	SpdCoef   float64 // 순발력 계수
	Skill     int     // 대표 스킬 id (SkillTBL — 다음 슬라이스)
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
	Ranks []RankRow // Rank 오름차순
}

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
			Idx: atoi(r[0]), Attribute: atoi(r[7]), Talent: atoi(r[8]), Role: atoi(r[9]),
			InitCoef: atof(r[10]), HpCoef: atof(r[11]), AtkCoef: atof(r[12]),
			DefCoef: atof(r[13]), SpdCoef: atof(r[14]), Skill: atoi(r[15]),
		}
		t.Bases[b.Idx] = b
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
