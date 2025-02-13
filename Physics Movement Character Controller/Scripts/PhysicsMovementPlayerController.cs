using System.Collections;
using ScottEwing.ExtensionMethods;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#elif NAUGHTY_ATTRIBUTES
    using NaughtyAttributes;
#endif

using UnityEngine;

namespace ScottEwing.PhysicsPlayerController{
    public class PhysicsMovementPlayerController : MonoBehaviour{
        //--Serialized Fields
        [SerializeField] private float speed = 14;

        [Tooltip("If true, this script cannot increase the speed to above the minimum value while grounded, but other things (e.g. explosions) can increase " +
                 "the speed to above the min velocity while grounded. If false nothing can increase speed above max while grounded")]
        [SerializeField] private bool clampVelocityMagnitude = false;

        [SerializeField] private float maxVelocity = 9.0f;
        [SerializeField] private float jumpHeight = 2;

        [Tooltip("Controls the strength the players movement input has on the direction of the players jump. The angle of the ground also affects the direction.")] [Range(0, 1)] [SerializeField]
        private float jumpInputDirectionScale = 0.1f;

        [SerializeField] private float inAirDrag = 0.5f;
        [SerializeField] private float inAirSpeed = 4f;

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

        [BoxGroup("Brake")]
        [SerializeField] private bool _useBrake = true;

        [ShowIf(nameof(_useBrake))] [BoxGroup("Brake")] [SerializeField]
        public bool _toggleBrake = false;

        [ShowIf(nameof(_useBrake))] [BoxGroup("Brake")] [SerializeField]
        private float _brakeStrength = 50;

        [ShowIf(nameof(_useBrake))] [BoxGroup("Brake")] [SerializeField]
        private float _brakeApplyTime = 0.5f;

        [ShowIf(nameof(_useBrake))] [BoxGroup("Brake")] [SerializeField]
        private PhysicMaterial _brakePhysicsMaterial;

        [BoxGroup("Auto Brake")] [SerializeField]
        [Tooltip("If true the brake will be applied automatically when the player is not moving. If false the brake will only be applied when the player presses the brake button")]
        public bool _useAutoBrake = false;

        [ShowIf(nameof(_useAutoBrake))] [BoxGroup("Auto Brake")] [SerializeField]
        private float _autoBrakeStrength = 10.0f;

        [BoxGroup("Responsive Movement")]
        public bool _useResponsiveMovement = true;

        [ShowIf(nameof(_useResponsiveMovement))] [BoxGroup("Responsive Movement")] [SerializeField]
        private AnimationCurve _accelerationCurve;

        [ShowIf(nameof(_useResponsiveMovement))] [BoxGroup("Responsive Movement")] [SerializeField]
        [Tooltip("The acceleration modifier will be applied over this time after input has begun")]
        private float _accelerateTime = 0.2f;

        [ShowIf(nameof(_useResponsiveMovement))] [BoxGroup("Responsive Movement")] [SerializeField]
        private AnimationCurve _inputDirectionCurve;

        [ShowIf(nameof(_useResponsiveMovement))] [BoxGroup("Responsive Movement")] [SerializeField]
        [Tooltip("When the Dot product between the input vector and the movement vector is less than this value the angular drag will be increased to make the ball turn faster")]
        private float _inputVectorMovementVectorDotTarget = 0.25f;

        [ShowIf(nameof(_useResponsiveMovement))] [BoxGroup("Responsive Movement")] [SerializeField]
        private float _inputDirectionDragModifier = 15.0f;



        //--Auto Properties
        [SerializeField] private float _groundedBufferSeconds = 0.075f;
        private Coroutine _groundedFalseBufferRoutine;
        [field: SerializeField] public bool IsGrounded { get; private set; } = true;
        private bool _jumped;
        private Rigidbody PlayerRigidbody { get; set; }

        //-----------------Private--------------------- 

        //--Brake / Auto Brake
        [SerializeField] private bool _isAutoBrakeOn;
        [SerializeField] private bool _isBrakeOn = false;
        private float _defaultBrakeStrength = 1;
        private PhysicMaterial _defaultBrakePhysicsMaterial;
        private Coroutine _applyBrakesRoutine;

        //--Responsive Movement
        private float _accelerationModifier = 1.0f;
        private float _inputDirectionModifier = 1.0f;
        private Coroutine _accelerationRoutine;


