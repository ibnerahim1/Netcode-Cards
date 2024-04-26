using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Tools;
using System.Linq;
using Unity.Linq;
using Game.Extensions;

/// <summary>
/// Visualizes the cards in the players hand
/// </summary>
public class V_Player_Client : MonoBehaviour
{
    public ActionSO A_SwipeDown, A_SwipeUp;
    public CardsListSO cards;
    public CardVisualStyleSO cardVisualStyleSO;

    private Camera cam;
    private Vector3 mousePos;

    public ComponentMetaData MyMetaData => ComponentMetaData.GetC_metaDataAction(gameObject.GetInstanceID());

    // Visualization doesn't know about the brain because it is game agnostic solution
    // Visualization emits signals that the card was swiped

    private void Awake()
    {
        OnRemoveAllElements(new ComponentMetaData());
        cards.GetList().ForEach((x, i) => InstantiateCard(i, x, new ComponentMetaData()));
    }

    private void Start()
    {
        cam = Camera.main;
        if (!cards)
            cards = CardsListSO.getDefault();
    }

    private void OnEnable()
    {
        cards.OnAddElement += OnAddElement;
        cards.OnRemoveElement += OnRemoveElement;
        cards.OnRemoveAllElements += OnRemoveAllElements;
        //cards.OnMoveElement += OnMoveElement;
    }
    private void OnDisable()
    {
        cards.OnAddElement -= OnAddElement;
        cards.OnRemoveElement -= OnRemoveElement;
        cards.OnRemoveAllElements -= OnRemoveAllElements;
        //cards.OnMoveElement -= OnMoveElement;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePos = cam.ScreenToViewportPoint(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (mousePos.y > swipeUpBound && cam.ScreenToViewportPoint(Input.mousePosition).y < swipeDownBound)
            {
                if (A_SwipeDown != null)
                    A_SwipeDown.PlayAction(new CardSO[] { }, MyMetaData);
            }
        }
    }

    private void OnRemoveAllElements(ComponentMetaData C_metaData)
    {
        int N = transform.childCount;
        for (int i = N - 1; i >= 0; i--) Destroy(transform.GetChild(i).gameObject);
    }

    private void InstantiateCard(int index, CardSO newCard, ComponentMetaData C_metaData)
    {
        if (index > transform.childCount) return;
        CardParent temp = Instantiate(cardVisualStyleSO.GetValue(newCard), transform).GetComponent<CardParent>();
        temp.transform.SetSiblingIndex(index);

        CardParent.ArrangeCardsInHand(transform);
        temp.owner = cards;
        temp.OnInteracted += OnInteractedwithCard;
    }

    private void OnAddElement(int index, CardSO newCard, ComponentMetaData C_metaData)
    {
        InstantiateCard(index, newCard, C_metaData);
    }

    private void OnRemoveElement(int index, ComponentMetaData C_metaData)
    {
        if (index >= transform.childCount) return;
        CardParent temp = transform.GetChild(index).GetComponent<CardParent>();
        temp.transform.parent = null;
        temp.SetPosition(temp.target.position + Vector3.up * 4);
        Destroy(temp.gameObject, 0.2f);
        this.DelayedAction(() =>
        {
            CardParent.ArrangeCardsInHand(transform);
        }, 0.21f);
    }

    public float swipeUpBound = 0.8f;
    public float swipeDownBound = 0.6f;

    private void OnInteractedwithCard(Vector3 mousePos1, CardParent cardParent)
    {
        if (mousePos1.y > swipeUpBound/* && b_Player.cards.playerTurn.IsMyTurn()*/)
        {
            if (A_SwipeUp != null)
                A_SwipeUp.PlayAction(new CardSO[] { cardParent.card }, MyMetaData);
        }
        else
        {
            CheckCardsOrder(cardParent);
        }
    }

    private void CheckCardsOrder(CardParent selectedCard)
    {
        for (int i = selectedCard.transform.GetSiblingIndex(); i >= 0; --i)
        {
            if (selectedCard.target.position.x < transform.GetChild(i).transform.GetComponent<CardParent>().target.position.x)
            {
                CardSO card = transform.GetChild(i).GetComponent<CardParent>().card;
                cards.MoveElement(selectedCard.card, card, MyMetaData);
                selectedCard.transform.SetSiblingIndex(i);
            }
        }
        for (int i = selectedCard.transform.GetSiblingIndex(); i < transform.childCount; ++i)
        {
            if (selectedCard.target.position.x > transform.GetChild(i).transform.GetComponent<CardParent>().target.position.x)
            {
                CardSO card = transform.GetChild(i).GetComponent<CardParent>().card;
                cards.MoveElement(card, selectedCard.card, MyMetaData);
                transform.GetChild(i).SetSiblingIndex(i - 1);
            }
        }
    }
}