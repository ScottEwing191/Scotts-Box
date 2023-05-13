using UnityEngine;
using UnityEngine.UI;

//==============================================================================================
//--Checkpoint Reset UI show a visual representation of the players input while holding the reset button.
//--The Reset UI will also be displayed if the player gets further away from the checkpoint to prompt
//--the player to use the Reset feature.
//==============================================================================================


namespace ScottEwing.Input {
    public class HoldInputUI : MonoBehaviour {
        [SerializeField] private Image _filledImage;
        private bool _isButtonHeld;
        private float _timer = 0.0f;
        private float _holdTime;

        //protected Player ThisPlayer;

        private void Awake() {
            //ThisPlayer = GetComponentInParent<Player>();
        }

        public virtual void StartButtonHold(float holdTime) {
            _filledImage.gameObject.SetActive(true);
            _timer = 0;
            _holdTime = holdTime;
            _isButtonHeld = true;
            _filledImage.fillAmount = 0;
        }
        public virtual void StopButtonHold() {
            _isButtonHeld = false;
            _filledImage.gameObject.SetActive(false);
        }

        public virtual void ShowHoldButtonUI() {
            _filledImage.gameObject.SetActive(true);
            _filledImage.fillAmount = 0;
        }
        public virtual void HideHoldButtonUI() { 
            _filledImage.gameObject.SetActive(false);
        }

        private void Update() {
            if (!_isButtonHeld) { return; }
            if (_timer > _holdTime) { return; }
            _timer += Time.deltaTime;
            _filledImage.fillAmount = Mathf.Lerp(0,1, _timer / _holdTime);
        }
    }
}
