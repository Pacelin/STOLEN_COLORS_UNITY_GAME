using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.Map;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Map.Effects
{
    public class DamageEffect : MonoBehaviour
    {
        [SerializeField] private Vector3 _initialOffset;
        [SerializeField] private Vector3 _animateDirection;
        [SerializeField] private float _duration;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Color _allyColor;
        [SerializeField] private Color _enemyColor;
        
        private Tween _tween;

        public async UniTask Show(Unit warrior, int amount)
        {
            if (!warrior)
                return;
            var transform1 = transform;
            transform1.position = warrior.Position + _initialOffset;
            _text.color = warrior.Side == EBattleSide.Ally ? _allyColor : _enemyColor;
            _text.alpha = 1;
            _text.text = amount.ToString();
            gameObject.SetActive(true);
            _tween = DOTween.Sequence()
                .Append(transform.DOBlendableLocalMoveBy(_animateDirection, _duration))
                .Join(_text.DOFade(0, _duration)).SetEase(Ease.InFlash);
            await _tween;
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            _tween?.Kill();
        }
    }
}