using UnityEngine;

namespace ScottEwing.PhysicsPlayerController{
    public class PhysicsMovementPlayerController : MonoBehaviour{
        //--Serialized Fields
        [SerializeField] private float speed = 150;
        [SerializeField] private float maxVelocity = 9.0f;
        [SerializeField] private float jumpHeight = 2;
        [Tooltip("Controls the strength the players movement input has on the direction of the players jump. The angle of the ground also affects the direction.")] [Range(0,1)] [SerializeField] 
        private float jumpInputDirectionScale = 0.1f;

        [SerializeField] private float inAirDrag = 0.5f;
        [SerializeField] private float inAirSpeed = 75f;

        [Tooltip("The percentage of the ball radius bellow the centre of the ball to use as the groundCheckOffset")] [SerializeField]
        private float groundCheckOffsetPercentage = 0.457f;

        [SerializeField] private bool hasAirControl = true;

        [Tooltip("A transform whose forward vector is always parallel to the ground")] [SerializeField]
        private Transform parallelToGroundTransform;

        [Tooltip("affects how fast player can roll down slopes i think.")] [SerializeField]
        private float maxAngularVelocity = 100;

        [Tooltip("this is the velocity the ball will be able to get up to if jumping from a stand still")] [SerializeField]
        private float defaultAirVelocityMagnitude = 3f;

        //--Auto Properties
        public bool IsGrounded { get; private set; } = true;
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
            jumpDirection += parallelToGroundTransform.TransformDirection(Vector3.up);
            if (IsGrounded) {
                PlayerRigidbody.AddForce(jumpDirection.normalized * jumpHeight, ForceMode.Impulse);
            }
        }

        void Movement() {
            PlayerRigidbody.maxAngularVelocity = maxAngularVelocity;
            if (IsGrounded || hasAirControl) { // if not in the air (has Air Control is probably redundant here as should get here while ball is in air)
                //--Version A: This will result in the player getting a force that goes up any slope it is on (i.e will have a Y component to force)
                //Vector3 movementVector = parallelToGroundTransform.TransformDirection(movementVector);
                
                //--Version B: This will result in the player only getting a force on the XZ axis. 
                Vector3 movementVector = GetMovementVectorAdjustedForCamera();
                PlayerRigidbody.AddForce(movementVector * (speed * Time.deltaTime));
                PlayerRigidbody.velocity = Vector3.ClampMagnitude(PlayerRigidbody.velocity, maxVelocity);
            }
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
            Vector3 movementVector = _playerInputHandler.MovementVector;
            movementVector = Camera.main.transform.TransformDirection(movementVector);
            movementVector.Scale(Vector3.right + Vector3.forward); // add force forwards independent of camera pitch (sets y component to 0)
            return movementVector;
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
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) {
                IsGrounded = false;
            }
        }

        void CheckIfGrounded(Collision collision) {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) {
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
                IsGrounded = false;
            }
        }
    }

}