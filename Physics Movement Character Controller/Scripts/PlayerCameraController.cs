using System;
using System.Collections;
using System.Collections.Generic;
using ScottEwing.EventSystem;
using UnityEngine;

namespace ScottEwing.PhysicsPlayerController{
    public class PlayerCameraController : MonoBehaviour{
        [SerializeField] private GameObject _player;
        [SerializeField] private float _rotateSpeed = 50.0f;
        [SerializeField] private float _verticalRotateSpeed = 25.0f;
        private PlayerInputHandler _playerInputs;
        [SerializeField] [Range(0, 90)]private int _minAngleX = 40;
        [SerializeField] [Range(275, 360)]private int _maxAngleX = 320;
        private float _sensitivity = 1;

        void Awake() {
            _playerInputs = GetComponentInParent<PlayerInputHandler>();
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


        void LateUpdate() {
            // Horizontal Movement
            transform.position = _player.transform.position;
            //float horizontalMovement = _playerInputs.HorizontalCameraMovement;
            float horizontalMovement = _playerInputs.Inputs.look.x;
            gameObject.transform.rotation *= Quaternion.AngleAxis(_sensitivity * _rotateSpeed * horizontalMovement * Time.deltaTime, Vector3.up);

            //Vertical Movement
            //float verticalMovement = _playerInputs.VerticalCameraMovement;
            float verticalMovement = _playerInputs.Inputs.look.y;
            gameObject.transform.rotation *= Quaternion.AngleAxis(_sensitivity * _verticalRotateSpeed * verticalMovement * Time.deltaTime, Vector3.right);

            Vector3 angles = transform.localEulerAngles;
            angles.z = 0;

            float angle = transform.localEulerAngles.x;

            if (angle > 180 && angle < _maxAngleX) {
                angles.x = _maxAngleX;
            }
            else if (angle < 180 && angle > _minAngleX) {
                angles.x = _minAngleX;
            }

            transform.localEulerAngles = angles;
        }
        
        

    }
}
