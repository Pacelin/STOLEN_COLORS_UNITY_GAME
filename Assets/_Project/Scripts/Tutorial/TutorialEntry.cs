using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialEntry : MonoBehaviour
{
    [SerializeField]
    private CameraController _cameraController;
    
    [SerializeField]
    private Image _border;

    [SerializeField]
    private TMP_Text _text;

    [SerializeField]
    private Button _nextButton;

    [SerializeField]
    private TutorialPanels _tutorial;

    [SerializeField]
    private Transform _cameraTarget;

    private void OnEnable()
    {
        _nextButton.onClick.AddListener(OnEntryComplete);
    }

    private void OnDisable()
    {
        _nextButton.onClick.RemoveListener(OnEntryComplete);
    }

    private void OnEntryComplete()
    {
        _cameraController.SetTarget(_cameraTarget);
    }
}
