using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public partial class InGame
{
    private void Load_Enter()
    {
        InitializeCompleteState();

        MessengerAddListner();
        GameDataManager.Instance.GameSceneMessengerAddListner();

        Messenger.Broadcast(Definition.LoadData);
        CompleteState(Complete.InGame);

        //InGameLoad();
        StartCoroutine(LoadBattlePrefab());
    }
    private void Load_Update()
    {

    }

    private void CreateCharacter(GameObject characterObj)
    {

    }

    private IEnumerator LoadBattlePrefab()
    {
        GameObject battleObj = null;
        ResourcePoolManager.Instance.AsyncGetData("BattleManager", objectType.gameobject, null, null, (data) =>
        {
            battleObj = (GameObject) data;
        });
        
        yield return new WaitUntil(() => battleObj != null);
        
        //var battleObj = Addressables.LoadAssetAsync<GameObject>("Assets/Asset/BattleSource/BattleManager.prefab");
        
        //var obj = GameObject.Instantiate(battleObj.Result);
        battleObj.name = "BattleManager";

        //var positionObj = Addressables.LoadAssetAsync<GameObject>("Assets/Asset/BattleSource/CharacterPosition.prefab");
        //yield return new WaitUntil(() => positionObj.IsDone == true);
        //var obj2 = GameObject.Instantiate(positionObj.Result);
        //obj2.gameObject.name = "CharacterPosition";

        //var gameDataBaseObj = Addressables.LoadAssetAsync<GameObject>("Assets/Asset/BattleSource/GameDataBase.prefab");
        //yield return new WaitUntil(() => gameDataBaseObj.IsDone == true);
        //var obj3 = GameObject.Instantiate(gameDataBaseObj.Result);
        //obj3.gameObject.name = "GameDataBase";

        GameObject ownerTexture = null;
        //var ownerTexture = Addressables.LoadAssetAsync<GameObject>("Assets/Asset/BattleSource/OwnerRenderTextureRoot.prefab");
        ResourcePoolManager.Instance.AsyncGetData("OwnerRenderTextureRoot", objectType.gameobject, null, null, (data) =>
        {
            ownerTexture = (GameObject) data;
        });
        yield return new WaitUntil(() => ownerTexture != null);
        ownerTexture.name = "OwnerRenderTextureRoot";

        GameObject enemyTexture = null;
        //var enemyTexture = Addressables.LoadAssetAsync<GameObject>("Assets/Asset/BattleSource/MonsterRenderTextureRoot.prefab");
        ResourcePoolManager.Instance.AsyncGetData("MonsterRenderTextureRoot", objectType.gameobject, null, null, (data) =>
        {
            enemyTexture = (GameObject) data;
        });
        yield return new WaitUntil(() => enemyTexture != null);
        enemyTexture.name = "MonsterRenderTextureRoot";

        //GameObject effectObj = null;
        //var effectObj = Addressables.LoadAssetAsync<GameObject>("Assets/Asset/BattleSource/EffectManager.prefab");
        //ResourcePoolManager.Instance.AsyncGetData("EffectManager", objectType.gameobject, null, null, (data) =>
        //{
        //    effectObj = (GameObject)data;
        //});
        //yield return new WaitUntil(() => effectObj != null);
        //effectObj.name = "EffectManager";

        InGameLoad();
        yield break;
    }
}
