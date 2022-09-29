using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScottEwing.Triggers{
    public interface ILookInteractable{
        Trigger.TriggerState Look(Vector3 cameraPosition);
        
    }

}