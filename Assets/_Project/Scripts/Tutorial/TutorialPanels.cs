using UnityEngine;

public class TutorialPanels : MonoBehaviour
{
    [SerializeField]
    private TutorialEntryBase[] _entries;

    [SerializeField]
    private CameraController _cameraController;
    
    private int _currentEntryIndex = -1;
    
    private void Start()
    {
        NextEntry();
    }

    public void NextEntry()
    {
        if (_currentEntryIndex >= 0)
            _entries[_currentEntryIndex].gameObject.SetActive(false);
        
        _currentEntryIndex++;

        if (_currentEntryIndex == _entries.Length)
        {
            Complete();
            return;
        }
        
        var entry = _entries[_currentEntryIndex];
        entry.gameObject.SetActive(true);
        _cameraController.SetTarget(entry.CameraTarget);
    }

    private void Complete()
    {
        _cameraController.SetAutoState();
        Destroy(gameObject);
    }
}
