using System.Collections.Generic;
using UnityEngine;

namespace ScottEwing.Trajectory{
    public class PhysicsTrajectoryBehaviour : MonoBehaviour{
        [Tooltip("The height above the initial position of the launchObjectRigidbody that it will reach. (Increase this if trying to hit objects above the harpoon)")]
        [SerializeField] private float _trajectoryHeight = 1;              
        [SerializeField] private float _gravity = -9.81f;
        [SerializeField] private LineRenderer _lineRenderer;
        
        /// <summary>
        /// Check if it is possible to reach the target and calculates the initial velocity required to get there
        /// </summary>
        /// <returns>True if it is possible to reach target and false if not.</returns>
        public bool TryCalculateLaunchData(Vector3 targetPosition, out LaunchData data, Vector3 startPosition) {
            return PhysicsTrajectory.TryCalculateLaunchData(targetPosition, out data, startPosition,_trajectoryHeight, _gravity);
        }
        
        public Vector3[] GetPathPointsTimeLimit(LaunchData launchData, float timeLimit, Vector3 startPosition, int resolution = 30) {
            return PhysicsTrajectory.GetPathPointsTimeLimit(launchData,timeLimit,startPosition,resolution,_gravity);
        }
        public Vector3[] GetPathPoints(LaunchData launchData, Vector3 startPosition) {
            return GetPathPointsTimeLimit(launchData, launchData.timeTotarget, startPosition);
        }

        // Takes an array of points (from DrawPath and draws a line between each of the creating an arc)
        public void RenderPath(Vector3[] linePoints) {
            _lineRenderer.positionCount = linePoints.Length;
            _lineRenderer.SetPositions(linePoints);
        }

        public void ClearLine() {
            _lineRenderer.positionCount = 0;
        }
    }
}
