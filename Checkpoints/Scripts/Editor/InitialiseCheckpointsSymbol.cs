using System;
using System.Collections.Generic;
using UnityEditor;

namespace ScottEwing.Checkpoints
{
#if UNITY_EDITOR
    [InitializeOnLoad]
    public class InitialiseCheckpointsSymbol
    {
        private static BuildTargetGroup[] supportedBuildTargetGroups =
        {
            BuildTargetGroup.Standalone,
            BuildTargetGroup.Android,
            BuildTargetGroup.iOS
        };

        static InitialiseCheckpointsSymbol()
        {

            foreach(var group in supportedBuildTargetGroups)
            {
                var defines = GetDefinesList(group);
                if (!defines.Contains("SE_CHECKPOINTS"))
                {
                    defines.Add("SE_CHECKPOINTS");
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

