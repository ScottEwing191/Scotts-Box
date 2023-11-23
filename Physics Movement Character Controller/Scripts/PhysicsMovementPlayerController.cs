﻿using System;
using System.Collections;
using ScottEwing.ExtensionMethods;
using UnityEngine;

namespace ScottEwing.PhysicsPlayerController{
    public class PhysicsMovementPlayerController : MonoBehaviour{
        //--Serialized Fields
        [SerializeField] private float speed = 150;

        [Tooltip("If true, this script cannot increase the speed to above the minimum value while grounded, but other things (e.g. explosions) can increase " +
                 "the speed to above the min velocity while grounded. If false nothing can increase speed above max while grounded")]
        [SerializeField] private bool _onlyLimitControllerVelocity = false;

        [SerializeField] private float maxVelocity = 9.0f;
        [SerializeField] private float jumpHeight = 2;

        [Tooltip("Controls the strength the players movement input has on the direction of the players jump. The angle of the ground also affects the direction.")] [Range(0, 1)] [SerializeField]
        private float jumpInputDirectionScale = 0.1f;

        [SerializeField] private float inAirDrag = 0.5f;
        [SerializeField] private float inAirSpeed = 75f;

        [Tooltip("The percentage of the ball radius bellow the centre of the ball to use as the groundCheckOffset")] [SerializeField]
        private float groundCheckOffsetPercentage = 0.457f;

        [SerializeField] private bool hasAirControl = true;

        [Tooltip("Will be grounded if on one of these layers")]
        [SerializeField] private LayerMask _jumpLayers;

        [Tooltip("A transform whose forward vector is always parallel to the ground")] [SerializeField]
        private Transform parallelToGroundTransform;

        [Tooltip("affects how fast player can roll down slopes i think.")] [SerializeField]
        private float maxAngularVelocity = 100;

        [Tooltip("this is the velocity the ball will be able to get up to if jumping from a stand still")] [SerializeField]
        private float defaultAirVelocityMagnitude = 3f;

        [Header("Brake")]
        [SerializeField] private bool _useBrake = true;

        [SerializeField] public bool _toggleBrake = false;

        [Tooltip("EXPERIMENTAL: If true the brake will be applied automatically when the player is not moving. If false the brake will only be applied when the player presses the brake button")]
        [SerializeField] public bool _autoBrake = false;
        [SerializeField] private float _autoBrakeStrength = 10.0f;
        [SerializeField] private bool _autoBrakeOn;
        

        [SerializeField] private float _brakeStrength = 50;
        [SerializeField] private PhysicMaterial _brakePhysicsMaterial;

        [SerializeField] private float _defaultBrakeStrength = 1;
        [SerializeField] private PhysicMaterial _defaultBrakePhysicsMaterial;
        [SerializeField] private float _brakeApplyTime = 0.5f;

        [SerializeField] private bool isBrakeOn = false;
        private Coroutine _applyBrakesRoutine;

        public bool UseResponsiveMovement = true;
        //--Auto Properties
        [SerializeField] private float _groundedBufferSeconds = 0.075f;
        private Coroutine _groundedFalseBufferRoutine;
        [field: SerializeField] public bool IsGrounded { get; private set; } = true;
        private bool _jumped;
        private Rigidbody PlayerRigidbody { get; set; }

        //--Private 
        private float _defaultDrag = 0.1f;
        private bool _isStillInTheAir; // True if the ball is in the air and was also in the air in the previous frame. (Except first frame in air)
        private bool _hasGroundCheckBeenDoneThisFrame; // keeps track of whether ground check has already been done
        private float _groundCheckOffset = 0.4f; // If the the collision point with the ground is bellow this distance relative to the centre of the ball then the ball with be grounded
        private Vector3 _jumpStartVelocity; // When the ball jumps or is in the air the velocity will be clamped to this amount. 
        private Collider _playerCollider; // the collider attached to the player used to check if player is grounded
        private PlayerInputHandler _playerInputHandler;

        [SerializeField] AnimationCurve _accelerationCurve;
        private float _accelerationModifier = 1.0f;
        [SerializeField] AnimationCurve _inputDirectionCurve;
        private float _inputDirectionModifier = 1.0f;

