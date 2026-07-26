using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DamageUi : MonoBehaviour
{
    [SerializeField] private Text damageTxt;
    private Text[] damageUI;
    private float moveTime;
    private bool critical = false;
    
    // Start is called before the first frame update
    void Start()
    {
        DamageUIInitialize();
        Messenger.AddListener<CharacterBase, int, bool>(Definition.DamageUIOn, DamageUIOn);
        Messenger.AddListener<CharacterBase>(Definition.EvadeDamageOn, EvadeDamageOn);
        Messenger.AddListener<CharacterBase, int>(Definition.HealDamageOn, HealDamageOn);
    }
    private void DamageUIInitialize()
    {
        damageUI = new Text[30];
        for(int i = 0; i < damageUI.Length; i++)
        {
            damageUI[i] = GameObject.Instantiate(damageTxt, gameObject.transform);
            damageUI[i].transform.localPosition = Vector3.zero;
            damageUI[i].gameObject.SetActive(false);
        }
    }
    #region 데미지 UI
    private void DamageUIOn(CharacterBase target, int damage, bool critical)
    {
        var text = FindEmptyText();
        if (text == null)
            return;

        Transform criticalText = text.transform.Find("Critical");
        text.text = damage.ToString();

        Vector3 position = Camera.main.WorldToScreenPoint(target.transform.position);
        Vector3 parentPosition = position + (Vector3.up * 90);

        text.gameObject.transform.position = parentPosition;

        Vector3 targetPosition = text.gameObject.transform.position;
        targetPosition += (Vector3.up * 180);

        text.gameObject.SetActive(true);
        criticalText.gameObject.SetActive(critical);


        Tweener tween = text.gameObject.transform.DOMove(targetPosition, 2f);
        tween.onKill = () =>
        {
            text.text = "";
            text.gameObject.transform.localPosition = Vector2.zero;
            text.gameObject.SetActive(false);
            criticalText.gameObject.SetActive(false);
        };
    }
    private void HealDamageOn(CharacterBase target, int value)
    {
        var text = FindEmptyText();
        if (text == null)
            return;

        Transform criticalText = text.transform.Find("Critical");
        text.text = ((int)value).ToString();

        Vector3 position = Camera.main.WorldToScreenPoint(target.transform.position);
        Vector3 parentPosition = position + (Vector3.up * 90);

        text.gameObject.transform.position = parentPosition;

        Vector3 targetPosition = text.gameObject.transform.position;
        targetPosition += (Vector3.up * 180);

        text.gameObject.SetActive(true);
        criticalText.gameObject.SetActive(critical);


        Tweener tween = text.gameObject.transform.DOMove(targetPosition, 2f);
        tween.onKill = () =>
        {
            text.text = "";
            text.gameObject.transform.localPosition = Vector2.zero;
            text.gameObject.SetActive(false);
            criticalText.gameObject.SetActive(false);
        };
    }

    private void EvadeDamageOn(CharacterBase target)
    {
        var text = FindEmptyText();

        text.text = "Miss!";
        Vector3 position = Camera.main.WorldToScreenPoint(target.transform.position);
        Vector3 parentPosition = position + (Vector3.up * 90);

        text.gameObject.transform.position = parentPosition;

        Vector3 targetPosition = text.gameObject.transform.position;
        targetPosition += (Vector3.up * 180);

        text.gameObject.SetActive(true);

        Tweener tween = text.gameObject.transform.DOMove(targetPosition, 2f);
        tween.onKill = () =>
        {
            text.text = "";
            text.gameObject.transform.localPosition = Vector2.zero;
            text.gameObject.SetActive(false);
        };
    }
    private Text FindEmptyText()
    {
        foreach(var text in damageUI)
        {
            if (text.gameObject.activeSelf == false)
                return text;
        }
        return null;
    }
    #endregion

    private void OnDestroy()
    {
        Messenger.RemoveListener<CharacterBase, int, bool>(Definition.DamageUIOn, DamageUIOn);
        Messenger.RemoveListener<CharacterBase>(Definition.EvadeDamageOn, EvadeDamageOn);
        Messenger.RemoveListener<CharacterBase, int>(Definition.HealDamageOn, HealDamageOn);
    }
}
