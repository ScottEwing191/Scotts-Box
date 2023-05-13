using System;
using UnityEngine;

namespace ScottEwing.TriggersV2{
    [Serializable]
    public struct TestData: ITriggerData{
        [SerializeField] private int _test;

    }
}