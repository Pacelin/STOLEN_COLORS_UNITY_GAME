using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UniOwl.UI
{
    public class Typewriter : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _text;

        [SerializeField]
        private TypewriterSettings _typeSettings;

        [SerializeField]
        private TypewriterAnimation _animation;
        
        private Sequence _sequence;

        private void OnDisable()
        {
            _sequence?.Kill();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
                TypeText("Lorem Ipsum Dolor Sit Amet Lorem Ipsum Dolor Sit Amet Lorem Ipsum Dolor Sit Amet");
        }

        public void TypeText(string text)
        {
            _text.SetText(string.Empty);
            
            if (_animation)
                _animation.ResetAnimations();
            
            _sequence?.Kill();
            _sequence = DOTween.Sequence();

            foreach (char ch in text)
                _sequence.
                    AppendCallback(() => AddCharacter(ch)).
                    AppendInterval(_typeSettings.TypeSpeed);
        }

        private void AddCharacter(char ch)
        {
            _text.SetText(_text.text + ch);

            if (_animation)
                _animation.AddCharAnimation(_text.text.Length - 1);
        }
    }
}