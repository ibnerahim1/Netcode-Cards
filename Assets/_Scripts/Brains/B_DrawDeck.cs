using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Extensions;

public class B_CreateDrawDeck : MonoBehaviour
{
    public CardsListSO drawDeck, discardDeck;

    public int seed;

    public ComponentMetaData MyMetaData => ComponentMetaData.GetC_metaDataBrain(gameObject.GetInstanceID());

    private void Awake()
    {
        //CardsListSO.Init();
        ShuffleDeck();
    }
    private void Start()
    {
        this.DelayedAction(() =>
        {
            CardSO card = drawDeck.GetElement(0);
            discardDeck.AddElement(0, card, MyMetaData);
            drawDeck.RemoveElement(0, MyMetaData);
        }, 0.01f);
    }

    private void ShuffleDeck()
    {
        //Random.State state = Random.state;
        //Random.InitState(seed);
        for (int i = 0; i < drawDeck.GetCount(); i++)
        {
            drawDeck.MoveElement(i, Random.Range(0, drawDeck.GetCount()), MyMetaData);
        }
        //Random.state = state;
    }
}