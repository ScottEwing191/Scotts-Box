using UnityEngine;

namespace ScottEwing.Checkpoints{
    public class CheckpointDemo : MonoBehaviour{
        private void Update() {
            if (UnityEngine.Input.GetKeyDown(KeyCode.R)) {
                CheckpointManager.Instance.ReloadCheckpoint();
            }
        }
    }
}
