using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScottEwing.Helpers
{
    public static class Helper 
    {
        public static IEnumerator CooldownRoutine(float cooldownTime, Action nullRoutine = null) {
            yield return GetWait(cooldownTime);
            nullRoutine?.Invoke();
        }

        private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new Dictionary<float, WaitForSeconds>();
        public static WaitForSeconds GetWait(float time) {
            if (WaitDictionary.TryGetValue(time, out var wait)) return wait;
            WaitDictionary[time] = new WaitForSeconds(time);
            return WaitDictionary[time];
        }
    }
}
