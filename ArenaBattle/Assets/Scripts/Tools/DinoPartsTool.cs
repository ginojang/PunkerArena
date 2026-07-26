using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

public class DinoPartsTool : MonoBehaviour
{
    [SerializeField]
    private static GameObject dino = null;

    private Dictionary<string, GameObject> dinoBasePool = new Dictionary<string, GameObject>();

    private string baseFilePath = "";//"Assets/Asset/Cha/Dino/orange/prefab/";
    private string baseCharName_ca = "ca_orange_01";
    private string baseCharName_om = "om_orange_01";
    private string baseCharName_he = "he_orange_01";

    private CharacterTalent curTalent = CharacterTalent.Carnivore;
    private CharacterClass curClass = CharacterClass.orange;
#if UNITY_EDITOR && !UNITY_ANDROID
    private Texture button_select_tex = null;
#endif
    private Dictionary<ItemType, selectedPartInfo> initData = new Dictionary<ItemType, selectedPartInfo>();
    private Dictionary<ItemType, string> selectedNames = new Dictionary<ItemType, string>();

    public bool bInit = false;
    public bool bInitAsset = false;

    public GameObject DINO
	{
		get { return dino; }
	}

    void Awake()
	{
        AssetManager.Instance.Initialize(OnAssetManagerIntializeComplete);
    }

    void Start()
    {
//        Destroy(dino);
//        Destroy(GameObject.Find("AssetManager"));
//        Destroy(GameObject.Find("__immortal__"));

#if UNITY_EDITOR && !UNITY_ANDROID
        button_select_tex = (Texture)AssetDatabase.LoadAssetAtPath("Assets/EditorResources/Btnbg_Selected.png", typeof(Texture));
#endif
        Messenger.AddListener<int, GameObject>(Definition.Dino_Costume_End, OnCompleteDinoCostume);

        bInit = true;
    }

    public void Initialize(Dictionary<ItemType, selectedPartInfo> list, Dictionary<ItemType, string> names)
	{
        bInit = false;
        initData = list;
		selectedNames = names;

        LoadStartCharacter(baseCharName_ca, initData);
    }

	protected virtual void OnAssetManagerIntializeComplete()
    {
        UiAddressablePoolManager.Instance.Initialize();

        bInitAsset = true;
        //LoadStartCharacter(initData);
    }

    private void OnCompleteDinoCostume(int index, GameObject newDino)
	{
        dino = newDino;
        Character character = dino.GetComponent<Character>();

        //dino.transform.Rotate(Vector3.up * 90.0f);
/*
        for (int type = 0; type < (int)ItemType.Max; type++)
        {
            initData[(ItemType)type].charTalent = curTalent;

            initData[(ItemType)type].partItemName = ChangeTalentPartName(initData[(ItemType)type].partItemName, curTalent);

            selectedNames[(ItemType)type] = ChangeTalentPartName(selectedNames[(ItemType)type], curTalent);
        }

//        character.CharacterInfo.costumeInfo = new costumCharInfo(CharacterTalent.Carnivore, CharacterClass.banana, dino.transform);
        CostumeManager.Instance.AddCostoumCharacter(dino, 0);*/
    }

    private string ChangeTalentPartName(string source, CharacterTalent destTalent)
    {
        string prefix = "";
        string part = "";
        switch (curTalent)
        {
            case CharacterTalent.Carnivore:
                {
                    prefix = "ca_";
                }
                break;
            case CharacterTalent.Omnivore:
                {
                    prefix = "om_";
                }
                break;
            case CharacterTalent.Herbivore:
                {
                    prefix = "he_";
                }
                break;
        }

        if (source != string.Empty)
        {
            string[] partnames = source.Split('_');
            string partname = prefix;
            for (int o = 1; o < partnames.Length; o++)
            {
                partname += partnames[o];
                if (o < partnames.Length - 1)
                    partname += '_';
            }

            part = partname;
        }

        return part;
    }

