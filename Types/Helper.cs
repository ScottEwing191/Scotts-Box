using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaleBlueOcean.Insubordinate
{
    public static class Helper 
    {
        public static IEnumerator CooldownRoutine(float cooldownTime, Action nullRoutine = null) {
            float time = 0;
            while (time < cooldownTime) {
                time += Time.deltaTime;
                yield return null;
            }
            nullRoutine?.Invoke();
        }
    }
}
