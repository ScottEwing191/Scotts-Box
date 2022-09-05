using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScottEwing.Checkpoints{
    public class MovableObject : MonoBehaviour{
        [Tooltip("True: On checkpoint reload, velocity reset to value as of when checkpoint was reached. False: On checkpoint reload, velocity reset to 0. ")]
        [SerializeField] private bool _useVelocity;

        private Vector3 _respawnPosition;
        private Quaternion _respawnRotation;
        private Vector3 _respawnVelocity;
        private Vector3 _respawnAngularVelocity;
        private Rigidbody _rb;

        private void Awake() {
            _rb = GetComponent<Rigidbody>();
            var t = transform;
            _respawnPosition = t.position;
            _respawnRotation = t.rotation;
            if (_rb != null) {
                _respawnVelocity = _rb.velocity;
                _respawnAngularVelocity = _rb.angularVelocity;
            }
            else if (_useVelocity){
                Debug.LogError("Movable Object Has No Rigidbody", this);
            }
        }

        private void OnEnable() {
            CheckpointManager.OnCheckpointReachedEvent += SaveLocation;
            CheckpointManager.OnResetToCheckpointEvent += Respawn;
        }

        private void OnDisable() {
            CheckpointManager.OnCheckpointReachedEvent -= SaveLocation;
            CheckpointManager.OnResetToCheckpointEvent -= Respawn;
        }

        private void SaveLocation() {
            var t = transform;
            _respawnPosition = t.position;
            _respawnRotation = t.rotation;
            if (_rb != null) {
                _respawnVelocity = _rb.velocity;
                _respawnAngularVelocity = _rb.angularVelocity;
            }
        }

        private void Respawn() {
            var t = transform;
            t.position = _respawnPosition;
            t.rotation = _respawnRotation;
            if (_rb == null) return;

            if (_useVelocity) {
                _rb.velocity = _respawnVelocity;
                _rb.angularVelocity = _respawnAngularVelocity;
            }else {
                _rb.AddForce(-_rb.velocity, ForceMode.VelocityChange);
                _rb.AddTorque(-_rb.angularVelocity, ForceMode.VelocityChange);
            }
        }
    }
}