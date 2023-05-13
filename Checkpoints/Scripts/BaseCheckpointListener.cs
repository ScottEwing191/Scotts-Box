#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System;
using UnityEngine;
#if SE_EVENTSYSTEM
using ScottEwing.EventSystem;
#endif

namespace ScottEwing.Checkpoints{
    public abstract class BaseCheckpointListener : MonoBehaviour, ICheckpointListener{

        protected enum CheckpointEventSubscribeType{
            EnableAndDisable, 
            StartAndDestroy
        }
        [field:SerializeField] public bool UseCheckpointEvents { get; set; } = true;

#if ODIN_INSPECTOR
        [ShowIf("UseCheckpointEvents")]
#endif
        [Tooltip("Controls whether checkpoint events will be subscribed / unsubscribed to in the Start / OnDestroy methods or in the OnEnable / OnDisable methods")]
        [SerializeField] private CheckpointEventSubscribeType _eventSubscribeType = CheckpointEventSubscribeType.EnableAndDisable;
#if SE_EVENTSYSTEM


        public virtual void OnEnable() {
            if (!UseCheckpointEvents || _eventSubscribeType != CheckpointEventSubscribeType.EnableAndDisable) return;
            EventManager.AddListener<CheckpointReachedEvent>(obj => OnCheckpointReached(obj));
            EventManager.AddListener<CheckpointReloadEvent>(obj => OnCheckpointReload(obj));
        }

        public virtual void OnDisable() {
            if (!UseCheckpointEvents || _eventSubscribeType != CheckpointEventSubscribeType.EnableAndDisable) return;
            EventManager.RemoveListener<CheckpointReachedEvent>(obj => OnCheckpointReached(obj));
            EventManager.RemoveListener<CheckpointReloadEvent>(obj => OnCheckpointReload(obj));
        }
        
        protected virtual void Start() {
            if (!UseCheckpointEvents|| _eventSubscribeType != CheckpointEventSubscribeType.StartAndDestroy) return;
            EventManager.AddListener<CheckpointReachedEvent>(obj => OnCheckpointReached(obj));
            EventManager.AddListener<CheckpointReloadEvent>(obj => OnCheckpointReload(obj));
        }

        protected virtual void OnDestroy() {
            if (!UseCheckpointEvents|| _eventSubscribeType != CheckpointEventSubscribeType.StartAndDestroy) return;
            EventManager.RemoveListener<CheckpointReachedEvent>(obj => OnCheckpointReached(obj));
            EventManager.RemoveListener<CheckpointReloadEvent>(obj => OnCheckpointReload(obj));
        }

        public virtual void OnCheckpointReached(CheckpointReachedEvent obj) {
        }

        public virtual void OnCheckpointReload(CheckpointReloadEvent obj) {
        }
#else
        protected virtual void Start() {
            
        }
#endif

        public virtual void Save(CheckpointReachedData data) {
        } 
        
        public virtual void Reload() {
        } 

        


    }
}