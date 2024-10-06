using UnityEngine;
using UniRx;

namespace Gameplay.Map.Bosses
{
    public class Boss : Unit
    {
        private void Start()
        {
            Model.OnDie.Subscribe(_ => GameObject.Destroy(this.gameObject))
                .AddTo(this);
        }
    }
}