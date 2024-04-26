using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( menuName = "Gadget-top SOs/CardVisualStyleSO")]
public class CardVisualStyleSO : ScriptableObject
{
    public List<SerializableKeyValuePair> visualStyles;
    //public Dictionary<CardSO, GameObject> visualStyles = new Dictionary<CardSO, GameObject>();

    public GameObject GetValue(CardSO card){
        foreach(SerializableKeyValuePair x in visualStyles){
            if(x._Key == card) return x._Value;
        }
        return null;
    }
}
