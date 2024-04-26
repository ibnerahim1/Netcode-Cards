using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Tools;

// Brain doesn't know about what metadata is about
// Don't do Brain for now, we need to do Visualization first
public class Brain : MonoBehaviour
{
    public ActionSO A_SwipeDown, A_SwipeUp, A_Swap;
    public CardsListSO myCards;
    public CardsListSO drawDeck;
    public CardsListSO discardDeck;
    public GlobalStateSO myTurn;

    public ComponentMetaData MyMetaData => ComponentMetaData.GetC_metaDataBrain(gameObject.GetInstanceID());

    private void OnEnable()
    {
        A_SwipeDown.OnActionPlayed += OnDrawAction;
        A_SwipeUp.OnActionPlayed += OnPlayAction;
        A_Swap.OnActionPlayed += OnSwapAction;
        myTurn.OnEnterState += MyTurnEntered;
        myTurn.OnExitState += MyTurnExited;
    }
    private void OnDisable()
    {
        A_SwipeDown.OnActionPlayed -= OnDrawAction;
        A_SwipeUp.OnActionPlayed -= OnPlayAction;
        A_Swap.OnActionPlayed -= OnSwapAction;
        myTurn.OnEnterState -= MyTurnEntered;
    }

    private void MyTurnEntered(ComponentMetaData C_metaData)
    {

    }

    private void MyTurnExited(ComponentMetaData C_metaData)
    {

    }

    public void OnDrawAction(CardSO[] i_cards, ComponentMetaData C_metaData)
    {
        //if (cards.playerTurn.IsMyTurn() && drawDeck.cards.GetCount()>0)
        if (drawDeck.GetCount() > 0)
        {
            myCards.AddElement(0, drawDeck.GetElement(0), MyMetaData);
            drawDeck.RemoveElement(0, MyMetaData);
        }
    }
    public void OnPlayAction(CardSO[] i_cards, ComponentMetaData C_metaData)
    {
        //if (cards.playerTurn.IsMyTurn() && cards.GetCount() > 0)
        discardDeck.AddElement(0, i_cards[0], MyMetaData);
        myCards.RemoveElement(i_cards[0], MyMetaData);
        //myTurn.FlipState();
        //cards.playerTurn.SwitchTurn();
    }
    public void OnSwapAction(CardSO[] i_cards, ComponentMetaData C_metaData)
    {
        myCards.MoveElement(i_cards[0], i_cards[1], MyMetaData);
    }
}