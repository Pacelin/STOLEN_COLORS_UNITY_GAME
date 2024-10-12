using Gameplay.Map.Spawn;
using UnityEngine;
using Zenject;

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

    [Inject]
    private WaveManager _waveManager;
    
    protected virtual void OnEnable()
    {
        Debug.Log(_waveManager);
        Debug.Log(!_showPanelsOnBegin);
        _waveManager.SetHidePanels(!_showPanelsOnBegin);
    }

    protected virtual void OnDisable()
    {
        Debug.Log(_waveManager);
        Debug.Log(!_showPanelsOnEnd);
        _waveManager.SetHidePanels(!_showPanelsOnEnd);
    }
}
