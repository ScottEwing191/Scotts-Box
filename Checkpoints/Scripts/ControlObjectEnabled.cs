using System.Collections.Generic;
using UnityEngine;

#if SE_EVENTSYSTEM
using ScottEwing.EventSystem;
#endif

//--This script will save whether an object is active or not when a checkpoint is reached and will return the object to that 
//--state when returning to a checkpoint

namespace ScottEwing.Checkpoints {
    public class ControlObjectEnabled : BaseCheckpointListener {

        [Tooltip("If objects were enabled when checkpoint was reached the objects will be set (back) to enabled when checkpoint is reset and vice versa")]
        [SerializeField] protected List<GameObject> _targets = new List<GameObject>();
        protected bool[] isEnabledOnCheckpointReached;
 
        protected override void Start() {
            base.Start();
            isEnabledOnCheckpointReached = new bool[_targets.Count];
            for (int i = 0; i < _targets.Count; i++) {
                isEnabledOnCheckpointReached[i] = _targets[i].activeSelf;
            }
        }

#if SE_EVENTSYSTEM
        public override void OnCheckpointReached(CheckpointReachedEvent obj) => SaveEnabledStatus();
        public override void OnCheckpointReload(CheckpointReloadEvent obj) => ReloadEnabledStatus();


#endif

        public override void Save(CheckpointReachedData data) => SaveEnabledStatus();
        public override void Reload() => ReloadEnabledStatus();


        private void SaveEnabledStatus() {
            if (isEnabledOnCheckpointReached == null) {
                return;
            }

            for (int i = 0; i < _targets.Count; i++) {
                isEnabledOnCheckpointReached[i] = _targets[i].activeSelf;
            }
        }
        
        private void ReloadEnabledStatus() {
            if (isEnabledOnCheckpointReached == null) return;
            for (var i = 0; i < _targets.Count; i++) {
                _targets[i].SetActive(isEnabledOnCheckpointReached[i]);
            }
        }
    } 
}
