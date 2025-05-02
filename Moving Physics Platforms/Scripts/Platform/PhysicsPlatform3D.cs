using UnityEngine;

namespace ScottEwing.MovingPlatforms{
    public class PhysicsPlatform3D : BasePhysicsPlatform{
        public Rigidbody Rb { get; set; }
        public PhysicsPlatform3D(Rigidbody rigidbody) {
            Rb = rigidbody;
        }

        public override void MovePlatform(Vector3 direction, float force) {
            Rb.AddForce(direction * force, ForceMode.VelocityChange);

        }

        public override void MoveWithAcceleration(Vector3 direction, float acceleration) {
            Rb.AddForce(direction * acceleration, ForceMode.Acceleration);
        }

        public override Vector3 GetPlatformVelocity() {
            return Rb.linearVelocity;
        }

        /*public override bool IsRigidbodyNull() {
            if (Rb == null) {
                return true;
            }
            return false;
        }*/

        /*public override bool IsRigidbodyNull() {
            throw new System.NotImplementedException();
        }*/

        public override float GetPlatformVelocityMagnitude() {
            return Rb.linearVelocity.magnitude;
        }

        
        
        public override void ClampVelocity(float maxLength) {
            Rb.linearVelocity = Vector3.ClampMagnitude(Rb.linearVelocity, maxLength);       // Set platform velocity to zero
        }

        public override void SetPlatformVelocity(Vector3 velocity) {
            Rb.linearVelocity = velocity;
            //Rb.AddForce(velocity, ForceMode.VelocityChange);
        }
    }
}
