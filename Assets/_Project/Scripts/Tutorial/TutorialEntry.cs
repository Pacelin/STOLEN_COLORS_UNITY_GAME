using UnityEngine;
using UnityEngine.UI;

public class TutorialEntry : MonoBehaviour
{
    [SerializeField]
    private Button _nextButton;

    [SerializeField]
    private TutorialPanels _tutorial;

    [SerializeField]
    private Transform _cameraTarget;

    public Transform CameraTarget => _cameraTarget;

    private void OnEnable()
    {
        _nextButton.onClick.AddListener(_tutorial.NextEntry);
    }

    private void OnDisable()
    {
        _nextButton.onClick.RemoveListener(_tutorial.NextEntry);
    }
}
