using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ScottEwing.TriggersV2{
    public interface ITakesInput{
        //public InputActionProperty InputActionReference { get; set; }
        public bool ShouldCheckInput { get; set; }
        public void GetInput();
    }
}
