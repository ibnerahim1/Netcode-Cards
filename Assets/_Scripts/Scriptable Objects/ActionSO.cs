using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player 1 made an action "swipe up"
/// Each player and each action has its own ActionSO
/// That gives you an idea which CardsListSO to apply changes to
/// </summary>
[CreateAssetMenu(fileName = "Action", menuName = "Gadget-top SOs/ActionSO")]
public class ActionSO : ScriptableObject
{
    public static string pathToAssetsActions = "ScriptableVariables/Actions";

    public delegate void ActionDelegate(CardSO[] cardsList, ComponentMetaData C_metaData);
    public event ActionDelegate OnActionPlayed;

    public void PlayAction(CardSO [] cardsList, ComponentMetaData C_metaData)
    {
        OnActionPlayed?.Invoke(cardsList, C_metaData);
    }
    public static ActionSO GetByName(string actionUniqueName)
    {
        return Resources.Load<ActionSO>($"{pathToAssetsActions}/{actionUniqueName}");
    }
}
