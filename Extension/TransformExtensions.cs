using System;
using System.Collections.Generic;
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

        /*public static Transform[] GetChildren(this Transform transform, bool recursive) {
            var childCount = transform.childCount;
            if (childCount == 0) {
                return Array.Empty<Transform>();
                
            }
            var childrenArray = new Transform[childCount];
            
            for (int i = 0; i < childCount; i++) {
                childrenArray[i] = transform.GetChild(i);
            }
            return childrenArray;
        }*/
        
        public static Transform[] GetChildren(this Transform transform, int depth = 0)
        {
            if (transform.childCount == 0)
                return Array.Empty<Transform>();

            var results = new List<Transform>();

            void Collect(Transform parent, int currentDepth)
            {
                for (int i = 0; i < parent.childCount; i++)
                {
                    var child = parent.GetChild(i);
                    results.Add(child);

                    // Stop if we've reached max depth
                    if (currentDepth == 0)
                        continue;

                    // Negative depth = unlimited
                    Collect(child, currentDepth - 1);
                }
            }

            Collect(transform, depth);
            return results.ToArray();
        }
        
    }
}