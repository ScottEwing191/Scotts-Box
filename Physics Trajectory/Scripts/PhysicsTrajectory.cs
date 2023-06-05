using UnityEngine;

namespace ScottEwing.Trajectory{
    public class PhysicsTrajectory{
        private static float defaultGravity = -9.81f;
        private static float defaultTrajectoryHeight = 1;
        
        
        public static bool TryCalculateLaunchData(Vector3 targetPosition, out LaunchData data, Vector3 startPosition, float trajectoryHeight, float gravity)
        {
            // Calculate the displacement in the vertical direction (Y-axis) between the target and start positions.
            var displacementY = targetPosition.y - startPosition.y;
            // Check if the displacement is greater than the trajectory height.
            // If so, the target is unreachable, and the calculation fails.
            if (displacementY > trajectoryHeight)
            {
                Debug.Log("Couldn't Calculate Launch Data: Height difference between harpoon and trash is greater than harpoon trajectory height");
                data = default;
                return false;
            }
            // Calculate the displacement in the horizontal plane (XZ-plane) between the target and start positions.
            var displacementXZ = new Vector3(targetPosition.x - startPosition.x, 0, targetPosition.z - startPosition.z);
            // Calculate the time to reach the target by considering the trajectory height and gravity.
            var time = Mathf.Sqrt(-2 * trajectoryHeight / gravity) + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity);
            // Calculate the initial upward velocity based on the trajectory height and gravity.
            var velocityY = Vector3.up * Mathf.Sqrt(-2.0f * gravity * trajectoryHeight);
            // Calculate the horizontal velocity required to reach the target within the calculated time.
            var velocityXZ = displacementXZ / time;
            // Combine the horizontal and vertical velocities to obtain the initial velocity vector.
            data = new LaunchData(velocityXZ + velocityY, time);
            return true;
        }
        
        public static Vector3[] GetPathPointsTimeLimit(LaunchData launchData, float timeLimit, Vector3 startPosition, int resolution = 30, float _gravity = -9.81f) {
            var previousDrawPoint = startPosition;
            //const int resolution = 30; // how many times are we checking the path when drawing the line
            var linePath = new Vector3[resolution + 1];

            for (var i = 0; i <= resolution; i++) {
                var simulationTime = i / (float)resolution * timeLimit; // gives a variable going from 0 to the timeToTarget over the course of the for loop
                var displacement = launchData.initialVelocity * simulationTime + Vector3.up * _gravity * simulationTime * simulationTime / 2f; // using 3rd suvat equation  s = ut + at^2 / 2
                var drawPoint = startPosition + displacement;
                Debug.DrawLine(previousDrawPoint, drawPoint, Color.green);
                previousDrawPoint = drawPoint;
                linePath[i] = drawPoint; // will miss first point i.e player position
            }
            return linePath;
        }
    }
}