using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif

[CreateAssetMenu(fileName = "CardList", menuName = "Gadget-top SOs/CardsListSO")]
public class CardsListSO : ScriptableObject
{
    #region Static
    public static CardsListSO _default = null;
    public static CardsListSO getDefault()
    {
        Debug.Log("Instantiating CardsListSO in real time! Either you are testing something or there is something wrong (some componenets were not initialized)!");
        if (_default == null) _default = ScriptableObject.CreateInstance<CardsListSO>();
        return (_default);
    }

    public static string pathToAssetsCardsLists = "ScriptableVariables/CardsLists";

    //private void Awake()
    //{
    //    Init();
    //}
    //public static void Init()
    //{
    //    Debug.Log("cardsList Awake()");
    //    foreach (CardsListSO x in Resources.LoadAll(pathToAssetsCardsLists))
    //    {
    //        x.SetDefault();
    //    };
    //}

#if UNITY_EDITOR

    /// <summary>
    /// Initialize the list of cards when we build the game
    /// </summary>
    class MyCustomBuildProcessor : IPreprocessBuildWithReport
    {
        public int callbackOrder { get { return 0; } }

        public void OnPreprocessBuild(BuildReport report)
        {
            Debug.Log("OnPreprocessBuild()");
            foreach (CardsListSO x in UtilsFiles.GetAtPath<CardsListSO>("Resources/" + pathToAssetsCardsLists))
            {
                x.SetDefault();
            };
        }
    }

    /// <summary>
    /// Initialize the list of cards when we run the game in the editor
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnRuntimeMethodLoad()
    {
        Debug.Log("OnRuntimeMethodLoad()");
        foreach (CardsListSO x in UtilsFiles.GetAtPath<CardsListSO>("Resources/" + pathToAssetsCardsLists))
        {
            x.SetDefault();
        };
    }


#endif


    internal static CardsListSO GetByName(string cardUniqueName)
    {
        return Resources.Load<CardsListSO>($"{pathToAssetsCardsLists}/{cardUniqueName}");
    }

    #endregion

    public string pathToAssetsCards = "Assets/ScriptableObjects/Cards";

    public delegate void AddElementDelegate(int index, CardSO newCard, ComponentMetaData C_metaData);

    public event AddElementDelegate OnAddElement;
    public delegate void RemoveElementDelegate(int index, ComponentMetaData C_metaData);
    public event RemoveElementDelegate OnRemoveElement;
    public delegate void MoveElementDelegate(int index1, int index2, ComponentMetaData C_metaData);
    public event MoveElementDelegate OnMoveElement;
    public delegate void RemoveAllElementDelegate(ComponentMetaData C_metaData);
    public event RemoveAllElementDelegate OnRemoveAllElements;

    [SerializeField]
    private List<CardSO> cardsOnStart = new List<CardSO>();

    [SerializeField]
    private List<CardSO> cardsNow = new List<CardSO>(); // NOTE: The assumption is that all these cards are unique all the time

    // This object is closed for modification, it is responsible for the list of cards only

    /// <summary>
    /// Copy the list of cards from cardsOnStart to cardsNow
    /// </summary>
    private void SetDefault()
    {
        Debug.Log("SetDefault()");
        cardsNow = cardsOnStart.Select(x => x).ToList();
    }

    public void CreateFromJson(string cardsJson)
    {
        var newCards = CardSO.JsonToList(cardsJson);
        //TODO: modify this function so it won't remove the elements that are already in the list
        var N = cardsNow.Count;
        Debug.Log("N = " + N);
        for (int i = N - 1; i >= 0; i--)
        {
            Debug.Log("Removing element "+i);
            RemoveElement(i, new ComponentMetaData());
        }
        newCards.ForEach((x, i) => AddElement(i, x, new ComponentMetaData()));
    }

    public CardSO GetElement(int index)
    {

        if (index < cardsNow.Count && index >= 0)
        {
            return cardsNow[index];
        }

        Debug.LogError($"You are trying to get from the index {index} that doesn't exist");
        return null;
    }

    public void MoveElement(int index1, int index2, ComponentMetaData C_metaData)
    {
        if (index1 < cardsNow.Count && index1 >= 0 && index2 < cardsNow.Count && index2 >= 0)
        {
            var temp = cardsNow[index1];
            cardsNow[index1] = cardsNow[index2];
            cardsNow[index2] = temp;
            //CardSO[] cards = { cardsNow[index1], cardsNow[index2] };
            OnMoveElement?.Invoke(index1, index2, C_metaData);
            return;
        }

        Debug.LogError($"You are trying to move elments with index {index1} and {index2} which are out of bound");
    }
    public void MoveElement(CardSO card1, CardSO card2, ComponentMetaData C_metaData)
    {
        if (cardsNow.Contains(card1) && cardsNow.Contains(card2))
        {
            MoveElement(cardsNow.IndexOf(card1), cardsNow.IndexOf(card2), C_metaData);
        }
    }

    public void AddElement(int index, CardSO newCard, ComponentMetaData C_metaData)
    {
        if (index <= cardsNow.Count && index >= 0)
        {
            cardsNow.Insert(index, newCard);
            //CardSO[] cards = { cardsNow[index]};
            OnAddElement?.Invoke(index, newCard, C_metaData);
            return;
        }

        Debug.LogError($"You are trying to insert to the index {index} that doesn't exist");
    }

    public void RemoveElement(int index, ComponentMetaData C_metaData)
    {
        if (index < cardsNow.Count && index >= 0)
        {
            cardsNow.RemoveAt(index);
            // CardSO[] cards = { cardsNow[index] }; this thing you will have to do when you will connect OnRemoveElement with ActionSO
            // Pack the Transform into C_metaData
            OnRemoveElement?.Invoke(index, C_metaData);
            return;
        }
        //Debug.LogError($"You are trying to remove from the index {index} that doesn't exist");
    }
    public void RemoveElement(CardSO card, ComponentMetaData C_metaData)
    {
        if (cardsNow.Contains(card))
        {
            RemoveElement(cardsNow.IndexOf(card), C_metaData);
        }
    }

    public void RemoveAllElements(ComponentMetaData C_metaData)
    {
        cardsNow.Clear();
    }

    public List<CardSO> GetList()
    {
        return cardsNow;
    }

    public override string ToString()
    {
        return CardSO.ListToJson(GetList().ToArray());
    }

    public int GetCount()
    {
        return cardsNow.Count;
    }

    [Button("InformAboutChanges")]
    public bool Inform;

    public void InformAboutChanges()
    {
        var cardsJson = ToString();
        OnRemoveAllElements(new ComponentMetaData());
        CreateFromJson(cardsJson);
    }
}
