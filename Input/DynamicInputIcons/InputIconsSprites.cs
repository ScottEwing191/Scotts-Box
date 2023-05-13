using System;
using System.Collections.Generic;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace ScottEwing.Input.DynamicInputIcons{

    [CreateAssetMenu]
    public class InputIconsSprites 
#if ODIN_INSPECTOR
        : SerializedScriptableObject
#endif
    
    {
#if ODIN_INSPECTOR
        [DictionaryDrawerSettings(KeyLabel = "Input", ValueLabel = "Input Icon")]
#endif
        public Dictionary<string, Sprite> InputIcons = new Dictionary<string, Sprite>() {
            {"buttonSouth", null},
            {"buttonNorth", null},
            {"buttonEast", null},
            {"buttonWest", null},
            {"start", null},
            {"select", null},
            {"leftTrigger", null},
            {"rightTrigger", null},
            {"leftShoulder", null},
            {"rightShoulder", null},
            {"dpad", null},
            {"dpad/up", null},
            {"dpad/down", null},
            {"dpad/left", null},
            {"dpad/right", null},
            {"leftStick", null},
            {"rightStick", null},
            {"leftStickPress", null},
            {"rightStickPress", null}
        };
    
        public Sprite GetSprite(string key) => InputIcons[key];
    }

    
    /// <summary>
    /// KeyValuePair can be used instead of dictionary if Odin Inspector is not available to serialize the dictionary
    /// </summary>
    /*[Serializable]
    public class KeyValuePain{
        [SerializeField] private string _input;
        [SerializeField] private Sprite _inputIcon;
    }*/
}
