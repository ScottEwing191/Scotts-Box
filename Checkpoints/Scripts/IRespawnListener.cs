using System.Collections;
using System.Collections.Generic;
using ScottEwing.Checkpoints;
using UnityEngine;

namespace ScottEwing
{
    /// <summary>
    /// Listener for the Respawnable Object Respawning
    /// </summary>
    public interface IRespawnListener{
        public bool IsRespawnable { get; set; }
        public RespawnableObject RespawnObject { get; set; }
        public void OnRespawn();
    }
}
