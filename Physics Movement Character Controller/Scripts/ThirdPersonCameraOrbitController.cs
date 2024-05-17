using System;
using System.Collections;
using System.Collections.Generic;
using ScottEwing.EventSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ScottEwing.PhysicsPlayerController{
    public class ThirdPersonCameraOrbitController : MonoBehaviour{
        [SerializeField] private GameObject _player;
        [SerializeField] private GameObject _pitchGameObject;
        [SerializeField] private float _yawRotateSpeed = 14f;
        [SerializeField] private float _pitchRotateSpeed = 14f;

        [MinMaxSlider(-90, 90, true)]
        [SerializeField] private Vector2 _pitchClamp = new Vector2(-90, 90);

        [SerializeField] private bool _rotateBody = true;
        [SerializeField] private bool _lateFixedUpdate = false;
        private PlayerInputHandler _playerInputs;
        private float _sensitivity = 1;
        private float _currentPitchAngle;
        private float _currentYawAngle;
        
        private Quaternion _startRotation;
        

        [SerializeField] GameObject body;

        void Awake() {
            _playerInputs = GetComponentInParent<PlayerInputHandler>();
            _startRotation = transform.rotation;
        }

        private void Start() {
            if (PlayerPrefs.HasKey("CameraSensitivity")) {
                _sensitivity = PlayerPrefs.GetFloat("CameraSensitivity");
            }
#if SE_EVENTSYSTEM
            EventManager.AddListener<GameResumedEvent>((evt => _sensitivity = evt.cameraSensitivity));

#endif
        }

        private void OnDestroy() {
#if SE_EVENTSYSTEM
            EventManager.RemoveListener<GameResumedEvent>((evt => _sensitivity = evt.cameraSensitivity));
#endif
        }

        private RigidbodyConstraints save;
        private bool on = false;

        private void Update() {
            if (UnityEngine.Input.GetKeyDown(KeyCode.L)) {
                ToggleSpin();
            }
        }

        void LateUpdate() {
            
            //--Yaw Rotation
            //var yawAngleDelta = _sensitivity * _yawRotateSpeed * _playerInputs.Inputs.look.x * Time.deltaTime;
            float yawAngleDelta;
            if (spin) {
                yawAngleDelta = _sensitivity * _yawRotateSpeed * 7 * Time.deltaTime;
            }
            else {
                yawAngleDelta = _sensitivity * _yawRotateSpeed * _playerInputs.Inputs.look.x * Time.deltaTime;
            }

            _currentYawAngle += yawAngleDelta;
            transform.localRotation = /*_startRotation **/ Quaternion.AngleAxis(_currentYawAngle, Vector3.up);
            
            //--Rotate Body
            if (_rotateBody) {
                body.transform.localRotation = /*_startRotation **/ Quaternion.AngleAxis(_currentYawAngle, Vector3.up);
            }
            
            //--Pitch Rotation
            var pitchAngleDelta = _sensitivity * _pitchRotateSpeed * _playerInputs.Inputs.look.y * Time.deltaTime;
            _currentPitchAngle = Mathf.Clamp(_currentPitchAngle + pitchAngleDelta, _pitchClamp.x, _pitchClamp.y);
            _pitchGameObject.transform.localRotation = Quaternion.AngleAxis(_currentPitchAngle, Vector3.right);
            
            //--Follow Player
            transform.position = _player.transform.position;
        }

        /*private void FixedUpdate() {
            //--Rotate Body
            if (_rotateBody) {
                body.transform.localRotation = Quaternion.AngleAxis(_currentYawAngle, Vector3.up);
            }
        }*/

        [ButtonGroup]
        public void Reset(Quaternion respawnTransform) {
            _currentPitchAngle = 0;
            _currentYawAngle = respawnTransform.eulerAngles.y - _startRotation.eulerAngles.y;
        }
        
        private bool spin = false;

        [Button]
        void ToggleSpin() {
            spin = !spin;
        }
    }
}