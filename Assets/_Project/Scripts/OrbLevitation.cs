using UnityEngine;

public class OrbLevitation : MonoBehaviour
{
    [SerializeField]
    private Transform _target;

    [SerializeField]
    private float _amplitude, _speed;

    private Vector3 _initialPosition;
    
    private void OnEnable()
    {
        _initialPosition = _target.position;
    }

    private void Update()
    {
        Vector3 position = Vector3.zero;
        position.z = Mathf.Cos(Time.time * _speed) * _amplitude;
        _target.position = position + _initialPosition;
    }
}