        private void Awake() {
            
        }

        void Start() {
            PlayerRigidbody = GetComponent<Rigidbody>();
            _playerCollider = GetComponent<Collider>();
            _defaultDrag = PlayerRigidbody.drag;
            _playerInputHandler = GetComponentInParent<PlayerInputHandler>();
            _playerInputHandler.jump += DoJump;

            _playerInputHandler.brakeOn += BreakOn;
            _playerInputHandler.brakeOff += OnBrakeOff;
            
            _defaultBrakeStrength = PlayerRigidbody.angularDrag;
            _defaultBrakePhysicsMaterial = _playerCollider.material;
            PlayerRigidbody.maxAngularVelocity = maxAngularVelocity;

            
        }

        private void OnDestroy() {
            _playerInputHandler.jump -= DoJump;
            _playerInputHandler.brakeOn -= BreakOn;
            _playerInputHandler.brakeOff -= OnBrakeOff;
        }

        void FixedUpdate() {
            if (IsGrounded) {
                // Regular movement
                PlayerRigidbody.drag = _defaultDrag;
                Movement();
            }
            else {
                // in-air Movement
                PlayerRigidbody.drag = inAirDrag;
                if (!_isStillInTheAir) {
                    // if this is the first frame off the ground set the velocity of the ball at this point
                    _isStillInTheAir = true;
                    _jumpStartVelocity = PlayerRigidbody.velocity;
                }

                InAirMovement(_jumpStartVelocity);
            }
        }

        private void Update() {
            _hasGroundCheckBeenDoneThisFrame = false;
        }

        public void DoJump() {
            Vector3 jumpDirection = GetMovementVectorAdjustedForCamera() * jumpInputDirectionScale;
            //jumpDirection += parallelToGroundTransform.TransformDirection(Vector3.up); // jump in direction of slope
            jumpDirection += Vector3.up; // simple vertical jump
            if (IsGrounded) {
                PlayerRigidbody.AddForce(jumpDirection.normalized * jumpHeight, ForceMode.Impulse);
                _jumped = true;
                IsGrounded = false;
            }
        }

        void Movement() {
            //PlayerRigidbody.maxAngularVelocity = maxAngularVelocity;
            if (!IsGrounded && !hasAirControl) return;
            
            // if not in the air (has Air Control is probably redundant here as should get here while ball is in air)
            //--Version A: This will result in the player getting a force that goes up any slope it is on (i.e will have a Y component to force)
            //Vector3 movementVector = parallelToGroundTransform.TransformDirection(movementVector);

            Vector3 movementVector = GetMovementVectorAdjustedForCamera();
                
            //-- Set Responsive Movement Modifiers
            SetResponsiveMovementModifiers(movementVector);
            
            
            //--Version B: This will result in the player only getting a force on the XZ axis. 
            Vector3 force = movementVector * (speed * Time.deltaTime) * _accelerationModifier * _inputDirectionModifier;
            if (!_onlyLimitControllerVelocity) {
                PlayerRigidbody.AddForce(force);
                PlayerRigidbody.velocity = Vector3.ClampMagnitude(PlayerRigidbody.velocity, maxVelocity);
            }
            else if (PlayerRigidbody.velocity.magnitude < maxVelocity) {
                PlayerRigidbody.AddForce(force);
                //ClampVelocityDueToInputMagnitude();
                if (_autoBrake && !isBrakeOn) {
                    if (movementVector.magnitude == 0) {
                        if (!isBrakeOn) {
                            AutoBreakOn();
                        }
                    }
                    else 
                        AutoBrakeOff();
                }
            }
        }
        
        void SetResponsiveMovementModifiers(Vector3 movementVector) {
            if (UseResponsiveMovement) {
                SetAccelerationModifier(movementVector);
                SetInputDirectionModifier(movementVector);
            }
            else {
                _accelerationModifier = 1.0f;
                _inputDirectionModifier = 1.0f;
            }
        }

