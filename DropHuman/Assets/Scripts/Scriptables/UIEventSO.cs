using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "UIEventSO", menuName = "Scriptable Objects/ UIEventSO")]
public class UIEventSO : ScriptableObject
{
    [HideInInspector] public UnityEvent  
                                        EnableWinUIEvent, EnableLoseUIEvent, 
                                        DisableWinUIEvent, DisableLoseUIEvent,
                                        EnableReviveUIEvent, DisableReviveUIEvent;
}
