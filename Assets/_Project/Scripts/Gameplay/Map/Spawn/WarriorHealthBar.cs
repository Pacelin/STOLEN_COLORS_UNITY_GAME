using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.Map.Spawn
{
    public class WarriorHealthBar : MonoBehaviour
    {
        [SerializeField] private TMP_Text _healthText;
        [SerializeField] private Image _fill;
        [SerializeField] private Warrior _warrior;
        [SerializeField] private bool _showText;
        [SerializeField] private Color _allyColor;
        [SerializeField] private Color _enemyColor;

        private CompositeDisposable _disposables;
        
        private void OnEnable()
        {
            _disposables = new();
            if (_warrior.Side == EBattleSide.Ally)
                _fill.color = _allyColor;
            else
                _fill.color = _enemyColor;
            _healthText.gameObject.SetActive(_showText);
            _warrior.Model.Health
                .Where(_ => _warrior.Model.Alive.Value).Subscribe(h =>
                {
                    if (_showText)
                        _healthText.text = $"{h:0}/{_warrior.Model.MaxHealth:0}";
                    _fill.fillAmount = h / _warrior.Model.MaxHealth;
                }).AddTo(_disposables);
        }

        private void OnDisable()
        {
            _disposables.Dispose();
        }
    }
}