        private float prevSqrVelocityMagnitude;
        [SerializeField] private bool acceleration = false;
        private Vector3 prevMovementVector;
        /*private void SetAccelerationModifier(Vector3 movementVector) {
            
            if (prevSqrVelocityMagnitude <= PlayerRigidbody.velocity.sqrMagnitude) {
                acceleration = true;
                _accelerationModifier = _accelerationCurve.Evaluate(PlayerRigidbody.velocity.magnitude);
            }
            else {
                acceleration = false;
            }
            prevSqrVelocityMagnitude = PlayerRigidbody.velocity.sqrMagnitude;
        }*/
        
        [SerializeField] float accelerateTime = 0.2f;
        Coroutine _accelerationRoutine;
        private void SetAccelerationModifier(Vector3 movementVector) {
            
            if (movementVector.magnitude == 0) {
                prevMovementVector = Vector3.zero;
                return;
            }
            if (prevMovementVector.magnitude > 0 ) {
                prevMovementVector = movementVector;
                return;
            }
            prevMovementVector = movementVector;

            if (_accelerationRoutine != null) {
                //return;
                StopCoroutine(_accelerationRoutine);
                _accelerationRoutine = null;
            }
            _accelerationRoutine = StartCoroutine(AccelerateRoutine());

            IEnumerator AccelerateRoutine() {
                var time = 0.0f;
                
                while (time < accelerateTime) {
                    time += Time.fixedDeltaTime;
                    _accelerationModifier = _accelerationCurve.Evaluate(PlayerRigidbody.velocity.magnitude);
                    print("Accelerate");
                    yield return new WaitForFixedUpdate();
                }
                _accelerationModifier = _accelerationCurve.Evaluate(PlayerRigidbody.velocity.magnitude);
                _accelerationRoutine = null;
            }
        }

        private void SetInputDirectionModifier(Vector3 movementVector) {
            Vector2 inputDirection = new Vector2(movementVector.x, movementVector.z);
            Vector2 velocityDirection = new Vector2(PlayerRigidbody.velocity.x, PlayerRigidbody.velocity.z);
            _inputDirectionModifier = _inputDirectionCurve.Evaluate(Vector2.Dot(inputDirection.normalized, velocityDirection.normalized));
        }
        /*[SerializeField] float minVelocity = 0.1f;
        [SerializeField] float magnitudeOneVelocity = 1f;
        private void ClampVelocityDueToInputMagnitude() {
                var inputMagnitude = _playerInputHandler.Inputs.movement.magnitude;
            if (PlayerRigidbody.velocity.magnitude < magnitudeOneVelocity && inputMagnitude > 0) {
                var movementVector = GetMovementVectorAdjustedForCamera();
                var velocityMagnitude = Mathf.Lerp(minVelocity, magnitudeOneVelocity, inputMagnitude);
                PlayerRigidbody.velocity =  movementVector * velocityMagnitude;

            }
        }*/

        private void BreakOn() {
            if (!_useBrake) return;
            if (_toggleBrake && isBrakeOn) {
                BrakeOff();
                return;
            }

            isBrakeOn = true;
            _playerCollider.material = _brakePhysicsMaterial;
            _applyBrakesRoutine = StartCoroutine(ApplyBrakes());

            IEnumerator ApplyBrakes() {
                var time = 0.0f;
                var startDrag = PlayerRigidbody.angularDrag;

                while (time < _brakeApplyTime) {
                    PlayerRigidbody.angularDrag = Mathf.Lerp(startDrag, _brakeStrength, time / _brakeApplyTime);
                    yield return null;
                    time += Time.fixedDeltaTime;
                }
                PlayerRigidbody.angularDrag = _brakeStrength;
            }
        }

        public void AutoBreakOn() {
            _autoBrakeOn = true;
            _playerCollider.material = _brakePhysicsMaterial;
            PlayerRigidbody.angularDrag = _autoBrakeStrength;
        }


        

        //----------------------------------------------
        //--Brake Off
        private void OnBrakeOff() {
            if (!_useBrake) return;
            if (_toggleBrake) return;
            BrakeOff();
            if (_autoBrakeOn) 
                AutoBreakOn();
        }
        
        private void BrakeOff() {
            if (!_useBrake) return;

            if (_applyBrakesRoutine != null) {
                StopCoroutine(_applyBrakesRoutine);
                _applyBrakesRoutine = null;
            }
            isBrakeOn = false;
            PlayerRigidbody.angularDrag = _defaultBrakeStrength;
            _playerCollider.material = _defaultBrakePhysicsMaterial;
        }
        
