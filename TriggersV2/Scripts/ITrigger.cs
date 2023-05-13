using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScottEwing.TriggersV2{
    public interface ITrigger{
        public TriggerV2 TriggerV2 { get; set; }
        //public void Triggered(GameObject other);
        public void Awake();
        public void Start();
        public bool OnTriggerEnter(Collider other);
        public bool OnTriggerStay(Collider other);
        public bool OnTriggerExit(Collider other);

        public bool OnCollisionEnter(Collision collision);
        public bool OnCollisionStay(Collision collision);
        public bool OnCollisionExit(Collision collision);


    }
}
