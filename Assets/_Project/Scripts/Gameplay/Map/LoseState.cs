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
                        .AppendCallback(() => _audio.PlaySound(ESoundKey.GameOver))
                        .Append(_msg.DOFade(1, 2f))
                        .AppendInterval(2)
                        .AppendCallback(() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));
                });
        }

        public void Dispose()
        {
            _tween?.Kill();
            _stateDisposable.Dispose();
        }
    }
}