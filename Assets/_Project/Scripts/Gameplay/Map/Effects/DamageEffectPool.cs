using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Gameplay.Map;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Map.Effects
{
    public class DamageEffectPool : MonoBehaviour
    {
        [Space]
        [SerializeField] private DamageEffect _prefab;
        [SerializeField] private int _initialPoolSize;
        
        private List<DamageEffect> _nonActive = new();

        private void Awake()
        {
            for (int i = 0; i < _initialPoolSize; i++)
                Spawn();
        }

        public async UniTaskVoid ApplyEffect(Unit warrior, float damage)
        {
            var d = Get();
            await d.Show(warrior, (int)damage);
            Return(d);
        }

        private DamageEffect Get()
        {
            if (_nonActive.Count == 0)
                return Instantiate(_prefab, transform);
            var o = _nonActive[^1];
            _nonActive.Remove(o);
            return o;
        }

        private void Return(DamageEffect d) =>
            _nonActive.Add(d);

        private void Spawn()
        {
            var o = Instantiate(_prefab, transform);
            o.gameObject.SetActive(false);
            _nonActive.Add(o);
        }
    }
}