using UnityEngine;

namespace Audio
{
    public static class AudioExtensions
    {
        public static float ToAudioVolume(this float sliderValue) =>
            Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20;
        public static float ToAudioSlider(this float volume) =>
            Mathf.Pow(10, volume / 20);
    }
}