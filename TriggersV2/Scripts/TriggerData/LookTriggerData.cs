using System;
using System.ComponentModel;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace ScottEwing.TriggersV2{
    [Serializable]
    public class LookTriggerData: ITriggerData{
        [Serializable]
        internal struct LocalRaycastData{
            [SerializeField] public RayCastHelper _castHelper;

            [Tooltip("The game object that need to be looked at. Default is the game object this is attached to. If different, default and _lookTarget should be under same parent")]
            [SerializeField] public Transform _lookTargetRoot;

            [Description("Is true while inside collider with defined tag or layer mask")]
            [HideInInspector] public bool _castForTrigger;
        }
        
        /*[Description("")]
        public enum CastType{
            GlobalCastInteractor,
            LocalRaycastHelper,
        }*/

        [SerializeField] internal LookTrigger.CastType _castType = LookTrigger.CastType.GlobalCastInteractor;

#if ODIN_INSPECTOR
        [ShowIf("_castType", LookTrigger.CastType.LocalRaycastHelper)]
#endif
        [SerializeField]
        internal LocalRaycastData _castData;

    }
}