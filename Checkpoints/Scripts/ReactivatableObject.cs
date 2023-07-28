using System.Collections;
using System.Collections.Generic;
using ScottEwing.Checkpoints;
using ScottEwing.EventSystem;
using UnityEngine;

namespace ScottEwing
{
    public class ReactivatableObject : BaseCheckpointListener
    {

        [Tooltip("If objects were enabled when checkpoint was reached the objects will be set (back) to enabled when checkpoint is reset and vice versa")]
        [SerializeField] protected GameObject _target;

        [Tooltip("If true the object will be destroyed if the checkpoint is reset before a checkpoint has been reached. Useful if " +
                 "this object has been instantiated in between checkpoints, and should be destroyed if checkpoint is reloaded " +
                 "before reaching the next checkpoint")]
        [SerializeField] private bool _destroyOnReloadIfCheckpointNotReached = true;
        private bool _checkpointReached;
        private bool _isEnabledOnCheckpointReached;
 
        protected override void Start() {
            base.Start();
            if (!_target) {
                _target = gameObject;
            }
            _isEnabledOnCheckpointReached = _target.activeSelf;
        }

#if SE_EVENTSYSTEM
        public override void OnCheckpointReached(CheckpointReachedEvent obj) {
            _checkpointReached = true;
            SaveEnabledStatus();
        }

        public override void OnCheckpointReload(CheckpointReloadEvent obj) {
            if (_destroyOnReloadIfCheckpointNotReached && !_checkpointReached) {
                Destroy(_target);
                return;
            }
            ReloadEnabledStatus();
        }


#endif

        public override void Save(CheckpointReachedData data) => SaveEnabledStatus();
        public override void Reload() => ReloadEnabledStatus();


        private void SaveEnabledStatus() {
            if (!_target)  return; 
            _isEnabledOnCheckpointReached = _target.activeSelf;
        }
        
        private void ReloadEnabledStatus() {
            if (!_target)  return; 
            _target.SetActive(_isEnabledOnCheckpointReached);
        }
    }
}
