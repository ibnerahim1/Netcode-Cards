using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

/// <summary>
/// Visualizes the cards in the hands, but shows only back and the number
/// </summary>
public class V_Player_Host : MonoBehaviour
{
    public ActionSO A_Draw, A_Play, A_Swap;
    public Transform target;
    public CardsListSO cards;
    public CardVisualStyleSO cardVisualStyleSO;
    public Transform cardsParent;
    public EventTriggerSO aspectChanged;
    public float SwipeSensitivity = 4;
    public float selectedCardDistanceZ = 1;

    void MyTurnStarted(string uniqueName, bool serverRpcCall)
    {
        print(uniqueName + "'s turn started");
        transform.DOScale(Vector3.one * 1.5f, 0.2f);
    }

    void MyTurnEnded(string uniqueName, bool serverRpcCall)
    {
        print(uniqueName + "'s turn ended");
        transform.DOScale(Vector3.one, 0.2f);
    }

    private void OnEnable()
    {
        cards.OnAddElement += OnAddElement;
        cards.OnRemoveElement += OnRemoveElement;
        aspectChanged.OnTrigger += AspectChanged;
        cards.OnRemoveAllElements += OnRemoveAllElements;
    }
    private void OnDisable()
    {
        cards.OnAddElement -= OnAddElement;
        cards.OnRemoveElement -= OnRemoveElement;
        aspectChanged.OnTrigger -= AspectChanged;
        cards.OnRemoveAllElements -= OnRemoveAllElements;
    }

    private void AspectChanged()
    {
        transform.position = target.position;
        transform.eulerAngles = target.eulerAngles;
    }

    private void Awake()
    {
        OnRemoveAllElements(new ComponentMetaData()); 
        cards.GetList().ForEach((x, i) => InstantiateCard(i, x, new ComponentMetaData()));
    }

    private void OnRemoveAllElements(ComponentMetaData C_metaData)
    {
        int N = cardsParent.childCount;
        for (int i = N - 1; i >= 0; i--) Destroy(cardsParent.GetChild(i).gameObject);
    }

    private void InstantiateCard(int index, CardSO newCard, ComponentMetaData C_metaData)
    {
        CardParent temp = Instantiate(cardVisualStyleSO.GetValue(newCard)).GetComponent<CardParent>();
        temp.transform.parent = cardsParent;
        temp.transform.SetSiblingIndex(index);
        temp.SetPosition(new Vector3(cardsParent.position.x - (cards.GetCount() * 1), cardsParent.position.y, cardsParent.position.z + (cards.GetCount() * 0.25f)));
        temp.transform.GetChild(0).GetComponent<BoxCollider>().enabled = false;
        temp.OnHost = true;

        UpdateCards();
    }

    private void OnAddElement(int index, CardSO newCard, ComponentMetaData C_metaData)
    {
        InstantiateCard(index, newCard, C_metaData);
    }

    private void OnRemoveElement(int index, ComponentMetaData C_metaData)
    {
        if (index >= cardsParent.childCount) return;
        CardParent temp = cardsParent.GetChild(index).GetComponent<CardParent>();
        temp.transform.parent = null;
        Destroy(temp.gameObject);
        UpdateCards();
    }

    private void UpdateCards()
    {
        CardParent[] cards = cardsParent.GetComponentsInChildren<CardParent>();
        transform.GetChild(1).GetComponent<TextMeshPro>().text = cards.Length.ToString();
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].SetPosition(cardsParent.position);
            cards[i].SetRotation(new Vector3(0, 180, Mathf.Clamp((i - (cards.Length / 2f)) * 20, -60, 60)), true);
            cards[i].SetPosition(cards[i].target.position + cards[i].target.up * 0.25f + Vector3.back * 0.02f * i);
            cards[i].SetScale(Vector3.one * 0.5f);
        }
    }

    public void DrawCard()
    {
        A_Draw.PlayAction(new CardSO[] { }, ComponentMetaData.GetC_metaDataAction(gameObject.GetInstanceID()));
    }
}
