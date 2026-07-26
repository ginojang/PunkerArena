using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Turn : MonoBehaviour
{
    private GameObject enemy;
    private GameObject player;
    private GameObject move;
    private GameObject round;
    private GameObject empty;
    private GameObject attributeProperty;

    private CharacterProfile CharacterProfile { set; get; }
    [SerializeField]public RawImage allyImage;
    [SerializeField]public RawImage enemyImage;

    private Attributes attributes;
    private int turnNumber = -1;

    private CharacterProfile MoveProfile { set; get; }
    private void Awake()
    {
        InitailizeProfile();
    }
    private void InitailizeProfile()
    {
        enemy = gameObject.transform.Find("Enemy").gameObject;
        player = gameObject.transform.Find("Team").gameObject;
        move = gameObject.transform.Find("Move").gameObject;
        empty = gameObject.transform.Find("Empty").gameObject;
        round = gameObject.transform.Find("Round").gameObject;
        attributeProperty = round.transform.Find("Property").gameObject;

        allyImage = player.GetComponentInChildren<RawImage>();
        enemyImage = enemy.GetComponentInChildren<RawImage>();

        enemy.SetActive(false);
        player.SetActive(false);
        move.SetActive(false);
        empty.SetActive(false);
        round.SetActive(true);

        MessageAddListner();
    }
    public void SetProfile(CharacterProfile profile, int turn)
    {
        if(turn != turnNumber)
        {
            turnNumber = turn;
            SetAttribute();

            if (turnNumber % 10 == 0)
            {
                string attributeIcon = $"icon_Attribute_{attributes}";
                ResourcePoolManager.instance_value.AsyncGetData("Ingame", objectType.atlasSprite, null, null, (icon) =>
                {
                    attributeProperty.GetComponent<Image>().sprite = (Sprite)icon;
                    attributeProperty.SetActive(true);
                }, attributeIcon);
            }
            else
            {
                attributeProperty.SetActive(false);
            }
        }

        move.SetActive(false);
        if (profile == null)
        {
            CharacterProfile = profile;
            allyImage.texture = Texture2D.whiteTexture;
            enemyImage.texture = Texture2D.whiteTexture;

            enemy.SetActive(false);
            player.SetActive(false);
            empty.SetActive(true);
            round.SetActive(true);
            return;
        }

        if (CharacterProfile == profile)
            return;

        CharacterProfile = profile;
        empty.SetActive(false);
        
        Camp camp = profile.camp;
        GameObject obj = null;
        switch(camp)
        {
            case Camp.Ally:
                obj = player;
                enemy.SetActive(false);
                break;
            case Camp.Enemy:
                obj = enemy;
                player.SetActive(false);
                break;
        }
        var portrait = obj.GetComponentInChildren<RawImage>();
        var property = obj.transform.Find("Property").gameObject.GetComponent<Image>();
        var battleTag = obj.transform.Find("Turn").gameObject.GetComponent<Image>();

        portrait.texture = profile.snap;
        battleTag.sprite = profile.battleTag;
        property.sprite = profile.attribute;


        obj.SetActive(true);
    }

    private void SetAttribute()
    {
        Attributes current = InGameData.Instance.CurrentAttribute;
        int currentRound = InGameData.Instance.CurrentRound;
        int result = (turnNumber / 10) + 1;

        attributes = current;

        for (int i = currentRound; i < result; i++)
        {
            attributes++;
            if (attributes >= Attributes.Eclipse)
                attributes = Attributes.Day;
        }

        Image[] images = round.GetComponentsInChildren<Image>();
        for(int i = 0; i < images.Length; i++)
            images[i].gameObject.SetActive(false);

        var attribute = round.transform.Find(attributes.ToString());
        attribute.gameObject.SetActive(true);
    }

    public void SetTempProfile(CharacterProfile profile)
    {
         MoveProfile = profile;

        var portrait = move.GetComponentInChildren<RawImage>();
        var property = move.transform.Find("Property").gameObject.GetComponent<Image>();
        var battleTag = move.transform.Find("Turn").gameObject.GetComponent<Image>();

        portrait.texture = MoveProfile.snap;
        battleTag.sprite = MoveProfile.battleTag;
        property.sprite = MoveProfile.attribute;

        move.SetActive(true);
    }
    private void SetTempProfileOff(CharacterProfile prevProfile)
    {
        if (prevProfile != MoveProfile)
            return;

        MoveProfile = null;
        move.SetActive(false);
    }

    private void SetTempProfileAllOff()
    {
        MoveProfile = null;
        move.SetActive(false);
    }
    private void MessageAddListner()
    {
        Messenger.AddListener<CharacterProfile>(Definition.SetNextTurnUIOff, SetTempProfileOff);
        //Messenger.AddListener(Definition.SetNextTurnUIAllOff, SetTempProfileAllOff);
    }
    private void MessageRemoveListner()
    {
        Messenger.RemoveListener<CharacterProfile>(Definition.SetNextTurnUIOff, SetTempProfileOff);
        //Messenger.RemoveListener(Definition.SetNextTurnUIAllOff, SetTempProfileAllOff);
    }

    private void OnDestroy()
    {
        MessageRemoveListner();
    }
}
