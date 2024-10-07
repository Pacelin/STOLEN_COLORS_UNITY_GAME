using UnityEngine;

public class CameraLeftPivotX : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    private void Awake()
    {
        var orth = _camera.orthographicSize;
        var aspect = _camera.aspect;
        var transform1 = transform;
        var p = transform1.localPosition;
        p.x = -orth * aspect;
        transform1.localPosition = p;
    }
}