using Audio.Gameplay.PointsGrid;
using UniRx;
using UnityEngine;

namespace Audio.Tutorial
{
    public class TutorialEntryWaitForCast : TutorialEntryBase
    {
        [SerializeField]
        private GridPanel _gridPanel;

        private CompositeDisposable _disposable;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            _disposable = new CompositeDisposable();
            _gridPanel.OnApply.Subscribe(_ => _tutorial.NextEntry()).AddTo(_disposable);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _disposable.Dispose();
        }
    }
}