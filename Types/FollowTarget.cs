using System;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#elif NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using UnityEngine;

namespace ScottEwing{
    /// <summary>
    /// This class can be attached to a game object to make it follow a target game objects Position/Rotation.
    /// </summary>
    public class FollowTarget : MonoBehaviour{
        private enum PositionOptions{ Position, XZPosition, YPosition, NoPosition }
        private enum RotationOptions{ Rotation, XZRotation, YRotation, NoRotation }

        private enum OffsetType{ World, RelativeToTargetRotation }

        [field: SerializeField] public Transform PositionTarget { get; set; }
        [field: SerializeField] public Transform RotationTarget { get; set; }
        [SerializeField] private UpdateOptions _updateOptions = UpdateOptions.LateUpdate;
        [SerializeField] private PositionOptions _positionOption = PositionOptions.Position;
        [SerializeField] private RotationOptions _rotationOptions = RotationOptions.Rotation;
        [SerializeField] private OffsetType _offsetType = OffsetType.RelativeToTargetRotation;
        [Tooltip("If follower is child and is offset from parent, and parent Rotation is not (0,0,0), then parent should be used as pivot, otherwise ignore parent pivot ")]
        [SerializeField] private Transform _parentPivot;

        [Tooltip("If true, the follower's offset will be the difference in positions between the follower and target when the game starts, otherwise the follower will start at the same position as the target plus the specified offset")]
        [SerializeField] private bool _useStartOffset = false;
        [HideIf("_useStartOffset")]
        [SerializeField] private Vector3 _offsetPosition;

        private void Start() {
            transform.rotation.ToAngleAxis(out float angle, out Vector3 axis);
            //-- This seams to work for child/non child followers as long as the start with the same (world) rotation as the target
            /*if (_parentPivot && _parentPivot.rotation != Quaternion.identity) { 
                transform.RotateAround(_parentPivot.position, axis, -angle);
            }*/

            //-- This also seams to work for child/non child followers as long as the start with the same (world) rotation as the target
            if (_parentPivot && transform.rotation!= Quaternion.identity) {
                transform.RotateAround(_parentPivot.position, axis, -angle);
            }

            if (PositionTarget) {
                _offsetPosition = _useStartOffset ? transform.position - PositionTarget.position : _offsetPosition;
            }
        }

        private void Update() {
            if (_updateOptions == UpdateOptions.Update)
                Follow();
        }

        private void FixedUpdate() {
            if (_updateOptions == UpdateOptions.FixedUpdate)
                Follow();
        }

        private void LateUpdate() {
            if (_updateOptions == UpdateOptions.LateUpdate)
                Follow();
        }

        private void Follow() {
            if (PositionTarget) {
                SetPosition();
            }
            if (RotationTarget) {
                SetRotation();
            }
        }

        private void SetRotation() {
            switch (_rotationOptions) {
                case RotationOptions.Rotation:
                    transform.rotation = RotationTarget.rotation;
                    break;
                case RotationOptions.XZRotation:
                    transform.rotation = Quaternion.Euler(RotationTarget.eulerAngles.x, transform.eulerAngles.y, RotationTarget.eulerAngles.z);
                    break;
                case RotationOptions.YRotation:
                    transform.rotation = Quaternion.Euler(transform.eulerAngles.x, RotationTarget.eulerAngles.y, transform.eulerAngles.z);
                    break;
                case RotationOptions.NoRotation:
                    break;
            }
        }

        private void SetPosition() {
            Vector3 newPosition;

            var thisTransform = transform; // I think this is more efficient than repeatedly accessing transform
            switch (_positionOption) {
                case PositionOptions.Position:
                    newPosition = GetNewProvisionalPosition();
                    thisTransform.position = newPosition;
                    break;
                case PositionOptions.XZPosition:
                    newPosition = GetNewProvisionalPosition();
                    newPosition.y = transform.position.y;
                    thisTransform.position = newPosition;
                    break;
                case PositionOptions.YPosition:
                    newPosition = GetNewProvisionalPosition();
                    newPosition.x = thisTransform.position.x;
                    newPosition.z = thisTransform.position.z;
                    thisTransform.position = newPosition;
                    break;
                case PositionOptions.NoPosition:
                    break;
            }
        }

        private Vector3 GetNewProvisionalPosition() {
            return _offsetType switch {
                OffsetType.World => PositionTarget.position + _offsetPosition,
                OffsetType.RelativeToTargetRotation => PositionTarget.TransformPoint(_offsetPosition),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}