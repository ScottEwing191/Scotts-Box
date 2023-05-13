using UnityEngine;

namespace ScottEwing.TriggersV2{
    public class BaseTrigger: ITrigger{
        public TriggerV2 TriggerV2 { get; set; }
        
        public BaseTrigger(TriggerV2 triggerV2, ITriggerData data = null) => TriggerV2 = triggerV2;

        public virtual void Awake() { }
        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void LateUpdate() { }



        public virtual void Triggered(GameObject other = null) => TriggerV2.Triggered();

        public virtual bool OnTriggerEnter(Collider other) => true;
        public virtual bool OnTriggerStay(Collider other) => true;
        public virtual bool OnTriggerExit(Collider other) => true;
        public virtual bool OnCollisionEnter(Collision collision) => true;
        public virtual bool OnCollisionStay(Collision collision) => true;
        public virtual bool OnCollisionExit(Collision collision) => true;
        
        //public void UpdateData(ITriggerData data, )

    }
}