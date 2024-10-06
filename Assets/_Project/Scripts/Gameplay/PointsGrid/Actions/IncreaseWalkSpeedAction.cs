using Gameplay.Map.Allies;
using Gameplay.Map.Spawn;
using Runtime.Utils;
using UnityEngine;
using UnityEngine.Localization;
using Zenject;

namespace Audio.Gameplay.PointsGrid
{
    [System.Serializable]
    public class IncreaseWalkSpeedAction : IGridPointAction
    {
        [SerializeField] private LocalizedString _description;
        [SerializeField] private float _increaseConstantWalkSpeed = 0.1f;
        [SerializeField] private float _increaseMomentWalkSpeed = 1f;
        public ReactiveLocalizedString GetDescription() => new (_description);
        
        public void ApplyAction(
            SpawnModifiers momentModifiers,
            SpawnModifiers constantModifiers,
            WarriorsCollection warriors)
        {
            momentModifiers.WalkSpeed += _increaseMomentWalkSpeed;
            constantModifiers.WalkSpeed += _increaseConstantWalkSpeed;
        }
    }
}