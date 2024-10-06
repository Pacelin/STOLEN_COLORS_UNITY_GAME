using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;
using Audio;

namespace UniOwl.UI
{
    public class UniSelectableAudio : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
    {
        [SerializeField] private Selectable _selectable;

        [Inject]
        private AudioSystem _audioSystem;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_selectable != null && !_selectable.interactable) return;
            
            _audioSystem.PlaySound(ESoundKey.UIHover);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_selectable != null && !_selectable.interactable) return;
            _audioSystem.PlaySound(ESoundKey.UIClick);
        }
    }
}