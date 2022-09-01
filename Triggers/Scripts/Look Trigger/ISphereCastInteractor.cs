using UnityEngine;

namespace ScottEwing.Triggers{
    public interface ISphereCastInteractor{
        float SphereCastRadius { get; set; }
        LayerMask CollisionLayers { get; set; }
        QueryTriggerInteraction TriggerInteraction { get; set; }

        CastSourceType CastSourceType { get; set; }
        Transform AssignedSource { get; set; }
        Transform CurrentSource { get; set; }
    }
}
