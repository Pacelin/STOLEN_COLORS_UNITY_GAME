using UnityEngine;
using UnityEngine.UI;

namespace UniOwl.UI
{
    public class ApplicationQuitButton : MonoBehaviour
    {
        [SerializeField] private Button quitButton;

        private void Awake()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
                quitButton.gameObject.SetActive(false);
            else
                quitButton.onClick.AddListener(Quit);
        }

        private void Quit()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
            #else
            Application.Quit();
            #endif
        }
    }
}
