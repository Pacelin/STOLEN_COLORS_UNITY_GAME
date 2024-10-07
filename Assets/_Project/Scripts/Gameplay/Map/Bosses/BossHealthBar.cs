using Gameplay.Map.Bosses;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gameplay.Map.Spawn
{
    public class BossHealthBar : MonoBehaviour
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private TMP_Text _healthText;
        [SerializeField] private Image _fill;
        [SerializeField] private WaveState _waveState;

        [Inject] private BossReference _boss;

        private CompositeDisposable _disposables;
        
        private void OnEnable()
        {
            _disposables = new();
            _root.SetActive(false);
            
            _boss.OnBossActivated.Subscribe(_ =>
            {
                _waveState.gameObject.SetActive(false);
                _root.SetActive(true);
            }).AddTo(_disposables);
            _boss.Boss.Model.Health.Subscribe(h =>
            {
                _healthText.text = $"{h:0}/{_boss.Boss.Model.MaxHealth:0}";
                _fill.fillAmount = h / _boss.Boss.Model.MaxHealth;
            }).AddTo(_disposables);
            _boss.Boss.Model.OnDie.Subscribe(_ =>
            {
                gameObject.SetActive(false);
            }).AddTo(_disposables);
        }

        private void OnDisable()
        {
            _disposables.Dispose();
        }
    }
}