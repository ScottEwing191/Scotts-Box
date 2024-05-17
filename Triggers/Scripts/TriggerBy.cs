using System;

using ScottEwing.ExtensionMethods;
//using TPUModelerEditor;
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
            Collider3D,
            Type,
            //TypeInParent,
            //TypeInChild
        }

        [SerializeField] private TriggeredBy _triggeredBy = TriggeredBy.LayerMask;
        
        [ShowIf("_triggeredBy", TriggeredBy.LayerMask)]
        [SerializeField] private LayerMask _triggeredByMask = 1;
        
        [ShowIf("_triggeredBy", TriggeredBy.Tag)]
        [SerializeField] protected string _triggeredByTag = "Player";

        [ShowIf("_triggeredBy", TriggeredBy.Collider3D)]
        [SerializeField] protected Collider _targetCollider;
        
        //[ShowIf("@_triggeredBy == TriggeredBy.Type || _triggeredBy == TriggeredBy.TypeInParent || _triggeredBy == TriggeredBy.TypeInChild")]
        [ShowIf("_triggeredBy", TriggeredBy.Type)]
        [Tooltip("The type of Component to search for.")]
        [SerializeField] private string _type;
        
        /// Also checks if trigger is activatable
        /*public bool IsColliderValid(Collider other, bool isActivatable = true) {
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
        }*/

        public bool IsColliderValid(Collider other, bool isActivatable = true) {
            if (!isActivatable)return false;

            if (_triggeredBy == TriggeredBy.Collider3D) 
                return other == _targetCollider;
            
            return IsTransformValid(other.transform, isActivatable);
        }


        //-- Collision 3D
        //--is invalid if collision events should not be used or if not colliding with object in layer mask or with target tag
        public bool IsCollisionValid(Collision other, bool isActivatable = true) => IsColliderValid(other.collider, isActivatable);
        
        //-- Collision 2D
        public bool IsCollisionValid(Collision2D other, bool isActivatable = true) => IsColliderValid(other.collider, isActivatable);
        
        //-- Collider 2D
        public bool IsColliderValid(Collider2D other, bool isActivatable = true) => IsTransformValid(other.transform, isActivatable);
        
        //--Transform
        public bool IsTransformValid(Transform other, bool isActivatable = true) {
            if (!isActivatable) {
                return false;
            }

            return _triggeredBy switch {
                TriggeredBy.Tag => other.CompareTag(_triggeredByTag),
                TriggeredBy.LayerMask => _triggeredByMask.IsLayerInLayerMask(other.gameObject.layer),
                TriggeredBy.Either => other.CompareTag(_triggeredByTag) || _triggeredByMask.IsLayerInLayerMask(other.gameObject.layer),
                TriggeredBy.Type => other.GetComponent(_type) != null,
                //TriggeredBy.TypeInParent => other.GetComponentInParent(Type.GetType(_type)) != null,
                //TriggeredBy.TypeInChild => other.GetComponentInChildren(Type.GetType(_type)) != null,
                _ => false
            };
        }
    }
}