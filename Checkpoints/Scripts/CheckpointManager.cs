#if SE_EVENTSYSTEM
using ScottEwing.EventSystem;
#endif
using UnityEngine;


namespace ScottEwing.Checkpoints{
    public class CheckpointManager : NewMonoSingleton<CheckpointManager>{
        
        public void CheckpointReached(Vector3 respawnPosition, Quaternion respawnRotation) {
#if SE_EVENTSYSTEM
            var evt = new CheckpointReachedEvent {
                position = respawnPosition,
                rotation = respawnRotation
            };
            EventManager.Broadcast(evt);
#else
            Debug.LogError("Scott Ewing's EventSystem must be installed to Checkpoint Reached / Reload events");
#endif
        }
        
        public void ReloadCheckpoint() {
#if SE_EVENTSYSTEM
            EventManager.Broadcast(new CheckpointReloadEvent());
#else
            Debug.LogError("Scott Ewing's EventSystem must be installed to Checkpoint Reached / Reload events");
#endif
        }
    }
}