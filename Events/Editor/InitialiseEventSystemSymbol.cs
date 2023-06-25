using System;
using System.Collections.Generic;
using UnityEditor;

namespace ScottEwing.EventSystem
{
#if UNITY_EDITOR
    [InitializeOnLoad]
    public class InitialiseEventSystemSymbol
    {
        private static BuildTargetGroup[] supportedBuildTargetGroups =
        {
            BuildTargetGroup.Standalone,
            BuildTargetGroup.Android,
            BuildTargetGroup.iOS
        };

        static InitialiseEventSystemSymbol()
        {

            foreach(var group in supportedBuildTargetGroups)
            {
                var defines = GetDefinesList(group);
                if (!defines.Contains("SE_EVENTSYSTEM"))
                {
                    defines.Add("SE_EVENTSYSTEM");
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", defines.ToArray()));
                }
            }
        }

        private static List<string> GetDefinesList(BuildTargetGroup group)
        {
            return new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';'));
        }
    }
#endif

}