using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace UniOwl.UI
{
    public class UniProgressBar : MonoBehaviour
    {
        [SerializeField]
        private Image _fillMain, _fillTrailDecrease, _fillTrailIncrease;

        [SerializeField]
        private float _minValue, _maxValue;

        [SerializeField, HideInInspector]
        private float _currentTrailValue;
        [SerializeField]
        private float _currentValue;

        [SerializeField]
        private float _trailDelay, _trailTime;

        [SerializeField]
        private UnityEvent<float> _valueChanged;

        private Tween _tween;

        private void Awake()
        {
            _currentTrailValue = _currentValue;
        }

        public void SetValue(in float value)
        {
            _currentValue = Mathf.Clamp(value, _minValue, _maxValue);

            float delta = _currentTrailValue - _currentValue;
            float trailDuration = Mathf.Abs(delta / (_maxValue - _minValue) * _trailTime);
            
            Image fill = delta > 0f ? _fillMain : _fillTrailIncrease;
            Image trail = delta > 0f ? _fillTrailDecrease : _fillMain;
            Image otherTrail = delta > 0f ? _fillTrailIncrease : _fillTrailDecrease; 
            
            _tween?.Kill();
            _tween = DOTween.
                     Sequence().
                     AppendInterval(_trailDelay).
                     Append(DOVirtual.Float(_currentTrailValue, _currentValue, trailDuration, value => _currentTrailValue = value)).
                     Join(trail.DOFillAmount(Mathf.InverseLerp(_minValue, _maxValue, _currentValue), trailDuration)).
                     SetUpdate(true);

            fill.fillAmount = Mathf.InverseLerp(_minValue, _maxValue, _currentValue);
            otherTrail.fillAmount = fill.fillAmount;
            
            _valueChanged?.Invoke(value);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                SetValue(_currentValue - Random.Range(0, 25));
            if (Input.GetKeyDown(KeyCode.R))
                SetValue(_currentValue + Random.Range(0, 25));
        }

        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
                return;
            
            _fillMain.fillAmount = Mathf.InverseLerp(_minValue, _maxValue, _currentValue);
            _fillTrailDecrease.fillAmount = _fillMain.fillAmount;
            _fillTrailIncrease.fillAmount = _fillMain.fillAmount;
        }
        #endif
    }
}
