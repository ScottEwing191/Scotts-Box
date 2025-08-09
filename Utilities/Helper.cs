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
        
        public static IEnumerator WaitOneFrame(Action nullRoutine) {
            yield return null;
            nullRoutine?.Invoke();
        }
        
        public static IEnumerator WaitFrames(int framesToWait, Action nullRoutine) {
            int waitCount = 0;
            while (waitCount != framesToWait) {
                waitCount++;
                yield return null;
                
            }
            nullRoutine?.Invoke();
        }
    }
}
