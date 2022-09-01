using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScottEwing.PhysicsPlayerController{
    public class PlayerCameraController : MonoBehaviour{
        [SerializeField] private GameObject player;
        [SerializeField] private float rotateSpeed = 50.0f;
        [SerializeField] private float verticalRotateSpeed = 25.0f;
        PlayerInputHandler _playerInputs;

        void Awake() {
            _playerInputs = GetComponentInParent<PlayerInputHandler>();
        }

        void Update() {
            // Horizontal Movement
            transform.position = player.transform.position;
            float horizontalMovement = _playerInputs.HorizontalCameraMovement;
            gameObject.transform.rotation *= Quaternion.AngleAxis(rotateSpeed * horizontalMovement * Time.deltaTime, Vector3.up);

            //Vertical Movement
            float verticalMovement = _playerInputs.VerticalCameraMovement;

            gameObject.transform.rotation *= Quaternion.AngleAxis(verticalRotateSpeed * verticalMovement * Time.deltaTime, Vector3.right);

            Vector3 angles = transform.localEulerAngles;
            angles.z = 0;

            float angle = transform.localEulerAngles.x;

            if (angle > 180 && angle < 320) {
                angles.x = 320;
            }
            else if (angle < 180 && angle > 40) {
                angles.x = 40;
            }

            transform.localEulerAngles = angles;
        }
    }
}
