using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// [HARDCODE TEST] 오프라인이라 서버 편성(serverSquadDic)이 비어 전투에 공룡이 안 나온다.
/// 전투 배경 씬(Desert 등) 로드 시, 기본 공룡을 하드코딩으로 세워서 렌더를 확인한다.
/// 실제 편성/전투 참여가 아니라 "공룡을 세워 보는" 용도.
/// </summary>
public class TestDinoSpawner : MonoBehaviour
{
    // 세울 공룡 수 / 간격
    const int   Count   = 3;
    const float Spacing = 1.6f;

    static TestDinoSpawner _instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Bootstrap()
    {
        if (_instance != null) return;
        var go = new GameObject("[TestDinoSpawner]");
        DontDestroyOnLoad(go);
        _instance = go.AddComponent<TestDinoSpawner>();
        SceneManager.sceneLoaded += _instance.OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 전투 배경 씬(Desert/Jungle/Beach 등)만 대상. main/Title/Menu 등은 제외.
        string n = scene.name;
        if (n == "Desert" || n == "Jungle" || n == "Beach" || n == "Ice" ||
            n == "Snake" || n == "Remains" || n == "Avatar")
        {
            StartCoroutine(SpawnDelayed());
        }
    }

    IEnumerator SpawnDelayed()
    {
        // CSV 테이블 / ResourcePool / Addressables 준비 대기
        yield return new WaitForSeconds(1.0f);

        // 전투 카메라가 바라보는 중심
        var look = GameObject.Find("LookBasic");
        Vector3 basePos = look != null ? look.transform.position : Vector3.zero;

        string body = Definition.baseCharBodyName; // "ca_orange_01"

        for (int i = 0; i < Count; i++)
        {
            var anchor = new GameObject($"[TestDinoAnchor{i}]").transform;
            anchor.position = basePos + new Vector3((i - (Count - 1) * 0.5f) * Spacing, 0f, 0f);

            CostumeManager.Instance.LoadBaseCharacter(
                body, CharacterTalent.Carnivore, CharacterClass.orange,
                null, anchor, /*bTool*/ true);

            yield return null;
        }

        Debug.Log($"[TestDinoSpawner] spawned {Count} dinos at {basePos}");
    }
}
