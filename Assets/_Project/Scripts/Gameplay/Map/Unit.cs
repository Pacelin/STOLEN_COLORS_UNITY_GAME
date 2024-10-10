using _Project.Scripts.Gameplay.Map.Effects;
using Gameplay.Map.Allies;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Gameplay.Map
{
    public class Unit : MonoBehaviour
    {
        public Vector3 Position => transform.position;
        public EBattleSide Side => _side;

        public UnitData BaseData => _baseData;
        public NavMeshAgent Agent => _agent;
        public UnitModel Model => _model;        
        public EWarriorClass Class => _class;
        public Vector3 DamagePosition => _damagePosition.position;

        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private EBattleSide _side;
        [SerializeField] private EWarriorClass _class;
        [SerializeField] private UnitData _baseData;
        [SerializeField] private Transform _damagePosition;
        [SerializeField] private ParticleSystem _damageEffect;

        private UnitModel _model;
        [Inject] private DamageEffectPool _effectPool;

        [Inject]
        private void Construct(DiContainer container, [InjectOptional] SpawnModifiers unitModifiers)
        {
            if (unitModifiers == null)
            {
                unitModifiers = new()
                {
                    WalkSpeed = 0,
                    AttackSpeedMultiplier = 1,
                    MagesAttackRange = 0,
                    DamageMultiplier = 1,
                    HealthMultiplier = 1,
                };
            }
            _model = new UnitModel(_baseData, unitModifiers, _class);
            Agent.speed = _model.Speed;
        }

        public void UpdateModifiers(SpawnModifiers constantModifiers)
        {
            _model.UpdateStats(constantModifiers);
            Agent.speed = _model.Speed;
        }
        
        public virtual void TakeDamage(float damage)
        {
            if (!_model.Alive.Value)
                return;
            _model.TakeDamage(damage);
            _damageEffect.Play();
            _effectPool.ApplyEffect(this, damage).Forget();
        }
    }
}