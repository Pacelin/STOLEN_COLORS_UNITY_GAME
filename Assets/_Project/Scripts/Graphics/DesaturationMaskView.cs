using Audio;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class DesaturationMaskView : MonoBehaviour
{
    [Inject]
    private AudioSystem _audioSystem;
    
    [SerializeField]
    private SpriteRenderer _background, _mask, _glow;
    [SerializeField]
    private ParticleSystem _ps;
    [SerializeField]
    private SpriteRenderer _orb;

    [SerializeField, Range(0f, 1f)]
    private float _maskIdleAlpha = 1f, _maskExpandedAlpha = 0.25f;

    [SerializeField]
    private Color _glowStartColor;
    
    [SerializeField]
    private Transform _target;
    
    [SerializeField]
    private float _initialMaskScale = 2f, _finalMaskScale = 50f;
    [SerializeField]
    private float _scaleTime = 1f;
    [SerializeField]
    private float _glowFadeTime = .4f;
    [SerializeField]
    private float _expandDelay = 1f;
    [SerializeField]
    private float _winDelay = 2f;
    [SerializeField]
    private Vector3 _maskRotationSpeed = new Vector3(0f, 30f, 0);

    [SerializeField]
    private int _initialSaturationlevel;
    [SerializeField]
    private int _nextLevelBuildIndex;
    
    private Tween _tween;

    private void OnEnable()
    {
        _mask.transform.localScale = Vector3.one * _initialMaskScale;
        
        Color maskColor = _mask.color;
        maskColor.a = _maskIdleAlpha + 1e-3f;
        _mask.color = maskColor;
        
        ApplyDefaultLevel(_initialSaturationlevel);
    }

    private void OnDestroy()
    {
        _tween?.Kill();
    }

    private void Update()
    {
        _mask.transform.Rotate(_maskRotationSpeed * Time.deltaTime);
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

    private bool expanded;
    
    [ContextMenu("Expand")]
    public void ExpandPrism()
    {
        if (expanded)
            return;
        
        _audioSystem.PlaySound(ESoundKey.Victory);

        expanded = true;
        
        _tween?.Kill();

        _mask.transform.position = _target.position;
        _mask.transform.localScale = Vector3.zero;
        _glow.color = _glowStartColor;

        Color maskColor = _mask.color;
        maskColor.a = _maskIdleAlpha + 1e-3f;
        _mask.color = maskColor;

        Color finalMaskColor = maskColor;
        finalMaskColor.a = _maskExpandedAlpha + 1e-3f;
        
        _tween = DOTween.
                 Sequence().
                 AppendInterval(_expandDelay).
                 Append(_mask.transform.DOScale(Vector3.one * _finalMaskScale, _scaleTime)).
                 JoinCallback(_ps.Play).
                 Join(_glow.DOColor(Color.clear, _glowFadeTime)).
                 Join(_mask.DOColor(finalMaskColor, _glowFadeTime)).
                 Join(_orb.DOColor(Color.clear, _glowFadeTime)).
                 AppendInterval(_winDelay).
                 AppendCallback(AfterAnimationCallback).
                 SetEase(Ease.OutFlash);
    }

    private void AfterAnimationCallback()
    {
        SceneManager.LoadScene(_nextLevelBuildIndex);
    }
}
