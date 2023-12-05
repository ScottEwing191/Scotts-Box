using System;
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
        public static Dictionary<PlayerInput, ControlSchemeChangedData> controlsChanged = new Dictionary<PlayerInput,  ControlSchemeChangedData>();

        public Action<PlayerInput> OnControlSchemeChanged = delegate {  };
        private string _currentControlScheme = "";

        protected virtual void Awake() {
            if (_playerInput == null) {
                _playerInput = GetComponentInParent<PlayerInput>();
            }
        }

        protected virtual void OnEnable() {
            //OnControlSchemeChanged += OnControlsChanged;
            controlsChanged[_playerInput].OnControlSchemeChanged += OnControlsChanged;


        }

        protected virtual void OnDisable() {
            //OnControlSchemeChanged += OnControlsChanged;
            controlsChanged[_playerInput].OnControlSchemeChanged -= OnControlsChanged;

        }
        
        private void OnControlsChanged(PlayerInput obj) {
            print("Controls Changed");
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
        
        private void Update() {
            CheckIfControlSchemeChanged(_playerInput);
            /*if (_playerInput.currentControlScheme != _currentControlScheme) {
                _currentControlScheme = _playerInput.currentControlScheme;
                OnControlSchemeChanged?.Invoke(_playerInput);
            }*/
                
            
        }

        private void LateUpdate() => ResetBeenChecked(_playerInput);
        
        public void DisableActionMap() {
            if (_actionMap != null) {
                _actionMap.Disable();
            }
        }
        
        public void EnableActionMap() {
            if (_actionMap != null) {
                _actionMap.Enable();
            }
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
            controlsChanged.Add(playerInput, new ControlSchemeChangedData() {
                //PlayerInput = playerInput,
                BeenChecked = false,
                CurrentControlScheme = playerInput.currentControlScheme,
                OnControlSchemeChanged = delegate {  }
            });
            playerInput.user.AssociateActionsWithUser(controls);
            return controls;
        }

        private static void CheckIfControlSchemeChanged(PlayerInput playerInput) {
            if (controlsChanged.TryGetValue(playerInput, out var data)) {
                if (data.BeenChecked) {
                    return;
                }
                
                if (playerInput.currentControlScheme != data.CurrentControlScheme)
                {
                    controlsChanged[playerInput].CurrentControlScheme = playerInput.currentControlScheme;
                    controlsChanged[playerInput].OnControlSchemeChanged?.Invoke(playerInput);    
                        
                }
                controlsChanged[playerInput].BeenChecked = true;
            }
        }
        
        public static ControlSchemeChangedData GetControlSchemeChangedData(PlayerInput playerInput) {
            if (controlsChanged.TryGetValue(playerInput, out var data)) {
                return data;
            }
            return null;
        }

        private static void ResetBeenChecked(PlayerInput playerInput) {
            if (controlsChanged.TryGetValue(playerInput, out var data)) {
                controlsChanged[playerInput].BeenChecked = false;
            }
        }
        #endregion

    }

    public class ControlSchemeChangedData{
        //public PlayerInput PlayerInput;
        public bool BeenChecked;
        public string CurrentControlScheme;
        public Action<PlayerInput> OnControlSchemeChanged;
    }
}
