using System;
using System.Collections;
using System.Collections.Generic;
using ScottEwing.Checkpoints;
using UnityEngine;

namespace ScottEwing.Triggers{
    [AddComponentMenu("ScottEwing/Triggers/CheckpointTouchTrigger(deprecated)")]
    public class CheckpointTouchTrigger : TouchTrigger{
        [SerializeField] private CheckpointReachedReloadTrigger _checkpointReachedReload;
        [SerializeField] private float _checkpointDelay = 0.0f;

        private void Awake() {
            _checkpointReachedReload.Init(transform);
        }

        protected override void OnTriggerEnter(Collider other) {
            if (!IsColliderValid(other)) return;
            base.OnTriggerEnter(other); // this will call on triggered
            if (_checkpointDelay == 0.0f) {
                _checkpointReachedReload.Triggered(other);
            }
            else {
                StartCoroutine(CheckpointTriggeredDelayRoutine(other));
            }
        }

        private IEnumerator CheckpointTriggeredDelayRoutine(Collider other) {
            yield return new WaitForSeconds(_checkpointDelay);
            _checkpointReachedReload.Triggered(other);
        }
    }
}