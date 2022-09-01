using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScottEwing.Triggers{
    public interface ILookInteractable{
        void Look(Vector3 cameraPosition);
        void LookEnter();
        void LookStay();
        void LookExit();

    }

}