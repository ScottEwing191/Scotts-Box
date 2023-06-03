using UnityEngine;


namespace ScottEwing.Trajectory{
    
    public class TrajectoryProjectile : MonoBehaviour{
        //-- This calss still needs some work to remove the also harpoon stuff i.e the layer checks in OnCollisionEnter
        
        
        private Rigidbody _projectileRb;
        private bool _isProjectileInAir = false;

        //--Used to reset the harpoon  
        private Vector3 _resetLocalPosition;
        private Quaternion _resetLocalRotation;
        private Transform _startParent;
        

        private void Awake() {
            _projectileRb = GetComponent<Rigidbody>();
            _resetLocalPosition = transform.localPosition;
            _resetLocalRotation = transform.localRotation;
            _startParent = transform.parent;
        }

        /// <summary>
        /// T
        /// </summary>
        /// <param name="velocity">The initial velocity required to reach the target</param>
        public void Throw(Vector3 velocity) {
            _projectileRb.velocity = Vector3.zero;
            _projectileRb.isKinematic = false;
            _projectileRb.AddForce(velocity, ForceMode.VelocityChange);
            _projectileRb.transform.parent = null;
            _isProjectileInAir = true;
        }

        public void Reset() {
            _isProjectileInAir = false;
            _projectileRb.velocity = Vector3.zero;
            _projectileRb.isKinematic = true;
            transform.parent = _startParent;
            transform.localPosition = _resetLocalPosition;
            transform.localRotation = _resetLocalRotation;
        }

        private void Update() {
            if (_isProjectileInAir && _projectileRb.velocity.sqrMagnitude > 0) {
                transform.rotation = Quaternion.LookRotation(_projectileRb.velocity.normalized);
            }
        }

        private void OnCollisionEnter(Collision collision) {
            if (!_isProjectileInAir) {
                return;
            }
            _isProjectileInAir = false;
            _projectileRb.isKinematic = true;
            if (collision.gameObject.layer == LayerMask.NameToLayer("HarpoonableTrash") ||collision.gameObject.layer == LayerMask.NameToLayer("Default")) {
                transform.parent = collision.transform;
                Reset();
            }
        }
    }
}