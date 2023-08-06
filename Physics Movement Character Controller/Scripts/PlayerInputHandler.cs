using System;
using System.Collections;
using System.Collections.Generic;
using ScottEwing.EventSystem;
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
        private bool invertControllerYAxis;

        protected override void Start() {
            _actionMap["Jump"].performed += OnJump;
            _actionMap["Move"].performed += OnMove;
            _actionMap["Look"].performed += OnLook;
            _actionMap["Look"].canceled += context => Inputs.look = Vector2.zero;
            _actionMap["Brake"].performed += OnBrake;
            _actionMap["Brake"].canceled += OnBrake;

            if (PlayerPrefs.HasKey("InvertControllerYAxis")) {
                invertControllerYAxis = PlayerPrefs.GetInt("InvertControllerYAxis") == 1;
            }
            
            EventManager.AddListener<GameResumedEvent>(evt => invertControllerYAxis = evt.invertControllerYAxis);
         
            base.Start();
        }

        protected override void OnDestroy() {
            _actionMap["Jump"].performed -= OnJump;
            _actionMap["Move"].performed -= OnMove;
            _actionMap["Look"].performed -= OnLook;
            _actionMap["Look"].canceled -= context => Inputs.look = Vector2.zero;
            _actionMap["Brake"].performed -= OnBrake;
            _actionMap["Brake"].canceled -= OnBrake;
            
            EventManager.RemoveListener<GameResumedEvent>(evt => invertControllerYAxis = evt.invertControllerYAxis);

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

            //Invert Y Axis if using controller and setting is enabled
            if (_playerInput.currentControlScheme == "Gamepad" && invertControllerYAxis) {
                Inputs.look.y *= -1;
            }
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

        public override void DisableActionMap() {
            base.DisableActionMap();
            Inputs.look = Vector2.zero;
            Inputs.movement = Vector3.zero;
        }
        
        public override void EnableActionMap() {
            base.EnableActionMap();
        }
    }
}

