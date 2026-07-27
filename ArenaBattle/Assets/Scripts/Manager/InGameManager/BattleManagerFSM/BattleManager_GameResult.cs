using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleManager
{
    private void GameResult_Enter()
    {
        bool win = InGameData.Instance.EnemyList.Count == 0;
        string result = win ? "WIN(승리)" : "LOSE(패배)";
        Debug.Log($"[BATTLE] === 전투 종료: {result} ===");
        // 전투 루프 정지(이 상태 유지). InGame Result 페이즈 완전연동은 후속 작업.
        Messenger.Broadcast(Definition.CompleteState, Complete.BattleManager);
    }
    private void GameResult_Update()
    {

    }
    private void GameResult_Exit()
    {

    }
}
