using System;
using ScottEwing.Checkpoints;
using UnityEngine;

namespace ScottEwing.TriggersV2{
    [Serializable]
    public struct CheckpointTouchTriggerData : ITriggerData{
        [SerializeField] public CheckpointReachedReloadTrigger _checkpointReachedReload;

    }
}