
namespace ScottEwing.EventSystem{
    public static class Events{
        public static Template TemplateEvent = new Template();

#if SE_CHECKPOINTS
        public static CheckpointEvents checkpointEvents = new CheckpointEvents();
#endif

        //--Pause
        public static GamePausedEvent gamePausedEvent = new GamePausedEvent();
        public static GameResumedEvent gameResumedEvent = new GameResumedEvent();
    }
    
    public class Template : GameEvent{

    }
    
    //--Pause Events
    public class GamePausedEvent : GameEvent{
        
    }
    public class GameResumedEvent : GameEvent{
        public float cameraSensitivity;
        public bool invertLengthTriggers;
        public bool invertControllerYAxis;

    }
}