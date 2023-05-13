using UnityEngine;

namespace ScottEwing.EventSystem{
    // The Game Events used across the Game.
    // Anytime there is a need for a new event, it should be added here.
    public static class Events{
        //== Game Events
        public static Template TemplateEvent = new Template();

#if SE_CHECKPOINTS
        public static CheckpointEvents checkpointEvents = new CheckpointEvents();
#endif

        
        //==Player Events

        //--Hand Events
        public static LookAtRcpUpdatedEvent lookAtRcpUpdatedEvent = new LookAtRcpUpdatedEvent();
        public static LookAtRcpStoppedEvent lookAtRcpStoppedEvent = new LookAtRcpStoppedEvent();
        public static ControlPointSelectedToHandEvent controlPointSelectedToHandEvent = new ControlPointSelectedToHandEvent();
        public static RcpFiredEvent rcpFiredEvent = new RcpFiredEvent();
        public static RopeDeactivatedFromHandEvent ropeDeactivatedFromHandEvent = new RopeDeactivatedFromHandEvent();

        public static RopeCreationFailedEvent ropeCreationFailedEvent = new RopeCreationFailedEvent();

        public static FireRcpFailedEvent fireRcpFailedEvent = new FireRcpFailedEvent();

        
        public static DeactivatedAllRopesEvent deactivatedAllRopes = new DeactivatedAllRopesEvent();
        
        public static ChangeLengthPriorityUpdatedEvent changeLengthPriorityUpdated = new ChangeLengthPriorityUpdatedEvent();
        public static ChangingLengthEvent changingLengthEvent = new ChangingLengthEvent();


        
        //--RopeBelt
        /*public static RopeAddedToBeltEvent ropeAddedToBelt = new RopeAddedToBeltEvent();
        public static RopeTakenFromBeltEvent ropeTakenFromBelt = new RopeTakenFromBeltEvent();
        public static NextBeltSlotSelectedEvent nextBeltSlotSelected = new NextBeltSlotSelectedEvent();
        public static DeactivatedSelectedRopeInBeltEvent deactivatedSelectedRopeInBelt = new DeactivatedSelectedRopeInBeltEvent();
        public static DeactivatedAllRopesInBeltEvent deactivatedAllRopesInBelt = new DeactivatedAllRopesInBeltEvent();*/

        
        //--Range Control
        /*public static SingleRangePointSelectedEvent singleRangePointSelectedEvent = new SingleRangePointSelectedEvent();
        public static SingleRangePointDeselectedEvent singleRangePointDeselectedEvent = new SingleRangePointDeselectedEvent();
        public static MultiRangePointAddedEvent multiRangePointAddedEvent = new MultiRangePointAddedEvent();
        public static MultiRangePointRemovedEvent multiRangePointRemovedEvent = new MultiRangePointRemovedEvent();
        public static MultiRangeSelectListClearedEvent multiRangeSelectListClearedEvent = new MultiRangeSelectListClearedEvent();
        //public static ToggleRangeControlModeEvent toggleRangeControlModeEvent = new ToggleRangeControlModeEvent();
        public static SingleRangeSelectModeEvent singleRangeSelectModeEvent = new SingleRangeSelectModeEvent();
        public static MultiRangeSelectModeEvent multiRangeSelectModeEvent = new MultiRangeSelectModeEvent();*/

        //--Remote Control
        public static RemoteShootRopeAimStartedEvent remoteShootRopeAimStartedEvent = new RemoteShootRopeAimStartedEvent();
        public static RemoteShootRopePerformedEvent remoteShootRopePerformedEvent = new RemoteShootRopePerformedEvent();
        public static RemoteFireRcpAimStartedEvent remoteFireRcpAimStartedEvent = new RemoteFireRcpAimStartedEvent();
        public static RemoteFireRcpPerformedEvent remoteFireRcpPerformedEvent = new RemoteFireRcpPerformedEvent();
        public static CancelRemoteAimEvent cancelRemoteAimEvent = new CancelRemoteAimEvent();
        
        //--Pause
        public static GamePausedEvent gamePausedEvent = new GamePausedEvent();
        public static GameResumedEvent gameResumedEvent = new GameResumedEvent();
        
        //--Rope Limit
        public static CantExceedMaxRopeLimitEvent cantExceedMaxRopeLimitEvent = new CantExceedMaxRopeLimitEvent();

        //-- Info Log
        public static DisplayInfoLogEvent displayInfoLogEvent = new DisplayInfoLogEvent();
        
        //--RopeGeneration
        public static RopeGenerationStartedEvent ropeGenerationStartedEvent = new RopeGenerationStartedEvent();
    }
    

    public class Template : GameEvent{

    }
    
    public class LookAtRcpUpdatedEvent : GameEvent{
        public bool canPickUp;
        public bool canDeactivate;
        public bool canRemoveRcp;
        public bool canRemoteShootRope;
        public bool canRemoteFireRcp;
        //public bool canCancelRemoteAim;
        public RangeControlEvent rangeControlEvent;

    }
    public class LookAtRcpStoppedEvent : GameEvent{
    }

    public class HandWaitFinishedEvent : GameEvent{
        public RopeBeltEvent ropeBeltData;
        public RangeControlEvent rangeControlEvent;
    }
    
