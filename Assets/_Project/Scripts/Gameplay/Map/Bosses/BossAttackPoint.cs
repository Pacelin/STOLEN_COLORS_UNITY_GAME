using System.Linq;
using UnityEngine;

namespace Gameplay.Map.Bosses
{
    public class BossAttackPoint : MonoBehaviour
    {
        public float Cooldown => _cooldown;
        public float AnimationTime => _animationTime;
        public float AttackDuration => _attackDuration;
        
        [SerializeField] private float _cooldown;
        [SerializeField] private float _radius;
        [SerializeField] private float _damage;
        [SerializeField] private float _animationTime;
        [SerializeField] private float _attackDuration;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }

        public void SendDamage(Warrior[] warriors)
        {
            if (warriors.Length == 0)
                return;

            var ordered = warriors
                .ToDictionary(w => w, w => Vector3.Distance(transform.position, w.Position))
                .OrderBy(kp => kp.Value)
                .ToArray();
            
            for (int i = 0; i < ordered.Length; i++)
            {
                if (i == 0 && ordered[i].Value > _radius)
                {
                    ordered[i].Key.TakeDamage(_damage);
                    break;
                }

                if (ordered[i].Value <= _radius)
                    ordered[i].Key.TakeDamage(_damage);
                else
                    break;
            }
        }
    }
}