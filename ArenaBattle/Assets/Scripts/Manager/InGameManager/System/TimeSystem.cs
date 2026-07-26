using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Devil.Common;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Generated.CsvData;

public class TimeSystem : MonoBehaviour
{
    public static TimeSystem Instance = null;
    //struct TimeData
    //{
    //    public CharacterAttribute attribute;
    //    public float timeSet;
    //    public int buffId;
    //}
        
    //private bool stop = true;
    //private float currentTime = 0;

    //private Queue qAttribute;

    //private TimeColor timeColor = null;
    //private GameObject character_Light = null;

    //[SerializeField]private GameObject[] attributeImage = new GameObject[3];
    
    //[SerializeField] private GameObject tempTextRoot = null;
    //[SerializeField] private Text tempText = null;

    //private void Awake()
    //{
    //    if (Instance == null)
    //        Instance = this;
        
    //    MessageAddListner();
    //}

    //private void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if (stop == false)
    //    {
    //        CheckTime();
    //    }
    //}

    //public void Initialize(int stageIdx)
    //{
    //    qAttribute = new Queue();

    //    var data = CSVDataManager.GetStageTimeData(stageIdx);
    //    var timeData = CSVDataManager.GetTable<TimeChangeTable>().GetData(data.timeIndex);
    //    List<string> loadIcon = new List<string>();
    //    List<int> effectSource = new List<int>();
        
    //    CharacterAttribute start = data.startTime;
       
    //    for (int i = 0; i < 3; i++)
    //    {
    //        if (start >= CharacterAttribute.Attribute_Eclipse)
    //            start = CharacterAttribute.Attribute_Day;

    //        TimeData time = new TimeData();
    //        switch (start)
    //        {
    //            case CharacterAttribute.Attribute_Dawn:
    //                time.attribute = CharacterAttribute.Attribute_Dawn;
    //                time.buffId = timeData.dawn_buff;
    //                time.timeSet = timeData.dawn;
    //                break;
    //            case CharacterAttribute.Attribute_Day:
    //                time.attribute = CharacterAttribute.Attribute_Day;
    //                time.buffId = timeData.day_buff;
    //                time.timeSet = timeData.day;
    //                break;
    //            case CharacterAttribute.Attribute_Night:
    //                time.attribute = CharacterAttribute.Attribute_Night;
    //                time.buffId = timeData.night_buff;
    //                time.timeSet = timeData.night;
    //                break;
    //        }

    //        var buffData = CSVDataManager.GetTable<BuffTable>().GetData(time.buffId);
    //        var buffIcon = buffData.buff_icon;
    //        var effectIndex = buffData.buff_effectidx;

    //        if (loadIcon.Contains(buffIcon) == false)
    //            loadIcon.Add(buffIcon);

    //        if (effectSource.Contains(effectIndex) == false)
    //            effectSource.Add(effectIndex);

    //        qAttribute.Enqueue(time);
    //        start++;
    //    }

    //    for(int i = 0; i < loadIcon.Count; i++)
    //    {
    //        ResourcePoolManager.Instance.LoadBuffIcon(loadIcon[i]);
    //    }

    //    for(int i = 0; i < effectSource.Count; i++)
    //    {
    //        var effectData = CSVDataManager.GetTable<EffectListTable>().GetData(effectSource[i]);
    //        for(int j =0; j < effectData.Length; j++)
    //        {
    //            ResourcePoolManager.Instance.LoadEffectResourceData(effectData[j]);
    //        }
    //    }

    //    character_Light = GameObject.Find("Character_Light");
    //    timeColor = GameObject.Find("MainLand").GetComponent<TimeColor>();
        
    //    SetImage();
    //    SwitchMaterialLight();
    //}

    //public void SearchCommonBuffCharacter()
    //{
    //    TimeData currentTimeData = (TimeData)qAttribute.Peek();
    //    CharacterAttribute currentAttribute = currentTimeData.attribute;
    //    int buffID = currentTimeData.buffId;

    //    List<CharacterBase> characters = BattleManager.Instance.playerList;
    //    if (characters == null)
    //        return;

    //    for (int i = 0; i < characters.Count; i++)
    //    {
    //        CharacterAttribute attri = characters[i].CharacterInfo.CharacterStatus.attribute;
    //        if (currentAttribute == attri && characters[i].CharacterInfo.Death == false)
    //        {
    //            characters[i].AttachBuff(buffID);
    //        }
    //    }
        
