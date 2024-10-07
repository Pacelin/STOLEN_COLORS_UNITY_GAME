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
        
        public NavMeshAgent Agent => _agent;
        public UnitModel Model => _model;        
        public EWarriorClass Class => _class;

        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private EBattleSide _side;
        [SerializeField] private EWarriorClass _class;
        [SerializeField] private UnitData _baseData;

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
        }

        public virtual void TakeDamage(float damage)
        {
            _model.TakeDamage(damage);
            _effectPool.ApplyEffect(this, damage).Forget();
        }
    }
}