        private float _defaultDrag = 0.1f;
        private bool _isStillInTheAir; // True if the ball is in the air and was also in the air in the previous frame. (Except first frame in air)
        private bool _hasGroundCheckBeenDoneThisFrame; // keeps track of whether ground check has already been done
        private float _groundCheckOffset = 0.4f; // If the the collision point with the ground is bellow this distance relative to the centre of the ball then the ball with be grounded
        private Vector3 _jumpStartVelocity; // When the ball jumps or is in the air the velocity will be clamped to this amount. 
        private Collider _playerCollider; // the collider attached to the player used to check if player is grounded
        private PlayerInputHandler _playerInputHandler;


        void Start() {
            PlayerRigidbody = GetComponent<Rigidbody>();
            _playerCollider = GetComponent<Collider>();
            _defaultDrag = PlayerRigidbody.drag;
            _playerInputHandler = GetComponentInParent<PlayerInputHandler>();
            _playerInputHandler.jump += Jump;

            _playerInputHandler.brakeOn += BrakeOn;
            _playerInputHandler.brakeOff += OnBrakeOff;

            _defaultBrakeStrength = PlayerRigidbody.angularDrag;
            _defaultBrakePhysicsMaterial = _playerCollider.material;
            PlayerRigidbody.maxAngularVelocity = maxAngularVelocity;
        }

        private void OnDestroy() {
            _playerInputHandler.jump -= Jump;
            _playerInputHandler.brakeOn -= BrakeOn;
            _playerInputHandler.brakeOff -= OnBrakeOff;
        }

