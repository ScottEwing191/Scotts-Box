using System;
using System.Collections;
using System.Collections.Generic;
using ScottEwing.Triggers;
using UnityEngine;

namespace ScottEwing.Checkpoints{
    public class Checkpoint : TouchTrigger{
        [SerializeField] private Transform _respawnTransform;
        public Vector3 RespawnPosition { get; set; }
        public Quaternion RespawnRotation { get; set; }

        private void Awake() {
            RespawnPosition = _respawnTransform.position;
            RespawnRotation = _respawnTransform.rotation;
        }

        private void OnEnable() => _onTriggered.AddListener(delegate { CheckpointManager.Instance.CheckpointReached(this); });
        private void OnDisable() => _onTriggered.RemoveListener(delegate { CheckpointManager.Instance.CheckpointReached(this); });
        
    }
}
