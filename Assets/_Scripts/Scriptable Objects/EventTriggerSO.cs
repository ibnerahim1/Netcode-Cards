using UnityEngine;

[CreateAssetMenu(fileName = "EventTrigger", menuName = "ScriptableVariables/EventTrigger", order = 1)]
public class EventTriggerSO : ScriptableObject
{
    public delegate void OnTriggerDelegate();
    public event OnTriggerDelegate OnTrigger;

    [Button(nameof(TriggerEvent))]
    public bool triggerEvent;

    public void TriggerEvent()
    {
        OnTrigger?.Invoke();
    }
}