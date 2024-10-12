using UnityEngine;
using UnityEngine.UI;

namespace Audio.Tutorial
{
    public class TutorialEntryWithButton : TutorialEntryBase
    {
        [SerializeField]
        private Button _nextButton;

        protected override void OnEnable()
        {
            base.OnEnable();
            _nextButton.onClick.AddListener(_tutorial.NextEntry);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _nextButton.onClick.RemoveListener(_tutorial.NextEntry);
        }
    }
}