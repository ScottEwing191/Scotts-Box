using UnityEngine;


namespace ScottEwing.EventSystem
{
    public class CheckpointEvents
    {
        public static CheckpointReachedEvent checkpointReachedEvent = new CheckpointReachedEvent();
        public static CheckpointReloadEvent checkpointReloadEvent = new CheckpointReloadEvent();

    }
    
    public class CheckpointReachedEvent : GameEvent{
        public Vector3 position;
        public Quaternion rotation;
    }
    
    public class CheckpointReloadEvent : GameEvent{
        public bool checkpointAvailable;
        public Vector3 position;
        public Quaternion rotation;
    }
    
    public class ObjectRespawnedEvent : GameEvent{
        public GameObject respawnedObject;
        //public Collider[] childColliders;
    }
}

