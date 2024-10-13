using UnityEngine;
using Zenject;

namespace Audio
{
    public class Ost : MonoBehaviour
    {
        [SerializeField] private EMusicKey _ost;
        [SerializeField] private Texture2D _cursor;
        [Inject] private AudioSystem _audio;

        private void Awake()
        {
            Cursor.SetCursor(_cursor, new Vector2(6, 0), CursorMode.ForceSoftware);
            Time.timeScale = 1f;
            _audio.PlayMusic(_ost, true);
        }
    }
}