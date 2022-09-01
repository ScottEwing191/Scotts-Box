using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ScottEwing.PhysicsPlayerController{
    public class PlayerInputHandler : MonoBehaviour{
        public Vector3 MovementVector { get; private set; } = Vector3.zero;
        public float HorizontalCameraMovement { get; private set; } = 0.0f;
        public float VerticalCameraMovement { get; private set; } = 0.0f;
        private PhysicsMovementPlayerController _physicsMovementPlayerController;

        private void Awake() {
            _physicsMovementPlayerController = GetComponentInChildren<PhysicsMovementPlayerController>();
        }

        // === On Action Methods Start ===
        private void OnMove(InputValue input) {
            Vector2 v = input.Get<Vector2>();
            MovementVector = new Vector3(v.x, 0, v.y);
        }
        /*public void OnMove(InputAction.CallbackContext input) {
        Vector2 v = input.action.ReadValue<Vector2>();
        MovementVector = new Vector3(v.x, 0, v.y);
        if (input.started) {
            print("started");
        }
        if (input.performed) {
            print("performed");
        }
        if (input.canceled) {
            print("canceled");
        }
    }*/

        private void OnLook(InputValue input) {
            Vector2 v = input.Get<Vector2>();
            HorizontalCameraMovement = v.x;
            VerticalCameraMovement = -v.y; // inverted is better
        }

        private void OnJump(InputValue input) {
            _physicsMovementPlayerController.DoJump();
        }
    }
}
