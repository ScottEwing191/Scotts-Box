using System;
using UnityEngine;

namespace ScottEwing.TriggersV2{
    [Serializable]
    public struct TimedStayTriggerData : ITriggerData{
        [SerializeField] public float _durationRequiredForTrigger;
        [SerializeField] public bool _cancelOnTriggerExit;
    }
}