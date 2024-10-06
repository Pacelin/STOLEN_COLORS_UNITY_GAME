using Gameplay.Map.Allies;
using Gameplay.Map.Spawn;
using Runtime.Utils;

namespace Audio.Gameplay.PointsGrid
{
    public interface IGridPointAction
    {
        ReactiveLocalizedString GetDescription();
        void ApplyAction(SpawnModifiers momentModifiers, SpawnModifiers constantModifiers, WarriorsCollection warriors);
    }
}