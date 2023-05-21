using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using NaughtyAttributes;
#endif

namespace ScottEwing.TriggersV2{
    public class CombinedTrigger : BaseTrigger, ILookInteractable{
        private enum TriggerType{
            Base,
            Touch,
            Interact,
            ToggleInteract,
            Checkpoint, 
            Look,
            LookInteract,
            TimedStay,
            Audio,
            Counter
        }
        
#if ODIN_INSPECTOR
        [PropertyOrder(-1)]
#endif
        [SerializeField] 
        private TriggerType _triggerType = TriggerType.Base;
#if ODIN_INSPECTOR
        [PropertyOrder(-1)]
#endif
        [ShowIf("_triggerType", TriggerType.Interact)]
        [SerializeField] private InteractTriggerData _interactTriggerData;
#if ODIN_INSPECTOR
        [PropertyOrder(-1)]
#endif
        [ShowIf("_triggerType", TriggerType.ToggleInteract)]
        [SerializeField] private ToggleInteractTriggerData _toggleInteractTriggerData;
#if ODIN_INSPECTOR
        [PropertyOrder(-1)]
#endif
        [ShowIf("_triggerType", TriggerType.Checkpoint)]
        [SerializeField] private CheckpointTouchTriggerData _checkpointTouchTriggerData;
#if ODIN_INSPECTOR
        [PropertyOrder(-1)]
#endif
        [ShowIf("_triggerType", TriggerType.Look)]
        [SerializeField] private LookTriggerData _lookTriggerData;
#if ODIN_INSPECTOR
        [PropertyOrder(-1)]
#endif
        [ShowIf("_triggerType", TriggerType.LookInteract)]
        [SerializeField] private LookInteractTriggerData _lookInteractTriggerData;
#if ODIN_INSPECTOR
        [PropertyOrder(-1)]
#endif
        [ShowIf("_triggerType", TriggerType.TimedStay)]
        [SerializeField] private TimedStayTriggerData _timedStayTriggerData;
#if ODIN_INSPECTOR
        [PropertyOrder(-1)]
#endif
        [ShowIf("_triggerType", TriggerType.Counter)]
        [SerializeField] private CounterTriggerData _counterTriggerData;


        protected override void InitialiseTrigger() {
            if (_trigger != null) {
                return;
            }

            _trigger = _triggerType switch {
                TriggerType.Base => new BaseTriggerType(this),
                TriggerType.Touch => new TouchTrigger(this),
                TriggerType.Interact => new InteractTrigger(this, _interactTriggerData),
                TriggerType.ToggleInteract => new ToggleInteractTrigger(this, _toggleInteractTriggerData),
                TriggerType.Checkpoint => new CheckpointTouchTrigger(this, _checkpointTouchTriggerData),
                TriggerType.TimedStay => new TimedStayTrigger(this, _timedStayTriggerData),
                TriggerType.Audio => new AudioTouchTrigger(this),
                TriggerType.Counter => new CounterTrigger(this, _counterTriggerData),
                TriggerType.Look => new LookTrigger(this, _lookTriggerData),
                TriggerType.LookInteract => new LookInteractTrigger(this, _lookInteractTriggerData),
                _ => _trigger
            };
        }

        protected override void Awake() {
            InitialiseTrigger();
            base.Awake();
        }


        private void OnValidate() {
            if (Application.isEditor && Application.isPlaying && gameObject.activeInHierarchy) {
                print("OnValidate");
                _trigger = null;
                InitialiseTrigger();
            }
        }
        //USE ENUM CHECK INSTEAD POLyMORPH Check Not Working
        public TriggerState Look(Collider other, bool localCast = false) {
            return _triggerType == TriggerType.Look || _triggerType == TriggerType.LookInteract ? ((LookTrigger)_trigger).Look(other) : TriggerState.None;
        }
        
        
    }
}