        private void AutoBrakeOff() {
            _autoBrakeOn = false;
            BrakeOff();
        }

        void InAirMovement(Vector3 startVelocity) {
            Vector2 startVelocityXZ = new Vector2(startVelocity.x, startVelocity.z); // dont let the player XZ magnitude increase beyond this
            if (hasAirControl) {
                var movementVector = GetMovementVectorAdjustedForCamera();
                // only move ball if not in leap mode
                PlayerRigidbody.AddForce(movementVector * (inAirSpeed * Time.deltaTime));

                // Check if velocity has increased and undo it if it has
                Vector2 currentVelocityXZ = new Vector2(PlayerRigidbody.velocity.x, PlayerRigidbody.velocity.z);
                // Ball can maintain speed it had when it went into air and speed up to a minimum speed
                if (currentVelocityXZ.sqrMagnitude > startVelocityXZ.sqrMagnitude && currentVelocityXZ.magnitude > defaultAirVelocityMagnitude) {
                    //print("Velocity Clamped");
                    PlayerRigidbody.AddForce(-movementVector * (inAirSpeed * Time.deltaTime));
                }
            }
        }

        private Vector3 GetMovementVectorAdjustedForCamera() {
            //Vector3 movementVector = _playerInputHandler.MovementVector;
            Vector3 movementVector = _playerInputHandler.Inputs.movement;

            movementVector = Camera.main.transform.TransformDirection(movementVector);
            movementVector.Scale(Vector3.right + Vector3.forward); // add force forwards independent of camera pitch (sets y component to 0)
            return movementVector;
            //return movementVector.normalized;
        }

        public void StopBall() {
            PlayerRigidbody.AddForce(-PlayerRigidbody.velocity, ForceMode.VelocityChange);
            PlayerRigidbody.AddTorque(-PlayerRigidbody.angularVelocity, ForceMode.VelocityChange);
        }

        private void OnCollisionEnter(Collision collision) => CheckIfGrounded(collision);
        private void OnCollisionStay(Collision collision) => CheckIfGrounded(collision);

        private void OnCollisionExit(Collision collision) {
            //if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) {
            if (_jumpLayers.IsLayerInLayerMask(collision.gameObject.layer)) {
                if (_groundedFalseBufferRoutine != null) {
                    StopCoroutine(_groundedFalseBufferRoutine);
                }
                _groundedFalseBufferRoutine = StartCoroutine(GroundedFalseBufferRoutine());
                //IsGrounded = false;
            }
        }

        void CheckIfGrounded(Collision collision) {
            if (_jumped) {
                _jumped = false;
                return;
            }

            if (_jumpLayers.IsLayerInLayerMask(collision.gameObject.layer)) {
                // Is ball colliding with ground
                if (_hasGroundCheckBeenDoneThisFrame && IsGrounded) {
                    // has the ball already be found to be touching the ground this frame
                    return;
                }

                ContactPoint[] contactPoints = new ContactPoint[10];
                int numberOfPoints = collision.GetContacts(contactPoints);
                _groundCheckOffset = _playerCollider.bounds.extents.x * groundCheckOffsetPercentage;
                for (int i = 0; i < numberOfPoints; i++) {
                    if (contactPoints[i].point.y <= transform.position.y - _groundCheckOffset) {
                        // Check if collision point is far enough down on the ball for the ball to be grounded            
                        _isStillInTheAir = false; // Ball will no longer have been in the air on the previous frame (after this frame)
                        _hasGroundCheckBeenDoneThisFrame = true;
                        IsGrounded = true;
                        return;
                    }
                }

                _hasGroundCheckBeenDoneThisFrame = true;
                if (_groundedFalseBufferRoutine != null) {
                    StopCoroutine(_groundedFalseBufferRoutine);
                }

                _groundedFalseBufferRoutine = StartCoroutine(GroundedFalseBufferRoutine());
                //IsGrounded = false;
            }
        }

        IEnumerator GroundedFalseBufferRoutine() {
            yield return new WaitForSeconds(_groundedBufferSeconds);
            IsGrounded = false;
            _groundedFalseBufferRoutine = null;
        }
    }
}