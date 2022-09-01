Dynamic Input Icons
Dynamic input icons will change the sprite depnding on the current controller being used. 

Requirements
New Input System
Odin Inspector (can be easily modified to not need this)

PlayerInputIcons 
Attaches to parent of UiInputIcon game objects i.e a canvas. Needs Odin Inspector to display Dictionary in inspector. Could just make a list a simple KeyValuePair class instead.

UiInputIcon
Attach to game object of the UI element you want to change. Or just use the "Input Icon" prefab


InputIconSprite
Comes with three defaults (Keyboard, PS4, Xbox controller). To make more "right click in project -> Create -> Input Icons Sprites"  