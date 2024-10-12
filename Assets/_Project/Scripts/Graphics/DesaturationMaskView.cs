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
    private float _maskExpandedAlpha = 0.25f;

    [SerializeField]
    private Color _glowFlashColor;
    
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
    public void ExpandPrismDelay()
    {
        if (expanded)
            return;
        expanded = true;
        
        _audioSystem.PlaySound(ESoundKey.Victory);

        _tween?.Kill();
        _tween = DOTween.
                 Sequence().
                 AppendInterval(_expandDelay).
                 AppendCallback(ExpandPrism);
    }

    private void ExpandPrism()
    {
        _tween?.Kill();

        _mask.transform.position = _target.position;
        _glow.color = _glowFlashColor;

        Color finalMaskColor = _mask.color;
        finalMaskColor.a = _maskExpandedAlpha + 1e-3f;
        _mask.color = finalMaskColor;
        
        _tween = DOTween.
                 Sequence().
                 Append(_mask.transform.DOScale(Vector3.one * _finalMaskScale, _scaleTime)).
                 JoinCallback(_ps.Play).
                 Join(_glow.DOColor(Color.clear, _glowFadeTime)).
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
