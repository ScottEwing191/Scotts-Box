using System;

using ScottEwing.ExtensionMethods;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using NaughtyAttributes;
#endif
using UnityEngine;

namespace ScottEwing.Triggers
{
    [Serializable]
    public class TriggerBy{
        private enum TriggeredBy{
            Tag,
            LayerMask,
            Either,
            Collider3D
        }

        [SerializeField] private TriggeredBy _triggeredBy = TriggeredBy.LayerMask;
        
        [ShowIf("_triggeredBy", TriggeredBy.LayerMask)]
        [SerializeField] private LayerMask _triggeredByMask = 1;
        
        [ShowIf("_triggeredBy", TriggeredBy.Tag)]
        [SerializeField] protected string _triggeredByTag = "Player";

        [ShowIf("_triggeredBy", TriggeredBy.Collider3D)]
        [SerializeField] protected Collider _targetCollider;
        
        /// Also checks if trigger is activatable
        public bool IsColliderValid(Collider other, bool isActivatable = true) {
            if (!isActivatable) {
                return false;
            }

            return _triggeredBy switch {
                TriggeredBy.Tag => other.CompareTag(_triggeredByTag),
                TriggeredBy.LayerMask => _triggeredByMask.IsLayerInLayerMask(other.gameObject.layer),
                TriggeredBy.Either => other.CompareTag(_triggeredByTag) || _triggeredByMask.IsLayerInLayerMask(other.gameObject.layer),
                TriggeredBy.Collider3D => other == _targetCollider,
                _ => false
            };
        }
        
        
        //--is invalid if collision events should not be used or if not colliding with object in layer mask or with target tag
        public bool IsCollisionValid(Collision other, bool isActivatable = true) => IsColliderValid(other.collider, isActivatable);
        public bool IsCollisionValid(Collision2D other, bool isActivatable = true) => IsColliderValid(other.collider, isActivatable);
        public bool IsColliderValid(Collider2D other, bool isActivatable = true) => IsColliderValid(other.transform, isActivatable);
        public bool IsColliderValid(Transform other, bool isActivatable = true) {
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
    }
}