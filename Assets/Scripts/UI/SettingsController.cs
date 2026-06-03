using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace SamuraiSlice
{
    public class SettingsController : MonoBehaviour
    {
        [SerializeField] private LeaderboardManager leaderboardManager;
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Toggle musicToggle;
        [SerializeField] private Toggle sfxToggle;
        [SerializeField] private GameObject confirmationPanel;
        [SerializeField] private GameObject settingsPanel;

        private void Awake()
        {
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
            sfxToggle.onValueChanged.AddListener(OnSFXToggleChanged);
        }

        private void OnEnable()
        {
            masterVolumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(SettingsBootstrap.KeyMasterVolume, 1f));
            musicToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt(SettingsBootstrap.KeyMusicEnabled, 1) == 1);
            sfxToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt(SettingsBootstrap.KeySFXEnabled, 1) == 1);

            if(confirmationPanel != null)
            {
                confirmationPanel.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
            musicToggle.onValueChanged.RemoveListener(OnMusicToggleChanged);
            sfxToggle.onValueChanged.RemoveListener(OnSFXToggleChanged);
        }

        private void OnMasterVolumeChanged(float value)
        {
            SettingsBootstrap.ApplyMasterVolume(mixer, value);
            PlayerPrefs.SetFloat(SettingsBootstrap.KeyMasterVolume, value);
            PlayerPrefs.Save();
        }

        private void OnMusicToggleChanged(bool isOn)
        {
            SettingsBootstrap.ApplyMusicEnabled(mixer, isOn);
            PlayerPrefs.SetInt(SettingsBootstrap.KeyMusicEnabled, isOn ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void OnSFXToggleChanged(bool isOn)
        {
            SettingsBootstrap.ApplySFXEnabled(mixer, isOn);
            PlayerPrefs.SetInt(SettingsBootstrap.KeySFXEnabled, isOn ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void OnResetHighScoresClicked()
        {
            if (confirmationPanel != null)
                confirmationPanel.SetActive(true);
        }

        public void OnConfirmResetClicked()
        {
            if(leaderboardManager != null)
            {
                leaderboardManager.ResetLeaderboard();
            } else
            {
                Debug.LogWarning("[SettingsController] leaderboardManager not assigned; " +
                                 "falling back to direct PlayerPrefs deletion.", this);
                PlayerPrefs.DeleteKey("SamuraiSlice_Leaderboard");
                PlayerPrefs.DeleteKey("SamuraiSlice_HighScore");
                PlayerPrefs.Save();
            }

            if (confirmationPanel != null)
                confirmationPanel.SetActive(false);
        }

        public void OnCancelResetClicked()
        {
            if (confirmationPanel != null)
                confirmationPanel.SetActive(false);
        }

        public void Open()
        {
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(true);
            }

            if (confirmationPanel != null)
            {
                confirmationPanel.SetActive(false);
            }

            masterVolumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(SettingsBootstrap.KeyMasterVolume, 1f));
            musicToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt(SettingsBootstrap.KeyMusicEnabled, 1) == 1);
            sfxToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt(SettingsBootstrap.KeySFXEnabled, 1) == 1);
        }

        public void OnBackClicked()
        {
            if(settingsPanel != null)
            {
                settingsPanel.SetActive(false);
            }
        }
    }

}