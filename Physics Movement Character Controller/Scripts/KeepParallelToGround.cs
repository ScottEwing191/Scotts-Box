using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ScottEwing.PhysicsPlayerController{
    public class KeepParallelToGround : MonoBehaviour{
        [FormerlySerializedAs("player")] [SerializeField]
        private PhysicsMovementPlayerController physicsMovementPlayer;

        [SerializeField] private LayerMask groundLayers;

        private void Update() {
            if (physicsMovementPlayer.IsGrounded) {
                transform.position = physicsMovementPlayer.transform.position;
                RaycastHit hit;
                //Physics.Raycast(transform.position, Vector3.down, out hit, 5f, 1 << LayerMask.NameToLayer("Ground"));
                Physics.Raycast(transform.position, Vector3.down, out hit, 5f, groundLayers);
                Vector3 perpendicularToGround = Vector3.Cross(Camera.main.transform.right, hit.normal);
                transform.LookAt(transform.position + perpendicularToGround, hit.normal);
            }
        }
    }
}