    //    characters = BattleManager.Instance.enemyList;
    //    for (int i = 0; i < characters.Count; i++)
    //    {
    //        CharacterAttribute attri = characters[i].CharacterInfo.CharacterStatus.attribute;
    //        if (currentAttribute == attri && characters[i].CharacterInfo.Death == false)
    //        {
    //            characters[i].AttachBuff(buffID);
    //        }
    //    }
    //}

    //private void CheckTime()
    //{
    //    if (currentTime <= 0)
    //    {
    //        currentTime = 0;
    //        SwitchingAttribute();
    //    }
            

    //    currentTime -= Time.deltaTime;
    //}

    //private void SwitchingAttribute()
    //{
    //    TimeData current = (TimeData)qAttribute.Peek();
        
    //    Messenger.Broadcast(Definition.RemoveTimeSystemBuff);

    //    qAttribute.Dequeue();
    //    qAttribute.Enqueue(current);


    //    var temp = (TimeData)qAttribute.Peek();
    //    switch (temp.attribute)
    //    {
    //        case CharacterAttribute.Attribute_Day:
    //            tempText.text = "It was Day.\n Day attribute Dinos become stronger.";//""낮!!!!!!!!!!";
    //            break;
    //        case CharacterAttribute.Attribute_Night:
    //            tempText.text = "It was night.\n Night attribute Dinos become stronger.";//""밤!!!!!!!!!!";
    //            break;
    //        case CharacterAttribute.Attribute_Dawn:
    //            tempText.text = "It was Dawn.\n Dawn attribute Dinos become stronger.";//""새벽!!!!!!!";
    //            break;
    //    }

    //    #region 임시 입니다......... 요청 하셔서......
    //    tempTextRoot.gameObject.SetActive(true);
    //    var targetPosition = tempTextRoot.transform.position + (100 * Vector3.up);
    //    Tweener tween = tempTextRoot.transform.DOMove(targetPosition, 1.5f);
    //    tween.onKill = () =>
    //    {
    //        tempTextRoot.gameObject.SetActive(false);
    //        //tempText.gameObject.SetActive(false);
    //        tempTextRoot.transform.position -= Vector3.up * 100;
    //    };
    //    #endregion

    //    SetImage();
    //    SwitchMaterialLight();

    //    SearchCommonBuffCharacter();
    //}

    //private void SetImage()
    //{
    //    var etor = qAttribute.GetEnumerator();
    //    int index = 0;

    //    while (etor.MoveNext())
    //    {
    //        var data = (TimeData)etor.Current;

    //        string attribute = data.attribute.ToString();
    //        string[] split = attribute.Split('_');

    //        var images = attributeImage[index].GetComponentsInChildren<Image>();
    //        for (int i = 0; i < images.Length; i++)
    //        {
    //            Tweener tween = images[i].DOFade(0, 0.1f);
    //            tween.onKill = () =>
    //            {

    //            };
    //        }
         
            
    //        var obj = attributeImage[index].FindChildByName($"{split.Last()}");
    //        var image = obj.GetComponent<Image>();
    //        Tweener tween1 = image.DOFade(1, 0.3f);
    //        tween1.onKill = () =>
    //        {

    //        };

    //        index++;
    //    }
    //}

    //private void SwitchMaterialLight()
    //{
    //    Color newColor = Color.black;
    //    TimeData data = (TimeData)qAttribute.Peek();
    //    CharacterAttribute current = data.attribute;
    //    currentTime = data.timeSet;
        
    //    switch (current)
    //    {
    //        case CharacterAttribute.Attribute_Dawn:
    //            newColor = timeColor.dawnColor;
    //            break;
    //        case CharacterAttribute.Attribute_Day:
    //            newColor = timeColor.dayColor;
    //            break;
    //        case CharacterAttribute.Attribute_Night:
    //            newColor = timeColor.nightColor;
    //            break;
    //    }
        
    //    Messenger.Broadcast(Definition.ChangeTimeColor, newColor);
        
    //    Tweener tween1 = character_Light.GetComponent<Light>().DOColor(newColor, 1f);
    //    tween1.onKill = () =>
    //    {

    //    };
    //}

    //private void StopTime(bool time)
    //{
    //    stop = time;
    //}
    
    //#region Message
    //private void MessageAddListner()
    //{
    //    Messenger.AddListener<bool>(Definition.StopTime, StopTime);
    //}

    //private void MessageRemoveListner()
    //{
    //    Messenger.RemoveListener<bool>(Definition.StopTime, StopTime);   
    //}
    //#endregion

    //private void OnDestroy()
    //{
    //    MessageRemoveListner();
    //}
}
