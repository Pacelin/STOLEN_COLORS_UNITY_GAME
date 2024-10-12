using UnityEngine;

public class TutorialEntryBase : MonoBehaviour
{
    [SerializeField]
    protected TutorialPanels _tutorial;

    [SerializeField]
    private Transform _cameraTarget;

    public Transform CameraTarget => _cameraTarget;

    [SerializeField]
    private bool _showPanelsOnBegin;
    [SerializeField]
    private bool _showPanelsOnEnd;

    protected virtual void OnEnable()
    {
    }

    protected virtual void OnDisable()
    {
    }
}
