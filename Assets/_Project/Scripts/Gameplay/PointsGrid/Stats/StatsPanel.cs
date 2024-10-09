using System;
using DG.Tweening;
using Gameplay.Map;
using Gameplay.Map.Allies;
using Gameplay.Map.Spawn;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace Audio.Gameplay.PointsGrid
{
    public class StatsPanel : MonoBehaviour
    {
        [System.Serializable]
        private class StatsRow
        {
            public TMP_Text Mage;
            public TMP_Text Warrior;
            public TMP_Text Tank;
        }
        
        [SerializeField] private float _closedX;
        [SerializeField] private float _openedX;
        [SerializeField] private float _duration;
        [SerializeField] private float _delay;
        [Space] 
        [SerializeField] private StatsRow _hpRow;
        [SerializeField] private StatsRow _dmgRow;
        [SerializeField] private StatsRow _atkspdRow;
        [SerializeField] private StatsRow _mvspdRow;
        [SerializeField] private StatsRow _rangeRow;

        [Inject] private WaveManager _wave;
        [Inject] private WarriorsConfig _baseStats;
        [Inject] private AlliesSpawner _allies;

        private CompositeDisposable _disposables;
        private Tween _tween;
        
        private void OnEnable()
        {
            _disposables = new();
            var transform1 = transform;
            var p = transform1.localPosition;
            p.x = _closedX;
            transform1.localPosition = p;
            _tween = DOTween.Sequence()
                .AppendInterval(_delay)
                .Append(transform.DOLocalMoveX(_openedX, _duration));
            _wave.WaveIsInProgress.Skip(1).Subscribe(b =>
            {
                if (b)
                    _tween = transform.DOLocalMoveX(_closedX, _duration);
                else
                    _tween = transform.DOLocalMoveX(_openedX, _duration);
            }).AddTo(_disposables);
            
            UpdateModifiers(_allies.ConstantModifiers);
            _allies.OnChangeModifiers.Subscribe(UpdateModifiers).AddTo(_disposables);
        }

        private void OnDisable()
        {
            _tween?.Kill();
            _disposables.Dispose();
        }

        private void UpdateModifiers(SpawnModifiers m)
        {
            var mage = _baseStats.GetAlly(EWarriorClass.Mage).BaseData;
            var tank = _baseStats.GetAlly(EWarriorClass.Tank).BaseData;
            var warrior = _baseStats.GetAlly(EWarriorClass.Soldier).BaseData;
            
            FillRowMultiplier(_hpRow, mage, tank, warrior, 
                1 + m.HealthMultiplier, d => d.Health);
            FillRowMultiplier(_dmgRow, mage, tank, warrior,
                1 + m.DamageMultiplier, d => d.Damage);
            FillRowMultiplier(_atkspdRow, mage, tank, warrior,
                1 + m.AttackSpeedMultiplier, d => d.AttackSpeed);
            FillRowAdd(_mvspdRow, mage, tank, warrior,
                m.WalkSpeed, d => d.Speed);
            _rangeRow.Mage.text = (mage.AttackDistance + m.MagesAttackRange).ToString("0.#");
            _rangeRow.Tank.text = tank.AttackDistance.ToString("0.#");
            _rangeRow.Warrior.text = warrior.AttackDistance.ToString("0.#");
        }

        private void FillRowMultiplier(StatsRow row, UnitData mage, UnitData tank, UnitData warrior, float multiplier, 
            Func<UnitData, float> getStatFunc)
        {
            row.Mage.text = (getStatFunc(mage) * multiplier).ToString("0.#");
            row.Tank.text = (getStatFunc(tank) * multiplier).ToString("0.#");
            row.Warrior.text = (getStatFunc(warrior) * multiplier).ToString("0.#");
        }
        private void FillRowAdd(StatsRow row, UnitData mage, UnitData tank, UnitData warrior, float add, 
            Func<UnitData, float> getStatFunc)
        {
            row.Mage.text = (getStatFunc(mage) + add).ToString("0.#");
            row.Tank.text = (getStatFunc(tank) + add).ToString("0.#");
            row.Warrior.text = (getStatFunc(warrior) + add).ToString("0.#");
        }
    }
}