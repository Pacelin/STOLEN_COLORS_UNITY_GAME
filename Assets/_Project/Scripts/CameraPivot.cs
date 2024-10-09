using UnityEngine;

public class CameraPivot : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Vector2Int _pivot;
    
    [ContextMenu("Refresh pivot")]
    private void Awake()
    {
        var orth = _camera.orthographicSize;
        var aspect = _camera.aspect;
        var transform1 = transform;
        var p = transform1.localPosition;
        p.x = _pivot.x * orth * aspect;
        p.y = _pivot.y * orth;
        transform1.localPosition = p;
    }
}