using DG.Tweening;
using UnityEngine;

public class DesaturationMaskController : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _background, _mask, _glow;

    [SerializeField]
    private Color _glowStartColor;
    
    [SerializeField]
    private Transform _target;
    
    [SerializeField]
    private float _finalMaskScale = 200f;
    [SerializeField]
    private float _scaleTime = 1f;
    [SerializeField]
    private float _glowFadeTime = .4f;
    [SerializeField]
    private float _expandDelay = 1f;

    private Tween _tween;

    private void OnDestroy()
    {
        _tween?.Kill();
    }

    private int test;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ApplyDefaultLevel(test);
            ExpandPrism();
            test++;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            test--;
            ApplyDefaultLevel(test);
            ShrinkPrism();
        }
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public void ApplyDefaultLevel(int level)
    {
        float alpha = level * 0.25f + 1e-3f;
        _background.color = new Color(1f, 1f, 1f, alpha);
    }

    public void ShrinkPrism()
    {
        _tween?.Kill();

        _mask.transform.position = _target.position;
        _mask.transform.localScale = Vector3.one * _finalMaskScale;
        _tween = _mask.transform.DOScale(Vector3.zero, _scaleTime);
    }
    
    public void ExpandPrism()
    {
        _tween?.Kill();

        _mask.transform.position = _target.position;
        _mask.transform.localScale = Vector3.zero;
        _glow.color = _glowStartColor;
        
        _tween = DOTween.
                 Sequence().
                 AppendInterval(_expandDelay).
                 Append(_mask.transform.DOScale(Vector3.one * _finalMaskScale, _scaleTime)).
                 Join(_glow.DOColor(Color.clear, _glowFadeTime)).
                 SetEase(Ease.OutFlash);
    }
}
