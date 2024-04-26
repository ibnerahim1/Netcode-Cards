using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Tools;

public class V_DiscardDeck : Singleton<V_DiscardDeck>
{
    public Transform target;
    public CardsListSO cards;
    public CardVisualStyleSO cardVisualStyleSO;
    public EventTriggerSO aspectChanged;

    public ComponentMetaData MyMetaData => ComponentMetaData.GetC_metaDataVisualization(gameObject.GetInstanceID());


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
        temp.transform.SetAsFirstSibling();
        temp.SetPosition(new Vector3(transform.position.x + Random.Range(-0.3f, 0.3f), transform.position.y + Random.Range(-0.3f, 0.3f), transform.position.z - (transform.childCount * 0.02f)));
        temp.SetRotation(Vector3.forward * Random.Range(-60f, 60f));
        temp.SetScale(Vector3.one);
        temp.OnHost = true;
        temp.DiscardDeck = true;
    }

    private void OnRemoveElement(int index, ComponentMetaData C_metaData)
    {
        CardParent temp = transform.GetChild(index).GetComponent<CardParent>();
        Destroy(temp.gameObject);
    }
}