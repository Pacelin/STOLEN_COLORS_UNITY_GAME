using UnityEngine;

namespace Audio
{
    [System.Serializable]
    public class AudioEntry
    {
        public AudioClip Clip;
        public EAudioCategory Category;
        public float VolumeMultiplier = 1;
    }
}