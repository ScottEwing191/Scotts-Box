using System;
using System.Collections;
using System.Collections.Generic;
using ScottEwing.EventSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ScottEwing.Input
{
    public abstract class BaseInputHandlerV2 : MonoBehaviour
    {
        [SerializeField] protected PlayerInput _playerInput;
        protected InputActionMap _actionMap;
        
        [SerializeField] protected bool _enableActionMapOnStart = true;
        [SerializeField] private bool _disableOnPause = true;
        private bool _actionMapActiveWhenPaused;

        public static Dictionary<PlayerInput, IInputActionCollection2> inputDictionary = new Dictionary<PlayerInput, IInputActionCollection2>();


        protected virtual void Awake() {
            if (_playerInput == null) {
                _playerInput = GetComponentInParent<PlayerInput>();
            }
        }
        
        

        protected virtual void Start() {
            if (_enableActionMapOnStart && _actionMap != null) {
                _actionMap.Enable();
            }
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
        
        
        #region Static Methods 
        public static T GetActionsAsset<T>(PlayerInput playerInput) where T : IInputActionCollection2 {
            if (inputDictionary.ContainsKey(playerInput)) {
                return (T)inputDictionary[playerInput];
            }
            var controls = (T)Activator.CreateInstance(typeof(T));
            inputDictionary.Add(playerInput, controls);
            playerInput.user.AssociateActionsWithUser(controls);
            return controls;
        }
        #endregion

    }
}
