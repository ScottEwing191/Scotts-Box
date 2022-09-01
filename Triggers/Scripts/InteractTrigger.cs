using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace ScottEwing.Triggers {
    public class InteractTrigger : Trigger {
        private bool shouldCheckForInput = false;

        private void Update() {
            if (shouldCheckForInput) {
                if (UnityEngine.Input.GetButtonDown("Interact") && _isActivatable) {
                    onTriggered.Invoke();
                    if (_activateOnce) {
                        Destroy(this.gameObject);
                    }
                }
            }
        }
        protected override void OnTriggerEnter(Collider other) {               // display the interact text
            if (other.CompareTag(_triggeredByTag)) {
                shouldCheckForInput = true;
                print("Enter Trigger");

                base.OnTriggerEnter(other);
            }
        }

        protected override void OnTriggerStay(Collider other) {
            base.OnTriggerStay(other);
        }

        protected override void OnTriggerExit(Collider other) {                // clear the interact text
            if (other.CompareTag(_triggeredByTag)) {
                shouldCheckForInput = false;
                print("Exit Trigger");
                base.OnTriggerExit(other);
            }
        }
    } 
}
