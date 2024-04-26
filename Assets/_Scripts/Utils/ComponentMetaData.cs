using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentMetaData
{
    #region static

    private static ulong ID; // Client ID that will be set all instances

    public static void SetId(ulong id) { ID = id; }
    public static ComponentMetaData GetC_metaDataAction(int i_ComponentID) { return new ComponentMetaData(ID, i_ComponentID, TypeOfUpdate.ActionUpdate); }
    public static ComponentMetaData GetC_metaDataBrain(int i_ComponentID) { return new ComponentMetaData(ID, i_ComponentID, TypeOfUpdate.BrainUpdate); }
    public static ComponentMetaData GetC_metaDataNetwork(int i_ComponentID) { return new ComponentMetaData(ID, i_ComponentID, TypeOfUpdate.NetworkUpdate); }
    public static ComponentMetaData GetC_metaDataVisualization(int i_ComponentID) { return new ComponentMetaData(ID, i_ComponentID, TypeOfUpdate.VisualizationUpdate); }

    #endregion

    public TypeOfUpdate typeOfUpdate;
    public ulong ClientID;
    public int ComponentID;

    public ComponentMetaData()
    {
        typeOfUpdate = TypeOfUpdate.Initialisation;
    }

    public ComponentMetaData(ulong _ClientID, int _ComponentID, TypeOfUpdate _typeOfUpdate){
        ClientID = _ClientID;
        typeOfUpdate = _typeOfUpdate;
        ComponentID = _ComponentID;
    }

    public enum TypeOfUpdate
    {
        ActionUpdate,
        BrainUpdate,
        VisualizationUpdate,
        NetworkUpdate,
        Initialisation
    }
}
