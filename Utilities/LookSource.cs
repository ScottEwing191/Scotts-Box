using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif

namespace ScottEwing.Triggers{
    
    [Serializable]
    public class LookSource{
        public enum CastSourceType{
            UseMainCameraTransform,
            AssignSourceTransform
        }
        [field: SerializeField] public CastSourceType CastSource { get; set; } = CastSourceType.UseMainCameraTransform;

        [Tooltip("Controls the position and direction of the sphere cast")]
#if ODIN_INSPECTOR
        [ShowIf("CastSource", CastSourceType.AssignSourceTransform)]
#endif
        [SerializeField] private Transform _sourceTransform;
        public static Transform CachedCameraMain { get; set; }
        private Transform _currentSource;
        
        public Transform CurrentSource {
            get {
                switch (CastSource) {
                    //--might be better to just always return Camera.main.transform instead of caching the main camera transform??
                    case CastSourceType.UseMainCameraTransform:
                        if (CachedCameraMain == null || !CachedCameraMain.gameObject.activeInHierarchy || !CachedCameraMain.gameObject.activeSelf) {
                            CachedCameraMain = Camera.main.transform;
                        }
                        return CachedCameraMain;
                    case CastSourceType.AssignSourceTransform:
                        if (_sourceTransform == null) {
                            throw new Exception("Source Transform is not assigned");
                        }
                        return _sourceTransform;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set => _currentSource = value;
        }
        
        //--was supposed to be called when Camera.main it changed but has never been set up
        public static void OnCameraChanged(Transform newCamera) {
            CachedCameraMain = newCamera;
        }
    }
}
