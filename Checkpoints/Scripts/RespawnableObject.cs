using ScottEwing.Triggers;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
#if SE_EVENTSYSTEM
using Codice.Client.BaseCommands;
using ScottEwing.EventSystem;
#endif

namespace ScottEwing.Checkpoints{
    /// <summary>
    /// This class can be used to save the position and rotation of an object and then return the object to that location at a later time. The object location can be saved when a checkpoint is reach
    /// and respawned when a checkpoint is reloaded. Additionally the object can ignore the checkpoint events and the location can be saved  / respawned manually using the SavePosiiton(), Respawn()
    /// methods
    /// </summary>
    public class RespawnableObject : BaseCheckpointListener{
        protected enum UpdateRespawnTransformType{
            UseCurrentTransform,
            UseCheckpointRespawnTransform
        }

        [Tooltip("True: The respawn position and rotation will be updated each time a checkpoint reached event is invoked. False: The default respawn transform will be used indefinitely")]
#if ODIN_INSPECTOR
        [ShowIf("UseCheckpointEvents", true)]
#endif
        [SerializeField] private bool _updateRespawnTransformOnCheckpointReached = true;

#if ODIN_INSPECTOR
        [ShowIf("UseCheckpointEvents", true)]
#endif
        [Tooltip("If true the object will respawn when the checkpoint reload event occurs, if false this object will ignore the checkpoint reload event")]
        [SerializeField] private bool _respawnOnCheckpointReload = true;

        [Tooltip("True: When respawning, velocity is set to value as of when the checkpoint was reached. False: When respawning velocity is set 0.")]
        [SerializeField] private bool _useVelocity;

        [SerializeField] private Transform _defaultRespawnTransform;

        [Tooltip("UseCurrentTransform: When checkpoint is reached, use the current transform of the object as the respawn position / rotation \n " +
                 "UseCheckpointRespawnTransform: When checkpoint is reached, use the respawn transform provided by the checkpoint as the respawn position / rotation")]
        [SerializeField] protected UpdateRespawnTransformType _updateRespawnTransformType = UpdateRespawnTransformType.UseCurrentTransform;


        private Vector3 _respawnPosition;
        private Quaternion _respawnRotation;
        private Vector3 _respawnVelocity;
        private Vector3 _respawnAngularVelocity;
        private Rigidbody _rb;

        private void Awake() {
            _rb = GetComponent<Rigidbody>();
            var t = _defaultRespawnTransform ? _defaultRespawnTransform : transform;
            _respawnPosition = t.position;
            _respawnRotation = t.rotation;
            if (_rb != null) {
                _respawnVelocity = _rb.velocity;
                _respawnAngularVelocity = _rb.angularVelocity;
            }
            else if (_useVelocity) {
                Debug.LogError("Use velocity is true but GameObject has no Rigidbody", this);
            }
        }

#if SE_EVENTSYSTEM
        public override void OnCheckpointReached(CheckpointReachedEvent obj) {
            if (!_updateRespawnTransformOnCheckpointReached)
                return;
            SaveLocation(obj.position, obj.rotation);
        }

        public override void OnCheckpointReload(CheckpointReloadEvent obj) {
            if (!_respawnOnCheckpointReload) return;
            Respawn();
        }
#endif

        public override void Save(CheckpointReachedData data) => SaveLocation(data.respawnPosition, data.respawnRotation);
        public override void Reload() => Respawn();

        
        /// <summary>
        /// Sets the respawn position / rotation to the values passed in. Also saves the linear and angular velocity if a rigidbody exists. 
        /// </summary>
        public void SaveLocation(Vector3 position, Quaternion rotation) {
            if (_updateRespawnTransformType == UpdateRespawnTransformType.UseCurrentTransform) {
                _respawnPosition = transform.position;
                _respawnRotation = transform.rotation;    
            }
            else {
                _respawnPosition = position;
                _respawnRotation = rotation;
            }

            if (_rb != null) {
                _respawnVelocity = _rb.velocity;
                _respawnAngularVelocity = _rb.angularVelocity;
            }
        }
        
        
        /// <summary>
        /// Depending on the value of <para>_updateRespawnTransformType</para>, either use the collider transfrom as the save position / rotation. Or, use the current transform as the save
        /// position / rotation instead.
        /// </summary>
        public void SaveLocation(Collider checkpoint) {
            if (_updateRespawnTransformType == UpdateRespawnTransformType.UseCurrentTransform) {
                SaveLocation(transform.position, transform.rotation);
            }
            else {
                var t = checkpoint.gameObject.transform;
                SaveLocation(t.position, t.rotation);
            }
        }

        /// <summary>
        /// Respawn the object at it's saved position and rotation. Will either return the velocity to its saved value or will set velocity to zero.
        /// </summary>
        public virtual void Respawn() {
            var t = transform;
            t.position = _respawnPosition;
            t.rotation = _respawnRotation;
#if SE_EVENTSYSTEM
            var evt = new ObjectRespawnedEvent {
                respawnedObject = gameObject,
            };
            EventManager.Broadcast(evt);
#endif
            if (_rb == null) return;
            if (_useVelocity) {
                _rb.velocity = _respawnVelocity;
                _rb.angularVelocity = _respawnAngularVelocity;
            }
            else {
                _rb.velocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }
        }
    }
}