using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniOwl.UI
{
    public class UniSpriteColorTransition : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private SpriteRenderer _renderer;
        
        [SerializeField] private Color _defaultColor;
        [SerializeField] private Color _hoverColor;
        [SerializeField] private Color _clickColor;

        [SerializeField] private float fadeDuration = .1f;

        private Tween _tween;

        private void OnEnable()
        {
            _renderer.color = _defaultColor;
        }

        private void OnDisable()
        {
            _tween?.Kill();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _tween?.Kill();
            _tween = _renderer.DOColor(_hoverColor, fadeDuration).SetUpdate(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _tween?.Kill();
            _tween = _renderer.DOColor(_defaultColor, fadeDuration).SetUpdate(true);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _tween?.Kill();
            _tween = _renderer.DOColor(_clickColor, fadeDuration).SetUpdate(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _tween?.Kill();
            _tween = _renderer.DOColor(_hoverColor, fadeDuration).SetUpdate(true);
        }
    }
}