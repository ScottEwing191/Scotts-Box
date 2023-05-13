
using UnityEngine;
#if SE_EVENTSYSTEM
using ScottEwing.EventSystem;
#endif


namespace ScottEwing.Checkpoints{
    public interface ICheckpointListener{
#if SE_EVENTSYSTEM
        public bool UseCheckpointEvents { get; set; }
        public void OnEnable();
        public void OnDisable();
        public void OnCheckpointReached(CheckpointReachedEvent obj);
        public void OnCheckpointReload(CheckpointReloadEvent obj);
#endif
        public void Save(CheckpointReachedData data);
        public void Reload();
    }

    public struct CheckpointReachedData{
        public Vector3 respawnPosition;
        public Quaternion respawnRotation;

        public CheckpointReachedData(Vector3 respawnPosition, Quaternion respawnRotation) {
            this.respawnPosition = respawnPosition;
            this.respawnRotation = respawnRotation;
        }
    }
    
}