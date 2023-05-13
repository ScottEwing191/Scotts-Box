using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ScottEwing.TriggersV2{
    [Serializable]
    public class InteractTriggerData : ITriggerData{
        [field: SerializeField] public InputActionProperty InputActionReference { get; set; }
    }
}