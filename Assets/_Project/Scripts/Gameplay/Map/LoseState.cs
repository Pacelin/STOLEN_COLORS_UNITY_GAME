using System;
using DG.Tweening;
using Gameplay.Map;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Audio.Gameplay
{
    public class LoseState : IInitializable, IDisposable
    {
        [Inject] private GameStateManager _gameState;
        [Inject] private CanvasGroup _msg;
        [Inject] private AudioSystem _audio;
        
        private Tween _tween;
        private IDisposable _stateDisposable;
        
        public void Initialize()
        {
            _stateDisposable = _gameState.State.Where(s => s == GameStateManager.EState.Lose)
                .Subscribe(_ =>
                {
                    _msg.alpha = 0;
                    _msg.gameObject.SetActive(true);
                    _tween = DOTween.Sequence()
                        .Append(_msg.DOFade(1, 2f))
                        .AppendCallback(() => _audio.PlaySound(ESoundKey.GameOver))
                        .AppendInterval(2)
                        .AppendCallback(() => SceneManager.LoadScene(0));
                });
        }

        public void Dispose()
        {
            _tween?.Kill();
            _stateDisposable.Dispose();
        }
    }
}