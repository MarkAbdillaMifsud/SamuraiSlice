using UnityEngine;
using UnityEngine.Audio;

namespace SamuraiSlice
{
    [DefaultExecutionOrder(10)]
    public class SettingsBootstrap : MonoBehaviour
    {
        public const string KeyMasterVolume = "Settings_MasterVolume";
        public const string KeyMusicEnabled = "Settings_MusicEnabled";
        public const string KeySFXEnabled = "Settings_SFXEnabled";

        [SerializeField] private AudioMixer mixer;

        private void Start()
        {
            ApplyAll();
        }

        public void ApplyAll()
        {
            ApplyMasterVolume(PlayerPrefs.GetFloat(KeyMasterVolume, 1f));
            ApplyMusicEnabled(PlayerPrefs.GetInt(KeyMusicEnabled, 1) == 1);
            ApplySFXEnabled(PlayerPrefs.GetInt(KeySFXEnabled, 1) == 1);
        }

        public static void ApplyMasterVolume(AudioMixer mixer, float linear)
        {
            float dB = linear > 0.0001f ? Mathf.Log10(linear) * 20f : -80f;
            mixer.SetFloat("MasterVolume", dB);
        }

        public static void ApplyMusicEnabled(AudioMixer mixer, bool enabled)
        {
            mixer.SetFloat("MusicVolume", enabled ? 0f : -80f);
        }

        public static void ApplySFXEnabled(AudioMixer mixer, bool enabled)
        {
            mixer.SetFloat("SFXVolume", enabled ? 0f : -80f);
        }

        private void ApplyMasterVolume(float linear) => ApplyMasterVolume(mixer, linear);
        private void ApplyMusicEnabled(bool enabled) => ApplyMusicEnabled(mixer, enabled);
        private void ApplySFXEnabled(bool enabled) => ApplySFXEnabled(mixer, enabled);
    }
}
