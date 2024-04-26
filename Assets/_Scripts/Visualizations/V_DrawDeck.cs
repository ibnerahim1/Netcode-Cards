using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Tools;

public class V_DrawDeck : Singleton<V_DrawDeck>
{
    public Transform target;
    public CardsListSO cards;
    public CardVisualStyleSO cardVisualStyleSO;
    public EventTriggerSO aspectChanged;

    private void Start()
    {
        for (int i = 0; i < cards.GetCount(); i++)
        {
            OnAddElement(0, cards.GetElement(i), new ComponentMetaData());
        }
    }

    private void OnEnable()
    {
        cards.OnAddElement += OnAddElement;
        cards.OnRemoveElement += OnRemoveElement;
        aspectChanged.OnTrigger += AspectChanged;
    }
    private void OnDisable()
    {
        cards.OnAddElement -= OnAddElement;
        cards.OnRemoveElement -= OnRemoveElement;
        aspectChanged.OnTrigger -= AspectChanged;
    }

    private void AspectChanged()
    {
        transform.position = target.position;
    }

    private void OnAddElement(int index, CardSO newCard, ComponentMetaData C_metaData)
    {
        CardParent temp = Instantiate(cardVisualStyleSO.GetValue(newCard)).GetComponent<CardParent>();
        temp.transform.parent = transform;
        temp.transform.SetSiblingIndex(index);
        temp.SetScale(new Vector3(1,1,-1));
        temp.OnHost = true;

        UpdateTargets();
    }

    private void OnRemoveElement(int index, ComponentMetaData C_metaData)
    {
        CardParent temp = transform.GetChild(index).GetComponent<CardParent>();
        Destroy(temp.gameObject);
        UpdateTargets();
    }

    private void UpdateTargets()
    {
        CardParent[] cards = GetComponentsInChildren<CardParent>();
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].SetPosition(new Vector3(transform.position.x, transform.position.y, transform.position.z + (cards[i].transform.GetSiblingIndex() * 0.02f)));
            cards[i].SetRotation(Vector3.forward * 0.5f * cards[i].transform.GetSiblingIndex());
        }
    }
}