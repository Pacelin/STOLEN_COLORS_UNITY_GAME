using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UniOwl.UI
{
    public class TypewriterAnimation : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _text;

        private readonly List<Tween> _charTweens = new();

        private readonly List<Vector3> _charOffsets = new();
        
        [SerializeField]
        private float _animationSpeed = 0.1f;

        [SerializeField]
        private Vector3 _animationOffset = new Vector3(0f, 5f, 0f);

        [SerializeField]
        private Ease _easeFunction = Ease.Linear;
        
        private void OnDisable()
        {
            ResetAnimations();
        }

        public void AddCharAnimation(int charIndex)
        {
            _charOffsets.Add(_animationOffset);
            
            Tween tween = GetCharAnimationTween(charIndex);
            _charTweens.Add(tween);
        }

        public void ResetAnimations()
        {
            foreach (Tween charTween in _charTweens)
                charTween?.Kill();
            
            _charTweens.Clear();
            _charOffsets.Clear();
        }
        
        private void LateUpdate()
        {
            _text.ForceMeshUpdate();
                        
            TMP_TextInfo textInfo = _text.textInfo;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

                if (!charInfo.isVisible)
                    continue;

                TMP_MeshInfo meshInfo = textInfo.meshInfo[charInfo.materialReferenceIndex];
                            
                Vector3[] verts = meshInfo.vertices;

                for (int j = 0; j < 4; j++)
                {
                    int index = charInfo.vertexIndex + j;
                    Vector3 vertex = verts[index];
                    
                    if (i < _charOffsets.Count)
                        verts[index] = vertex + _charOffsets[i];
                }
            }
                        
            _text.UpdateVertexData();
        }

        private Tween GetCharAnimationTween(int charIndex)
        {
            return DOVirtual.Vector3(
                _animationOffset,
                Vector3.zero,
                _animationSpeed,
                offset => _charOffsets[charIndex] = offset);
        }
    }
}
