using UnityEngine;
using UnityEngine.AI;

namespace ScottEwing.ExtensionMethods
{
    public static class NavMeshAgentExtension
    {
        public static float GetPathRemainingDistance(this NavMeshAgent navMeshAgent)
        {
            if (navMeshAgent.pathPending ||
                navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid ||
                navMeshAgent.path.corners.Length == 0)
                return -1f;

            float distance = 0.0f;
            for (int i = 0; i < navMeshAgent.path.corners.Length - 1; ++i)
            {
                distance += Vector3.Distance(navMeshAgent.path.corners[i], navMeshAgent.path.corners[i + 1]);
            }

            return distance;
        }
    }
    
    public static class Vector3Extensions
    {
        /// <returns>Return the average direction between two vectors</returns>
        public static Vector3 GetAverageDirection(this Vector3 v1, Vector3 v2) {
            v1 = v1.normalized;
            v2 = v2.normalized;
            var average = v1 + v2;
            if (average == Vector3.zero) 
                Debug.LogWarning("Vectors were opposites so average was calculated as (0,0,0)");
            return average.normalized;
        }
        
        /// <returns>Returns the position in the middle of two vectors</returns>
        public static Vector3 GetMidPoint(this Vector3 v1, Vector3 v2) {
            var midPoint = (v1 + v2) / 2;
            return midPoint;
        }
    }
}