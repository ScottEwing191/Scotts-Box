using System;
using UnityEngine;
using UnityEngine.Events;

namespace ScottEwing.TriggersV2{
    [Serializable]
    public class ToggleInteractTriggerData : InteractTriggerData{
        [Tooltip("The Trigger has been toggled off")]
        [SerializeField] public UnityEvent _onTriggeredOff;
        [SerializeField] public bool _turnOnOnFirstEnter;
        [Tooltip("Turns off the trigger on trigger exit only if trigger is currently on")]
        [SerializeField] public bool _turnOffOnTriggerExit = true;
    }
}