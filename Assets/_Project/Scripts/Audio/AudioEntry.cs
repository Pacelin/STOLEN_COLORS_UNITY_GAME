﻿using UnityEngine;

namespace Audio
{
    [System.Serializable]
    public class AudioEntry
    {
        public bool IsStreaming;
        [Space]
        public string AudioPath;
        public AudioClip Clip;
        public EAudioCategory Category;
        public float VolumeMultiplier = 1;
    }
}