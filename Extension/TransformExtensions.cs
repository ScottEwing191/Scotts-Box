using UnityEngine;

namespace ScottEwing.ExtensionMethods{
    public static class TransformExtensions{
        public static Transform FindParentWithTag(this Transform child, string tag, int depth = -1) {
            if (depth == 0) return null;
            if (child.parent == null) return null;
            if (child.parent.CompareTag(tag)) return child.parent;
            return FindParentWithTag(child.parent, tag, depth - 1);
            
        }
    }
}