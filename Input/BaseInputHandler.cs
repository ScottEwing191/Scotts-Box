using System;
using System.Collections;
using System.Collections.Generic;
using ScottEwing.EventSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ScottEwing.Input{
    public abstract class BaseInputHandler : MonoBehaviour{
        protected PlayerInput _playerInput;
        protected InputActionAsset _actions;
        protected InputActionMap _actionMap;
        [SerializeField] protected string _actionMapName = "";
        [SerializeField] protected bool _enableActionMapOnStart = true;
        [SerializeField] private bool _disableOnPause = true;
        private bool _actionMapActiveWhenPaused;
        protected virtual void Awake() {
            _playerInput = GetComponentInParent<PlayerInput>();
            _actions = _playerInput.actions;
            _actionMap = _playerInput.actions.FindActionMap(_actionMapName);
            if (_enableActionMapOnStart) {
                _actionMap.Enable();
            }
        }

        protected virtual void Start() {
#if SE_EVENTSYSTEM
            EventManager.AddListener<GamePausedEvent>(DisableActionMapOnPause);
            EventManager.AddListener<GameResumedEvent>(EnableActionMapOnResume);
#endif
        }

        protected virtual void OnDestroy() {
#if SE_EVENTSYSTEM
            EventManager.RemoveListener<GamePausedEvent>(DisableActionMapOnPause);
            EventManager.RemoveListener<GameResumedEvent>(EnableActionMapOnResume);
#endif
        }

#if SE_EVENTSYSTEM
        private void DisableActionMapOnPause(GamePausedEvent obj) {
            _actionMapActiveWhenPaused = _actionMap.enabled;
            if (_actionMap.enabled && _disableOnPause) {
                _actionMap.Disable();
            }
        }

        private void EnableActionMapOnResume(GameResumedEvent obj) {
            if (_actionMapActiveWhenPaused && _disableOnPause) {
                _actionMap.Enable();
            }
        }
#endif

        public virtual void EnableActionMap() => _actionMap.Enable();
        public virtual void DisableActionMap() => _actionMap.Disable();
    }
}