using System;

using ScottEwing.ExtensionMethods;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using NaughtyAttributes;
#endif
using UnityEngine;

namespace ScottEwing.TriggersV2
{
    [Serializable]
    public class TriggerBy{
        private enum TriggeredBy{
            Tag,
            LayerMask,
            Either
        }

        [SerializeField] private TriggeredBy _triggeredBy = TriggeredBy.LayerMask;
        
        [HideIf("_triggeredBy", TriggeredBy.Tag)]
        [SerializeField] private LayerMask _triggeredByMask = 1;
        
        [HideIf("_triggeredBy", TriggeredBy.LayerMask )]
        [SerializeField] protected string _triggeredByTag = "Player";
        
        /// Also checks if trigger is activatable
        public bool IsColliderValid(Collider other, bool isActivatable = true) {
            if (!isActivatable) {
                return false;
            }

            return _triggeredBy switch {
                TriggeredBy.Tag => other.CompareTag(_triggeredByTag),
                TriggeredBy.LayerMask => _triggeredByMask.IsLayerInLayerMask(other.gameObject.layer),
                TriggeredBy.Either => other.CompareTag(_triggeredByTag) || _triggeredByMask.IsLayerInLayerMask(other.gameObject.layer),
                _ => false
            };
        }
        
        //--is invalid if collision events should not be used or if not colliding with object in layer mask or with target tag
        public bool IsCollisionValid(Collision other, bool isActivatable = true) => IsColliderValid(other.collider, isActivatable);
        
    }
}
