using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GlobalState", menuName = "ScriptableVariables/GlobalState", order = 1)]
public class GlobalStateSO : ScriptableObject
{
    #region Static
    public static string pathToGlobalStates = "ScriptableVariables/GlobalStates";

    public static void MoveFromStateToState(GlobalStateSO from, GlobalStateSO to)
    {
        from.ExitState(new ComponentMetaData());
        to.EnterState(new ComponentMetaData());
    }

#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad()
    {
        foreach (GlobalStateSO x in UtilsFiles.GetAtPath<GlobalStateSO>("Resources/" + pathToGlobalStates))
        {
            x.Initialize();
        };
    }

#endif
    #endregion

    public delegate void OnEnterStateDelegate(ComponentMetaData C_metaData);
    public event OnEnterStateDelegate OnEnterState;

    public delegate void OnExitStateDelegate(ComponentMetaData C_metaData);
    public event OnExitStateDelegate OnExitState;

    [SerializeField]
    private bool isActiveOnStart;

    [SerializeField]
    private bool isActiveNow;

    public bool IsActiveState() { return isActiveNow; }

    public void Initialize()
    {
        isActiveNow = isActiveOnStart;
    }


    [Button(nameof(EnterState))]
    public bool enterState;

    public void EnterState(ComponentMetaData C_metaData)
    {
        if (isActiveNow == false)
        {
            isActiveNow = true;
            OnEnterState?.Invoke(C_metaData);
        }
    }

    [Button(nameof(ExitState))]
    public bool exitState;

    public void ExitState(ComponentMetaData C_metaData)
    {
        if (isActiveNow == true)
        {
            isActiveNow = false;
            OnExitState?.Invoke(C_metaData);
        }
    }

    [Button(nameof(FlipState))]
    public bool flipState;
    public void FlipState()
    {
        if (isActiveNow) { ExitState(new ComponentMetaData()); } else EnterState(new ComponentMetaData());
    }

    public static GlobalStateSO GetByName(string GlobalStateUniqueName)
    {
        return Resources.Load<GlobalStateSO>($"{pathToGlobalStates}/{GlobalStateUniqueName}");
    }

}