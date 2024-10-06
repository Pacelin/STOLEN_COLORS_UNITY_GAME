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

        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private EBattleSide _side;
        [SerializeField] private UnitData _baseData;

        private UnitModel _model;
        
        [Inject]
        private void Construct(DiContainer container, [InjectOptional] UnitModifiers unitModifiers) =>
            _model = new UnitModel(_baseData, unitModifiers);
    }
}