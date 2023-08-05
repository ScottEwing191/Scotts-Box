using UnityEngine;

namespace ScottEwing.Triggers{
    [AddComponentMenu("ScottEwing/Triggers/DialogueTrigger(deprecated)")]
    public class DialogueTrigger : Trigger{
        [SerializeField] private string[] interactTextArray;
        private string currentInteractText;
        private bool shouldCheckForInput = false;
        private int textIndex = 0;

        private void Start() {
            if (interactTextArray.Length > 0) {
                currentInteractText = interactTextArray[0];
                textIndex = 0;
            }
        }

        private void Update() {
            if (shouldCheckForInput) {
                if (UnityEngine.Input.GetButtonDown("Interact") && IsActivatable) {
                    Triggered();
                }
            }
        }

        public void NextMessage() {
            if ((textIndex + 1 < interactTextArray.Length)) {
                // < not <=  (does the nex index number exist)
                currentInteractText = interactTextArray[textIndex + 1];
                textIndex++;
            }
        }

        protected override void TriggerEntered(Collider other) {
            base.TriggerEntered(other);
            // display the interact text
            shouldCheckForInput = true;
        }

        protected override void TriggerExited(Collider other) {
            base.TriggerExited(other);
            // clear the interact text
            shouldCheckForInput = false;
        }
    }
}