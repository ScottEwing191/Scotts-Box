using System;
using UnityEngine;

namespace ScottEwing.TriggersV2{
    [Serializable]
    public struct CounterTriggerData : ITriggerData{
        [SerializeField] public int _requiredCount;
        [SerializeField] public bool _decreaseCountOnExit;
    }
}