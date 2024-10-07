using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Core
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _pauseMenu;
        [SerializeField] private GameObject _settingsMenu;

        private bool _showen;

        public void Reload()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        private void Awake()
        {
            _showen = false;
            _settingsMenu.SetActive(false);
            _pauseMenu.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_showen)
                    Hide();
                else
                    Show();
            }
        }

        private void Show()
        {
            Time.timeScale = 0.0001f;
            _pauseMenu.SetActive(true);
        }

        public void Hide()
        {
            _pauseMenu.SetActive(false);
            _settingsMenu.SetActive(false);
            Time.timeScale = 1;
        }
    }
}