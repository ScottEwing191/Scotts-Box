using System;
using UnityEngine;

namespace ScottEwing.ExtensionMethods{
    public static class TransformExtensions{
        public static Transform FindParentWithTag(this Transform child, string tag, int depth = 2) {
            if (depth < 0) {
                return null;
            }
            while (true) {
                if (depth == 0) return null;
                if (child.parent == null) return null;
                if (child.parent.CompareTag(tag)) return child.parent;
                child = child.parent;
                depth = depth - 1;
            }
        }
        
        public static void SetParentAndZeroLocalPosition(this Transform transform, Transform parent) {
            transform.SetParent(parent);
            transform.localPosition = Vector3.zero;
        }
        
        /// <summary>
        /// Finds the first sibling with the given tag.
        /// </summary>
        public static Transform FindSiblingWithTag(this Transform transform, string tag) {
            var parent = transform.parent;
            if (parent == null) {
                return null;
            }
            
            for (int i = 0; i < parent.childCount; i++) {
                var child = parent.GetChild(i);
                if (child != transform && child.CompareTag(tag)) {
                    Debug.Log("Found sibling with tag: " + tag, child);
                    return child;
                }
            }
            Debug.Log("No sibling with tag: " + tag + " found.", transform);
            return null;
        }
        
        public static Transform FindRecursive(this Transform parent, string childName)
        {
            foreach (Transform child in parent)
            {
                if (child.name == childName)
                    return child;

                Transform found = child.FindRecursive(childName);

                if (found != null)
                    return found;
            }

            return null;
        }
        
    }
}