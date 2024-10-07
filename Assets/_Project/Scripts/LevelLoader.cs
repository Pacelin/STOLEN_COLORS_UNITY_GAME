using UnityEngine;
using UnityEngine.SceneManagement;

namespace Audio
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private int _buildIndex;
        public void Load() =>
            SceneManager.LoadScene(_buildIndex);
    }
}