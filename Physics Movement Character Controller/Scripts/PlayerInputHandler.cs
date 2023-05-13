using System;
using System.Collections;
using System.Collections.Generic;
using ScottEwing.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ScottEwing.PhysicsPlayerController{

    public class PlayerInputs{
        public Vector3 movement;
        public Vector2 look;

    }
    public class PlayerInputHandler : BaseInputHandler{
        public PlayerInputs Inputs { get; set; } = new PlayerInputs();
        public Action jump;
        public Action brakeOn;
        public Action brakeOff;

        private bool isBrakeOn = false;     // for brake toggle (not being used) 

        protected override void Start() {
            _actionMap["Jump"].performed += OnJump;
            _actionMap["Move"].performed += OnMove;
            _actionMap["Look"].performed += OnLook;
            _actionMap["Look"].canceled += context => Inputs.look = Vector2.zero;
            _actionMap["Brake"].performed += OnBrake;
            _actionMap["Brake"].canceled += OnBrake;
            base.Start();
        }

        protected override void OnDestroy() {
            _actionMap["Jump"].performed -= OnJump;
            _actionMap["Move"].performed -= OnMove;
            _actionMap["Look"].performed -= OnLook;
            _actionMap["Look"].canceled -= context => Inputs.look = Vector2.zero;
            _actionMap["Brake"].performed -= OnBrake;
            _actionMap["Brake"].canceled -= OnBrake;
            base.OnDestroy();


        }

        

        private void OnJump(InputAction.CallbackContext obj) => jump?.Invoke();
        private void OnMove(InputAction.CallbackContext obj) {
            var tmp = obj.ReadValue<Vector2>();
            Inputs.movement.x = tmp.x;
            Inputs.movement.z = tmp.y;
        }

        private void OnLook(InputAction.CallbackContext obj) {
            Inputs.look = obj.ReadValue<Vector2>();
            Inputs.look.y *= -1;
        }
        
        private void OnBrake(InputAction.CallbackContext obj) {
            //Toggle
            /*if (obj.phase == InputActionPhase.Canceled) {
                return;
            }
            isBrakeOn = !isBrakeOn;
            if (isBrakeOn) {
                brakeOn?.Invoke();
            }
            else{
                brakeOff?.Invoke();
            } */        
            //--Hold
            switch (obj.phase) {
                case InputActionPhase.Performed:
                    brakeOn?.Invoke();
                    break;
                case InputActionPhase.Canceled:
                    brakeOff?.Invoke();
                    break;
            }
        }


        /*public Vector3 MovementVector { get; private set; } = Vector3.zero;
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
    }#1#

        private void OnLook(InputValue input) {
            Vector2 v = input.Get<Vector2>();
            HorizontalCameraMovement = v.x;
            VerticalCameraMovement = -v.y; // inverted is better
        }

        private void OnJump(InputValue input) {
            _physicsMovementPlayerController.DoJump();
        }*/
    }
}

