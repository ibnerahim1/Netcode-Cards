using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

/*
 * The assumption is that:
 * There may be only single "card" of each instances of the ScriptableObject
 */
[CreateAssetMenu(fileName = "Card", menuName = "Gadget-top SOs/CardSO")]
public class CardSO : ScriptableObject 
{

    #region Static
    public static CardSO[] JsonToList(string cardsJson)
    {
        string[] cardNames = JsonConvert.DeserializeObject<string[]>(cardsJson);
        var cards = cardNames.Select(x => CardSO.GetByName(x)).ToArray();
        return cards;
    }

    public static string ListToJson(CardSO [] cards)
    {
        return JsonConvert.SerializeObject(cards.Select(x => x.name).ToArray());
    }

    public static string pathToAssetsCards = "ScriptableObjects/Cards";

    public static CardSO GetByName(string cardUniqueName)
    {
        return Resources.Load<CardSO>($"{pathToAssetsCards}/{cardUniqueName}");
    }
    #endregion
}
