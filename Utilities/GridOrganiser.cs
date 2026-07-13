using Sirenix.OdinInspector;
using UnityEngine;

namespace _1._Scripts{
    public class GridOrganizer : MonoBehaviour
    {
        [Title("Grid Settings")]
        [MinValue(1)]
        public int columns = 5;

        [MinValue(0f)]
        public float spacing = 2f;

        public float gridHeight = 0f;

        [Title("Objects")]
        public GameObject[] objects;

        [Button("Organize In Grid")]
        private void OrganizeInGrid()
        {
            if (objects == null || objects.Length == 0)
            {
                Debug.LogWarning("No objects assigned.");
                return;
            }

            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] == null)
                    continue;

                int row = i / columns;
                int column = i % columns;

                Vector3 position = new Vector3(
                    column * spacing,
                    gridHeight,
                    row * spacing
                );

                objects[i].transform.position = position;
            }
        }
    }
}