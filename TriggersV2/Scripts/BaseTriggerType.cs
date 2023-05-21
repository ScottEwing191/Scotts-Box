using UnityEngine;

namespace ScottEwing.TriggersV2{
    public class BaseTriggerType: ITrigger{
        public BaseTrigger Trigger { get; set; }
        
        public BaseTriggerType(BaseTrigger trigger, ITriggerData data = null) {
            Trigger = trigger;
        }

        public virtual void Awake() { }
        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void LateUpdate() { }



        public virtual void Triggered(GameObject other = null) => Trigger.Triggered();

        public virtual bool OnTriggerEnter(Collider other) => true;
        public virtual bool OnTriggerStay(Collider other) => true;
        public virtual bool OnTriggerExit(Collider other) => true;
        public virtual bool OnCollisionEnter(Collision collision) => true;
        public virtual bool OnCollisionStay(Collision collision) => true;
        public virtual bool OnCollisionExit(Collision collision) => true;
        
        //public void UpdateData(ITriggerData data, )

    }
}