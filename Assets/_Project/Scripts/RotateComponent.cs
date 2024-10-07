using UnityEngine;

public class RotateComponent : MonoBehaviour
{
    [SerializeField]
    private Vector3 _rotationSpeed;

    private void Update()
    {
        transform.Rotate(_rotationSpeed * Time.deltaTime);
    }
}
