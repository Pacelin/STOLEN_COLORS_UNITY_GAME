using Gameplay.Map.Bosses;
using UniRx;
using UnityEngine;
using Zenject;

public class DesaturationMaskController : MonoBehaviour
{
    [Inject]
    private BossReference _boss;

    private CompositeDisposable _disposable;

    [SerializeField]
    private DesaturationMaskView _view;

    [SerializeField]
    private int _level;
    
    private void OnEnable()
    {
        _disposable = new CompositeDisposable();
        
        _boss.Boss.Model.Alive.Subscribe(alive =>
        {
            if (!alive)
                StartPrismSequence();
        }).AddTo(_disposable);
        
        _view.ApplyDefaultLevel(_level);
    }

    private void OnDisable()
    {
        _disposable.Dispose();
    }

    private void StartPrismSequence()
    {
        _view.ExpandPrism();
    }
}