    public class RopeCreatedEvent : GameEvent{
        public bool createdFromHand;
        public RopeBeltEvent ropeBeltData;
    }
    public class RopeCreationFailedEvent : GameEvent{
    }
    
    public class RopeDeactivatedFromHandEvent : GameEvent{
        public RopeBeltEvent ropeBeltData;
        /*public bool deactivatedFromHand;
        public GameObject rcpObject;
        public bool wasOtherRcpInHand;
        public GameObject otherRcpObject;
        public RopeBeltEvent ropeBeltData;*/
    }
    public class RcpFiredEvent : GameEvent{
        public bool firedFromHand;
        public RopeBeltEvent ropeBeltData;
    }
    public class FireRcpFailedEvent : GameEvent{
    }
    

    public class RopeToggleOptionChangedEvent : GameEvent{
        public bool? activateOrDeactivateRope;          // true: Activate, false: Deactivate, null: Neither
    }
    
    public class CanFireControlPointStateChangedEvent : GameEvent{
        public bool canFireRope;          
    }

    public class ControlPointSelectedToHandEvent : GameEvent{
        
    }
    
    public class ChangeLengthPriorityUpdatedEvent : GameEvent{
        //public RopeLengthControl.ChangeLengthPriority lengthPriority;
        public int lengthPriority;
    }

    public class ChangingLengthEvent : GameEvent{
        public GameObject ropeControlPointObject;
        public bool shouldIncreaseLength;
        public float directionAndInput;
        public bool isChoppy;
        public bool slowSpeed;
    }
    
    public class DeactivatedAllRopesEvent : GameEvent{
        public RopeBeltEvent ropeBeltData;
    }
    
    //== BELT
    public class RopeBeltEvent : GameEvent{
        public bool beltFull;
        public bool beltEmpty;
        public bool canAddRope;
        public bool canTakeRope;
        public bool isSelectedSlotFilled;

        public RopeBeltEvent() {
        }

        public RopeBeltEvent(bool beltEmpty) {
            this.beltEmpty = beltEmpty;
        }
    }
    public class RopeAddedToBeltEvent : RopeBeltEvent{
    }
    public class RopeTakenFromBeltEvent : RopeBeltEvent{
    }
    public class NextBeltSlotSelectedEvent : RopeBeltEvent{
    }
    public class DeactivatedSelectedRopeInBeltEvent : RopeBeltEvent{
    }
    public class DeactivatedAllRopesInBeltEvent : RopeBeltEvent{
    }
    
    //==REMOTE CONTROLS
    public class RemoteControlEvent : GameEvent{
        public bool canRemoteShootRope;
        public bool canRemoteFireRcp;
        public bool canCancelRemoteCreateRope;
        public bool canCancelRemoteFireRcp;
    }
    
    public class RemoteShootRopeAimStartedEvent : RemoteControlEvent{
        public RemoteShootRopeAimStartedEvent() {
            canCancelRemoteCreateRope = true;
        }
    }
    public class RemoteShootRopePerformedEvent : RemoteControlEvent{
    }
    public class RemoteFireRcpAimStartedEvent : RemoteControlEvent{
        public RemoteFireRcpAimStartedEvent() {
            canCancelRemoteFireRcp = true;
        }
    }
    public class RemoteFireRcpPerformedEvent : RemoteControlEvent{
    }
    public class CancelRemoteAimEvent : RemoteControlEvent{
    }
    
    //==RANGE CONTROLS
    public class RangeControlEvent : GameEvent{
        public bool multiRangeSelectMode;
        public bool canRangeSelect;
        public bool canRangeDeselect;
        public bool canMultiRangeSelect;
        public bool canMultiRangeDeselect;
        public bool isSingleRangePointSelected;     // this can be true while in multi mode instead of single mode
        public int maxSelectedRopeCount;
        public int currentSelectedRopeCount;
        public GameObject singleSelectedObject;
        //public GameObject[] multiSelectedObject;

    }
    
    public class SingleRangePointSelectedEvent : RangeControlEvent{
    }
    public class SingleRangePointDeselectedEvent : RangeControlEvent{
    }

    public class MultiRangePointAddedEvent : RangeControlEvent{

    }public class MultiRangePointRemovedEvent : RangeControlEvent{
    }
    public class MultiRangeSelectListClearedEvent : RangeControlEvent{
    }
    public class SingleRangeSelectModeEvent : RangeControlEvent{
    }
    public class MultiRangeSelectModeEvent : RangeControlEvent{
    }
    
    public class GamePausedEvent : GameEvent{
        
    }
    public class GameResumedEvent : GameEvent{
        public float cameraSensitivity;
        public bool invertLengthTriggers;
    }
    public class CantExceedMaxRopeLimitEvent : GameEvent{
    }
    
    public class RopeGenerationStartedEvent : GameEvent{
        public GameObject sourceRcpGameObject;
    }
    
    public class RopeGeneratedEvent : GameEvent{
        public GameObject sourceRcpGameObject;
        public GameObject otherRcpGameObject;
        public int activeRopes;
        public int totalRopes;
    }
    public class RopeRemovedEvent : GameEvent{
        public int activeRopes;
        public int totalRopes;
    }
    
    public class DisplayInfoLogEvent : GameEvent{
        public string message;
    }
}