        void FixedUpdate() {
            Vector3 movementVector = GetMovementVectorAdjustedForCamera();
            HandleAutoBreak(movementVector);
            if (IsGrounded) {
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

        public void Jump() {
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
            Vector3 movementVector = GetMovementVectorAdjustedForCamera();
            //HandleAutoBreak(movementVector);
            if (!IsGrounded) return;
            SetResponsiveMovementModifiers(movementVector);
            ApplyMovementForce(movementVector);
        }

        void SetResponsiveMovementModifiers(Vector3 movementVector) {
            if (_useResponsiveMovement) {
                SetAccelerationModifier(movementVector);
                SetInputDirectionModifier(movementVector);
            }
            else {
                _accelerationModifier = 1.0f;
                _inputDirectionModifier = 1.0f;
            }
        }

        void ApplyMovementForce(Vector3 movementVector) {
            if (PlayerRigidbody.velocity.magnitude < maxVelocity) {
                Vector3 force = movementVector * speed * _accelerationModifier * _inputDirectionModifier;
                PlayerRigidbody.AddForce(force);
            }

            if (clampVelocityMagnitude)
                PlayerRigidbody.velocity = Vector3.ClampMagnitude(PlayerRigidbody.velocity, maxVelocity);
        }

        private void HandleAutoBreak(Vector3 movementVector) {
            if (!_useAutoBrake || _isBrakeOn) return;
            if (movementVector.magnitude == 0 && !_isBrakeOn && !_isAutoBrakeOn && IsGrounded) {
                AutoBreakOn();
            }
            else if (_isAutoBrakeOn && (movementVector.magnitude != 0 || !IsGrounded))
                AutoBrakeOff();
        }

        private float prevSqrVelocityMagnitude;

        private Vector3 _prevMovementVector;



        private void SetAccelerationModifier(Vector3 movementVector) {
            if (movementVector.magnitude == 0) {
                _prevMovementVector = Vector3.zero;
                return;
            }

            if (_prevMovementVector.magnitude > 0) {
                _prevMovementVector = movementVector;
                return;
            }

            if (movementVector.magnitude < 0.5f) {
                _accelerationModifier = 1.0f;
                return;
            }

            _prevMovementVector = movementVector;

            if (_accelerationRoutine != null) {
                StopCoroutine(_accelerationRoutine);
                _accelerationRoutine = null;
            }

            _accelerationRoutine = StartCoroutine(AccelerateRoutine());

            IEnumerator AccelerateRoutine() {
                var time = 0.0f;

                while (time < _accelerateTime) {
                    time += Time.fixedDeltaTime;
                    _accelerationModifier = _accelerationCurve.Evaluate(PlayerRigidbody.velocity.magnitude);
                    yield return new WaitForFixedUpdate();
                }

                _accelerationModifier = 1.0f;
                _accelerationRoutine = null;
            }
        }

        private void SetInputDirectionModifier(Vector3 movementVector) {
            if (_isBrakeOn) {
                return;
            }

            if (movementVector.magnitude == 0) {
                _inputDirectionModifier = 1.0f;
                return;
            }

            Vector2 inputDirection = new Vector2(movementVector.x, movementVector.z);
            Vector2 velocityDirection = new Vector2(PlayerRigidbody.velocity.x, PlayerRigidbody.velocity.z);
            float dot = Vector2.Dot(inputDirection.normalized, velocityDirection.normalized);
            PlayerRigidbody.angularDrag = dot < _inputVectorMovementVectorDotTarget ? _inputDirectionDragModifier : _defaultBrakeStrength;
        }

        private void BrakeOn() {
            if (!_useBrake) return;
            if (_toggleBrake && _isBrakeOn) {
                BrakeOff();
                return;
            }

            _isBrakeOn = true;
            _playerCollider.material = _brakePhysicsMaterial;
            _applyBrakesRoutine = StartCoroutine(ApplyBrakes(_brakeStrength));

        }

        private void AutoBreakOn() {
            _isAutoBrakeOn = true;
            _playerCollider.material = _brakePhysicsMaterial;
            _applyBrakesRoutine = StartCoroutine(ApplyBrakes(_autoBrakeStrength));
        }
        
        IEnumerator ApplyBrakes(float brakeStrength) {
            var time = 0.0f;
            var defaultBrakeStrength = _defaultBrakeStrength;
            var startDrag = PlayerRigidbody.angularDrag;

            // If angular drag is already higher than defaultBrakeStrength, scale down the apply time
            float timeFactor = startDrag > defaultBrakeStrength ? defaultBrakeStrength / startDrag : 1.0f;
            float adjustedBrakeApplyTime = _brakeApplyTime * timeFactor;
            print("Start Drag: " + startDrag);
            while (time < adjustedBrakeApplyTime) {
                PlayerRigidbody.angularDrag = Mathf.Lerp(startDrag, brakeStrength, time / adjustedBrakeApplyTime);
                yield return null;
                time += Time.fixedDeltaTime;
            }

            PlayerRigidbody.angularDrag = brakeStrength;
        }


        //----------------------------------------------
        //--Brake Off
        private void OnBrakeOff() {
            if (!_useBrake) return;
            if (_toggleBrake) return;
            _isBrakeOn = false;
            BrakeOff();
            if (_isAutoBrakeOn)
                AutoBreakOn();
        }

        private void BrakeOff() {
            if (!_useBrake && !_useAutoBrake) return;

            if (_applyBrakesRoutine != null) {
                StopCoroutine(_applyBrakesRoutine);
                _applyBrakesRoutine = null;
            }

            PlayerRigidbody.angularDrag = _defaultBrakeStrength;
            _playerCollider.material = _defaultBrakePhysicsMaterial;
        }

        private void AutoBrakeOff() {
            _isAutoBrakeOn = false;
            BrakeOff();
        }

        void InAirMovement(Vector3 startVelocity) {
            Vector2 startVelocityXZ = new Vector2(startVelocity.x, startVelocity.z); // dont let the player XZ magnitude increase beyond this
            if (hasAirControl) {
                var movementVector = GetMovementVectorAdjustedForCamera();
                // only move ball if not in leap mode
                PlayerRigidbody.AddForce(movementVector * (inAirSpeed));

                // Check if velocity has increased and undo it if it has
                Vector2 currentVelocityXZ = new Vector2(PlayerRigidbody.velocity.x, PlayerRigidbody.velocity.z);
                // Ball can maintain speed it had when it went into air and speed up to a minimum speed
                if (currentVelocityXZ.sqrMagnitude > startVelocityXZ.sqrMagnitude && currentVelocityXZ.magnitude > defaultAirVelocityMagnitude) {
                    //print("Velocity Clamped");
                    PlayerRigidbody.AddForce(-movementVector * (inAirSpeed));
                }
            }
        }

        /*private Vector3 GetMovementVectorAdjustedForCamera() {
            //Vector3 movementVector = _playerInputHandler.MovementVector;
            Vector3 movementVector = _playerInputHandler.Inputs.movement;

            movementVector = Camera.main.transform.TransformDirection(movementVector);
            movementVector.Scale(Vector3.right + Vector3.forward); // add force forwards independent of camera pitch (sets y component to 0)
            //return movementVector;
            return movementVector.normalized;
        }*/
        
        private Vector3 GetMovementVectorAdjustedForCamera() {
            // Get the movement input from the player
            Vector3 movementVector = _playerInputHandler.Inputs.movement;

            // Get the y-axis rotation (yaw) of the camera
            float cameraYaw = Camera.main.transform.eulerAngles.y;

            // Create a new forward direction based on the camera's y-axis rotation
            Vector3 forward = Quaternion.Euler(0, cameraYaw, 0) * Vector3.forward;
            Vector3 right = Quaternion.Euler(0, cameraYaw, 0) * Vector3.right;

            // Calculate the adjusted movement vector
            Vector3 adjustedMovementVector = (movementVector.x * right + movementVector.z * forward);
            return adjustedMovementVector;
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