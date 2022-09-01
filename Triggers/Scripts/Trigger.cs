using UnityEngine;
using UnityEngine.Events;

namespace ScottEwing.Triggers {
    public abstract class Trigger : MonoBehaviour {
        [SerializeField] protected bool _activateOnce = true;
        [SerializeField] protected bool _isActivatable = true;

        [SerializeField] protected string _triggeredByTag = "Player";

        [SerializeField] protected UnityEvent onTriggered;
        [SerializeField] protected UnityEvent onTriggerEnter;
        [SerializeField] protected UnityEvent onTriggerStay;
        [SerializeField] protected UnityEvent onTriggerExit;
        
        public bool IsActivatable {
            get { return _isActivatable; }
            set { _isActivatable = value; }
        }

        private void OnEnable() => onTriggered.AddListener(DestroyOnTriggered);

        private void OnDisable() => onTriggered.AddListener(DestroyOnTriggered);

        private void DestroyOnTriggered() {
            if (_activateOnce) {
                Destroy(this.gameObject);
            }
        }

        protected virtual void OnTriggerEnter(Collider other) => onTriggerEnter?.Invoke();
        protected virtual void OnTriggerStay(Collider other) => onTriggerStay?.Invoke();
        protected virtual void OnTriggerExit(Collider other) => onTriggerExit?.Invoke();

        /// <summary>
        /// Disables the Game object the trigger is attached to
        /// </summary>
        public void DisableTriggerObject() {
            gameObject.SetActive(false);
        }
    } 
}
