using ScottEwing.Triggers;
using UnityEngine;

namespace ScottEwing.Triggers{
    public class LookInteractTriggerDemo : MonoBehaviour{
        public void Look(Vector3 cameraPosition) {
        }

        public void LookEnter() {
            print(gameObject.name + ": Look Enter");
        }

        public void LookStay() {
            print(gameObject.name + ": Look Stay");
        }

        public void LookExit() {
            print(gameObject.name + ": Look Exit");
        }

        public void LookTriggered() {
            print(gameObject.name + ": Look Triggered");
        }
    }
}
