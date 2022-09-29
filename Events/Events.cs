namespace ScottEwing.EventSystem{
    // The Game Events used across the Game.
    // Anytime there is a need for a new event, it should be added here.
    public static class Events{
        //== Game Events
        public static Template TemplateEvent = new Template();
    }

    public class Template : GameEvent{

    }
    
}