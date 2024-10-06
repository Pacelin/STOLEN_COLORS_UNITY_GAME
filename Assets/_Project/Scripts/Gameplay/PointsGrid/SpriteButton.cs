using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Audio.Gameplay.PointsGrid
{
    public class SpriteButton : MonoBehaviour, IPointerClickHandler
    {
        public IObservable<Unit> OnClick => _onClick;
        private ReactiveCommand _onClick = new();
        
        public void OnPointerClick(PointerEventData eventData)
        {
            _onClick.Execute();
        }
    }
}