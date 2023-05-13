using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScottEwing.Triggers{
    public interface ILookInteractable{
        TriggerState Look(Collider other, bool localCast = false);
        
    }

}