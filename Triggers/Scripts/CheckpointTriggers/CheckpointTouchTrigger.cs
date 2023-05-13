using System;
using System.Collections;
using System.Collections.Generic;
using ScottEwing.Checkpoints;
using UnityEngine;

namespace ScottEwing.Triggers{
    public class CheckpointTouchTrigger : TouchTrigger{
        [SerializeField] private CheckpointReachedReloadTrigger _checkpointReachedReload;

        private void Awake() {
            _checkpointReachedReload.Init(transform);
        }

        protected override void OnTriggerEnter(Collider other) {
            if (!IsColliderValid(other)) return;
            base.OnTriggerEnter(other);             // this will call on triggered
            _checkpointReachedReload.Triggered(other);
        }
    }
}