    static public T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy as T;
    }

    // Update is called once per frame
    float rotSpeed = 2.0f; //ADD

    // Update is called once per frame
    float deltaTime = 0;
    public void OnDrag(PointerEventData eventData)
    {
        float x = eventData.delta.x * Time.deltaTime * rotSpeed;
        float y = eventData.delta.y * Time.deltaTime * rotSpeed;

        dino.transform.Rotate(0, -x, y, Space.World);

        Debug.Log("드래그");
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        
        if (Input.GetMouseButton(0))
        {
            float MouseX = Input.GetAxis("Mouse X");
            float MouseY = Input.GetAxis("Mouse Y");

            dino.transform.Rotate(0, rotSpeed * -MouseX, rotSpeed * -MouseY);
            //dino.transform.Rotate(Vector3.right * rotSpeed * MouseY);
            //RotateToMouseDir();
        }

        if (Input.GetKeyDown(KeyCode.Delete))
		{
            DeleteObject();
		}
    }

    void RotateToMouseDir()
    {
        // 현재 마우스 포지션에서 정면방향 * 10으로 이동한 위치의 월드좌표 구하기
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 10f);

        // Atan2를 이용하면 높이와 밑변(tan)으로 라디안(Radian)을 구할 수 있음
        // Mathf.Rad2Deg를 곱해서 라디안(Radian)값을 도수법(Degree)으로 변환
        float angle = Mathf.Atan2(
            dino.transform.position.y - mouseWorldPosition.y,
            dino.transform.position.x - mouseWorldPosition.x) * Mathf.Rad2Deg;

        // angle이 0~180의 각도라서 보정
        float final = -(angle + 90f);
        // 로그를 통해서 값 확인
        Debug.Log(angle + " / " + final);

        // Y축 회전
        dino.transform.rotation = Quaternion.Euler(new Vector3(0f, final, 0f));
    }

    public void DeleteObject()
	{
        Destroy(dino);
        dino = null;
    }

    private void OnGUI()
    {
        int w = Screen.width, h = Screen.height;
        GUIStyle style = new GUIStyle();
        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 70;
        style.normal.textColor = Color.green;
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
        
//        int w = Screen.width, h = Screen.height;
        style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = 20;
#if UNITY_EDITOR && !UNITY_ANDROID
        style.normal.background = (Texture2D)button_select_tex;
#endif
        if (GUI.Button(new Rect(20, 100, 200, 70), "Reset Character", style))
        {
            dino.transform.rotation = Quaternion.identity;
        }
    }

	private void OnDestroy()
	{
        Messenger.RemoveListener<int, GameObject>(Definition.Dino_Costume_End, OnCompleteDinoCostume);

        if (Application.isPlaying)
        {
            Destroy(dino);
            Destroy(GameObject.Find("AssetManager"));
            Destroy(GameObject.Find("__immortal__"));
        }
        else
        {
            DestroyImmediate(dino);
            DestroyImmediate(GameObject.Find("AssetManager"));
            DestroyImmediate(GameObject.Find("__immortal__"));
        }
    }

	void CreatePrimitive()
	{
        PrimitiveType type = PrimitiveType.Cube;
        DestroyImmediate(dino);
        int rand = UnityEngine.Random.Range(0, 4);
        switch (rand)
        {
            case 0:
				{
                    type = PrimitiveType.Cube;
				}
                break;
            case 1:
                {
                    type = PrimitiveType.Capsule;
                }
                break;
            case 2:
                {
                    type = PrimitiveType.Cylinder;
                }
                break;
            case 3:
                {
                    type = PrimitiveType.Sphere;
                }
                break;
        }

        dino = GameObject.CreatePrimitive(type);
        dino.transform.position = Vector3.zero;
    }

    Character backChar = null;
    static bool bAttack = false;
    public void ChangeAnimation(string aniName)
	{
        bAttack = !bAttack;
        //        Character charBase = dino.GetComponent<Character>();
        //        charBase.Get_AnimBase.Set_AttackIdle(charBase, true);

        backChar = dino.GetComponent<Character>();
        backChar.ChangeMouthObject(bAttack);
//        CostumeManager.Instance.ChangeDinoAttackMouth(dino, bAttack);
	}

	public void LoadStartCharacter(string filename, Dictionary<ItemType, selectedPartInfo> partlist = null)
	{
        DeleteObject();

        Dictionary<ItemType, string> list = new Dictionary<ItemType, string>();

        foreach (var item in partlist)
        {
            list.Add(item.Key, item.Value.partItemName);
        }

        CostumeManager.Instance.LoadBaseCharacter(filename, curTalent, curClass, list, null, true);
    }

    public void ChangePart(selectedPartInfo info)
    {
        if (!Application.isPlaying)
            return;

        Debug.Log($"Selected Item Name  :  {info.partItemName}");

        if (info.part == ItemType.body)
            curClass = info.charClass;

        CostumeManager.Instance.ChangeDinoPart(dino, curTalent, curClass, info.partItemName, info.part);
    }

    public void ChangeParts(CharacterTalent talent, CharacterClass charClass, Dictionary<ItemType, selectedPartInfo> itemlist)
	{
        if (!Application.isPlaying)
            return;

//        if (curTalent == talent)
//			return;

        string prefix = "";
		string filename = "";

		switch (talent)
		{
            case CharacterTalent.Carnivore:
				{
                    filename = $"{baseFilePath}{baseCharName_ca}";
                    prefix = "ca_";
				}
                break;
            case CharacterTalent.Omnivore:
				{
                    filename = $"{baseFilePath}{baseCharName_om}";
                    prefix = "om_";
                }
                break;
            case CharacterTalent.Herbivore:
				{
                    filename = $"{baseFilePath}{baseCharName_he}";
                    prefix = "he_";
                }
                break;
		}

        //        DeleteObject();

        Dictionary<ItemType, string> partlist = new Dictionary<ItemType, string>();

        foreach(var item in itemlist)
		{
            if(item.Value.partItemName != string.Empty)
			{
                string[] partnames = item.Value.partItemName.Split('_');
                string partname = prefix;
                for(int o = 1; o < partnames.Length; o++)
				{
                    partname += partnames[o];
                    if (o < partnames.Length - 1)
                        partname += '_';
				}

                partlist.Add(item.Key, partname);
            }
            else
                partlist.Add(item.Key, item.Value.partItemName);
		}
        costumCharInfo info = new costumCharInfo(curTalent, curClass, dino.transform);
        info.model = dino;
        //CostumeManager.Instance.AddCostoumCharacter(dino, 0);
        CostumeManager.Instance.ChangeDinoPartsList(info, talent, curClass, -1, partlist);

        curTalent = talent;
    }

    public void LoadPartsData(string partname)
	{
        List<object> param = new List<object>();
        List<AssetLoader> baseCharList = new List<AssetLoader>();

        for (int i = 1; i < 11; i++)
        {
            // ?????? ????, ?????? ?? ???????? ???? ??.
            AssetLoader fishLoader = AssetManager.Instance.PreLoadAsset($"Assets/Asset/Character/Character{i}.prefab", null);
            baseCharList.Add(fishLoader);
            param.Add(fishLoader);
        }
        AssetManager.Instance.LoadAssetAsyncForPreloadAsset(baseCharList, FinishCharacterLoad, param);
    }

    private void FinishCharacterLoad(object p)
    {
        List<object> charLoader = (List<object>)p;

        for (int i = 0; i < charLoader.Count; i++)
        {
            AssetLoader charobj = (AssetLoader)charLoader[i];
            GameObject charObj = charobj.MainAsset as GameObject;

 //           baseCharacterList.Add(charObj);
        }
    }

    private void OnSuccessCostume()
	{

	}
}
