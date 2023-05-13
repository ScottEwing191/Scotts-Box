using System;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ScottEwing.Checkpoints{
    [Serializable]
    public class CheckpointReachedReloadTrigger{
        enum ReachedOrReload{ CheckpointReached, CheckpointReload }

        public enum GlobalOrIndividual{ Global, Individual }

        [Flags]
        public enum FindCheckpointListenersIn{
            None = 0,
            ColliderGameObject = 1,
            ColliderOrParent = 2,
            ColliderAndSiblings = 4,
        }
        
        [SerializeField] private ReachedOrReload _reachedOrReload = ReachedOrReload.CheckpointReached;
        
        [Tooltip("Global: The OnCheckpointReached event will be invoked and every respawnable objects listening to this event will save its position Single: Only the respawnable object which touches this trigger will have its position saved ")]
        [SerializeField] private GlobalOrIndividual _globalOrIndividual = GlobalOrIndividual.Global;

#if ODIN_INSPECTOR
        [ShowIf("_globalOrIndividual", GlobalOrIndividual.Individual)]
#endif
        [Tooltip("Look for the Respawnable Object Component on either the game object of the collider which touch the checkpoint, or on child or parent game objects of that collider. (Only relevant when using the 'Single' Checkpoint Type")]
        [SerializeField] private FindCheckpointListenersIn _findCheckpointListenersIn = FindCheckpointListenersIn.ColliderGameObject;

#if ODIN_INSPECTOR
        [ShowIf("_reachedOrReload", ReachedOrReload.CheckpointReached)]
#endif
        [SerializeField] private Transform _respawnTransform;

        private Vector3 _respawnPosition;
        private Quaternion _respawnRotation;

        public void Init(Transform transform) {
            if (_respawnTransform == null) {
                _respawnTransform = transform;
            }
            _respawnPosition = _respawnTransform.position;
            _respawnRotation = _respawnTransform.rotation;
        }

        public void Triggered(Collider other) {
            if (_reachedOrReload == ReachedOrReload.CheckpointReached) {
                CheckpointReachedOrReloaded(other, CheckpointReached);
            }else if (_reachedOrReload == ReachedOrReload.CheckpointReload) {
                CheckpointReachedOrReloaded(other, CheckpointReload);
            }
        }
        
        private void CheckpointReached(ICheckpointListener respawnableObject) {
            respawnableObject.Save(new CheckpointReachedData(_respawnPosition, _respawnRotation));
        }

        private void CheckpointReload(ICheckpointListener respawnableObject) {
            respawnableObject.Reload();
        }

        private void CheckpointReachedOrReloaded(Collider other, Action<ICheckpointListener> callback) {
            if (HandleGlobalCheckpointTypes()) return;

            if (GetCheckpointListeners(other.gameObject, out var respawnableObjects)) {
                foreach (var o in respawnableObjects) {
                    callback.Invoke(o);
                }
            }
        }

        private bool HandleGlobalCheckpointTypes() {
            if (_globalOrIndividual == GlobalOrIndividual.Individual) {
                return false;
            }

            if (_reachedOrReload == ReachedOrReload.CheckpointReached)
                CheckpointManager.Instance.CheckpointReached(_respawnPosition, _respawnRotation);
            else if (_reachedOrReload == ReachedOrReload.CheckpointReload) 
                CheckpointManager.Instance.ReloadCheckpoint();
            return true;
        }

        public bool GetCheckpointListeners(GameObject gameObject, out ICheckpointListener[] checkpointListeners) {
            checkpointListeners = null;
            ICheckpointListener tmp = null;

            if ((_findCheckpointListenersIn & FindCheckpointListenersIn.ColliderAndSiblings) != 0) {
                if (gameObject.transform.parent != null) {
                    checkpointListeners = gameObject.transform.parent.GetComponentsInChildren<ICheckpointListener>();
                }
            }
            else if ((_findCheckpointListenersIn & FindCheckpointListenersIn.ColliderOrParent) != 0) {
                checkpointListeners = new[] { gameObject.GetComponentInParent<ICheckpointListener>() };
            }

            else if ( (_findCheckpointListenersIn & FindCheckpointListenersIn.ColliderGameObject) != 0 && gameObject.TryGetComponent<ICheckpointListener>(out tmp)) {
                checkpointListeners = new[] { gameObject.GetComponent<ICheckpointListener>() };
            }

            if (checkpointListeners == null) {
                Debug.LogError("No Checkpoint Listeners Found");
            }
            return checkpointListeners != null;
        }
    }
}