using UnityEngine;

namespace ScottEwing.Checkpoints {
    public abstract class CheckpointSaveResetObject : MonoBehaviour {
        protected virtual void OnEnable() {
            CheckpointManager.OnCheckpointReachedEvent += OnCheckpointReached;
            CheckpointManager.OnResetToCheckpointEvent += OnCheckpointReset;
        }
        protected virtual void OnDisable() {
            CheckpointManager.OnCheckpointReachedEvent -= OnCheckpointReached;
            CheckpointManager.OnResetToCheckpointEvent -= OnCheckpointReset;
        }
        protected virtual void OnCheckpointReached() {
        }
        protected virtual void OnCheckpointReset() {
        }
    }
}
