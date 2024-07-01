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
        
    }
}