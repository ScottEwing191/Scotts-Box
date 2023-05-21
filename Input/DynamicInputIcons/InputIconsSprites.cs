using System;
using UnityEngine;

namespace ScottEwing.Input.DynamicInputIcons{
    [CreateAssetMenu]
    public class InputIconsSprites : ScriptableObject{
        [SerializeField] public KeyValuePair[] InputIcons3 = new KeyValuePair[] {
            new("buttonSouth", null),
            new("buttonNorth", null),
            new("buttonEast", null),
            new("buttonWest", null),
            new("start", null),
            new("select", null),
            new("leftTrigger", null),
            new("rightTrigger", null),
            new("leftShoulder", null),
            new("rightShoulder", null),
            new("dpad", null),
            new("dpad/up", null),
            new("dpad/down", null),
            new("dpad/left", null),
            new("dpad/right", null),
            new("leftStick", null),
            new("rightStick", null),
            new("leftStickPress", null),
            new("rightStickPress", null)
        };


        //public Sprite GetSprite(string key) => InputIcons[key];
        public Sprite GetSprite(string key) {
            foreach (var kvp in InputIcons3) {
                if (kvp._input == key) {
                    return kvp._inputIcon;
                }
            }
            return null;
        }
    }
    
    /// <summary>
    /// KeyValuePair can be used instead of dictionary if Odin Inspector is not available to serialize the dictionary
    /// </summary>
    [Serializable]
    public class KeyValuePair{
        [SerializeField] public string _input;
        [SerializeField] public Sprite _inputIcon;

        public KeyValuePair(string input, Sprite inputIcon) {
            _input = input;
            _inputIcon = inputIcon;
        }
    }
}