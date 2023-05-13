using System;
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
        [Tooltip("Controls the strength the players movement input has on the direction of the players jump. The angle of the ground also affects the direction.")] [Range(0,1)] [SerializeField] 
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
        [SerializeField] private float _brakeStrength = 50;
        [SerializeField] private PhysicMaterial _brakePhysicsMaterial;

        [SerializeField] private float _defaultBrakeStrength = 1;
        [SerializeField] private PhysicMaterial _defaultBrakePhysicsMaterial;
        [SerializeField] private  float _brakeApplyTime = 0.5f;

        [SerializeField] private bool isBrakeOn = false;

        


        //--Auto Properties
        [SerializeField] private float _groundedBufferSeconds = 0.075f;
        private Coroutine _groundedFalseBufferRoutine;
        [field:SerializeField]public bool IsGrounded { get; private set; } = true;
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

        void Start() {
            PlayerRigidbody = GetComponent<Rigidbody>();
            _playerCollider = GetComponent<Collider>();
            _defaultDrag = PlayerRigidbody.drag;
            _playerInputHandler = GetComponentInParent<PlayerInputHandler>();
            _playerInputHandler.jump += DoJump;

            _playerInputHandler.brakeOn += BreakOn;
            _playerInputHandler.brakeOff += BreakOff;

        }

        private void OnDestroy() {
            _playerInputHandler.jump -= DoJump;
            _playerInputHandler.brakeOn -= BreakOn;
            _playerInputHandler.brakeOff -= BreakOff;
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
            jumpDirection += Vector3.up;    // simple vertical jump
            if (IsGrounded) {
                PlayerRigidbody.AddForce(jumpDirection.normalized * jumpHeight, ForceMode.Impulse);
                _jumped = true;
                IsGrounded = false;
            }
        }

        void Movement() {
            PlayerRigidbody.maxAngularVelocity = maxAngularVelocity;
            if (IsGrounded || hasAirControl) { // if not in the air (has Air Control is probably redundant here as should get here while ball is in air)
                //--Version A: This will result in the player getting a force that goes up any slope it is on (i.e will have a Y component to force)
                //Vector3 movementVector = parallelToGroundTransform.TransformDirection(movementVector);
                
                //--Version B: This will result in the player only getting a force on the XZ axis. 
                Vector3 movementVector = GetMovementVectorAdjustedForCamera();
                if (!_onlyLimitControllerVelocity) {
                    PlayerRigidbody.AddForce(movementVector * (speed * Time.deltaTime));
                    PlayerRigidbody.velocity = Vector3.ClampMagnitude(PlayerRigidbody.velocity, maxVelocity);
                }
                else if(PlayerRigidbody.velocity.magnitude < maxVelocity){
                    PlayerRigidbody.AddForce(movementVector * (speed * Time.deltaTime));
                    //PlayerRigidbody.velocity = Vector3.ClampMagnitude(PlayerRigidbody.velocity, maxVelocity);
                }
            }
        }
        
        public void BreakOn() {
            if (!_useBrake) return;
            
            isBrakeOn = true;
            _defaultBrakeStrength = PlayerRigidbody.angularDrag;
            _defaultBrakePhysicsMaterial = _playerCollider.material;
            //PlayerRigidbody.angularDrag = _brakeStrength;
            _playerCollider.material = _brakePhysicsMaterial;
            StartCoroutine(ApplyBrakes());

            IEnumerator ApplyBrakes() {
                var time = 0.0f;
                while (time < _brakeApplyTime && isBrakeOn) {
                    PlayerRigidbody.angularDrag = Mathf.Lerp(_brakeStrength, _defaultBrakeStrength, 1-time/_brakeApplyTime);
                    yield return null;
                    time += Time.fixedDeltaTime;
                }
                PlayerRigidbody.angularDrag = _brakeStrength;

                if (!isBrakeOn) {
                    BreakOff();
                }

            } 
        }

        public void BreakOff() {
            if (!_useBrake) return;
            isBrakeOn = false;
            PlayerRigidbody.angularDrag = _defaultBrakeStrength;
            _playerCollider.material = _defaultBrakePhysicsMaterial;
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
            return movementVector.normalized;
        }

        public void StopBall() {
            PlayerRigidbody.AddForce(-PlayerRigidbody.velocity, ForceMode.VelocityChange);
            PlayerRigidbody.AddTorque(-PlayerRigidbody.angularVelocity, ForceMode.VelocityChange);
        }

        private void OnCollisionEnter(Collision collision) {
            CheckIfGrounded(collision); // Check if the ball is grounded and set is grounded variable
        }

        private void OnCollisionStay(Collision collision) {
            CheckIfGrounded(collision);
        }

        private void OnCollisionExit(Collision collision) {
            //if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) {
            if (_jumpLayers.IsLayerInLayerMask(collision.gameObject.layer)) {
                if (_groundedFalseBufferRoutine!= null) {
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
                if (_groundedFalseBufferRoutine!= null) {
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