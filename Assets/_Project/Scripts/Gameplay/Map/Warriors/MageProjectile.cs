using UnityEngine;

namespace Gameplay.Map
{
    public class MageProjectile : MonoBehaviour
    {
        [SerializeField] private float _duration;
        [SerializeField] private AnimationCurve _yAnimationCurve;
        [SerializeField] private AnimationCurve _xAnimationCurve;

        private float _time;
        private float _damage;
        private Unit _target;
        private Vector3 _firePosition;
        private Vector3 _destinationPosition;
        private bool _hasTarget;
            
        public void Shoot(Vector3 firePosition, Unit target, float damage)
        {
            _firePosition = firePosition;
            _destinationPosition = target.DamagePosition;
            _time = 0;
            _target = target;
            _damage = damage;
            _hasTarget = true;
        }

        private void Update()
        {
            if (!_hasTarget)
                return;
            if (_target)
                _destinationPosition = _target.DamagePosition;
            _time += Time.deltaTime;
            var t = _time / _duration;
            var x = Mathf.LerpUnclamped(_firePosition.x, _destinationPosition.x, _xAnimationCurve.Evaluate(t));
            var height = _yAnimationCurve.Evaluate(t);
            var z = Mathf.LerpUnclamped(_firePosition.z, _destinationPosition.z, t) + height;
            transform.position = new Vector3(x, _firePosition.y, z);
            if (t >= 1)
            {
                if (_target)
                    _target.TakeDamage(_damage);
                Destroy(gameObject);
            }
        }
    }
}