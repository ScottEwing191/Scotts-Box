using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ScottEwing.TriggersV2{
    [Serializable]
    public class LookInteractTriggerData : LookTriggerData{
        [field: SerializeField] public InputActionProperty InputActionReference { get; set; }

    }
}