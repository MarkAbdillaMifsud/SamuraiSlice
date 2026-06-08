using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace SamuraiSlice
{
    public class FailureAudioController : MonoBehaviour
    {
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private string musicVolumeParam = "MusicVolume";
        [SerializeField] private AudioClip bombThudClip;
        [SerializeField] private AudioClip tinnitusRingClip;
        [SerializeField] private float tinnitusDelay = 0.05f;
        [SerializeField] private AudioMixerGroup sfxMixerGroup;
        [SerializeField] private float missHoldSeconds = 0.6f;
        [SerializeField] private float missFadeSeconds = 0.6f;

        private const float VolumeNormal = 0f;
        private const float VolumeSilent = -80f;

        private bool _bombDetonatedThisRun;
        private Coroutine _activeRoutine;

        private void OnEnable()
        {
            Bomb.DetonationStarted += HandleBombDetonationStarted;
        }

        private void OnDisable()
        {
            Bomb.DetonationStarted -= HandleBombDetonationStarted;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged -= HandleStateChanged;
            }
        }

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += HandleStateChanged;
            }
        }

        private void HandleBombDetonationStarted()
        {
            _bombDetonatedThisRun = true;

            CancelActiveRoutine();
            _activeRoutine = StartCoroutine(BombCutRoutine());
        }

        private void HandleStateChanged(GameManager.GameState newState)
        {
            switch (newState)
            {
                case GameManager.GameState.GameOver:
                    OnEnterGameOver();
                    break;

                case GameManager.GameState.Playing:
                    OnEnterPlaying();
                    break;
            }
        }

        private void OnEnterGameOver()
        {
            if (_bombDetonatedThisRun)
            {
                return;
            }

            CancelActiveRoutine();
            _activeRoutine = StartCoroutine(MissOutFadeRoutine());
        }

        private void OnEnterPlaying()
        {
            CancelActiveRoutine();
            _bombDetonatedThisRun = false;
            SetMusicVolume(VolumeNormal);
        }

        private IEnumerator BombCutRoutine()
        {
            SetMusicVolume(VolumeSilent);

            PlayOneShot(bombThudClip);

            if (tinnitusRingClip != null && tinnitusDelay > 0f)
            {
                yield return new WaitForSecondsRealtime(tinnitusDelay);
                PlayOneShot(tinnitusRingClip);
            }

            _activeRoutine = null;
        }

        private IEnumerator MissOutFadeRoutine()
        {
            yield return new WaitForSecondsRealtime(missHoldSeconds);

            float elapsed = 0f;
            while (elapsed < missFadeSeconds)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / missFadeSeconds);
                float dB = Mathf.Lerp(VolumeNormal, VolumeSilent, t);
                SetMusicVolume(dB);
                yield return null;
            }

            SetMusicVolume(VolumeSilent);
            _activeRoutine = null;
        }

        private void SetMusicVolume(float dB)
        {
            if (mixer == null)
            {
                return;
            }
            mixer.SetFloat(musicVolumeParam, dB);
        }

        private void CancelActiveRoutine()
        {
            if (_activeRoutine != null)
            {
                StopCoroutine(_activeRoutine);
                _activeRoutine = null;
            }
        }

        private void PlayOneShot(AudioClip clip)
        {
            if (clip == null)
            {
                return;
            }

            var host = new GameObject($"[FailureSFX] {clip.name}");
            host.transform.SetParent(transform);

            var src = host.AddComponent<AudioSource>();
            src.clip = clip;
            src.pitch = 1f;
            src.volume = 1f;
            src.spatialBlend = 0f;
            src.playOnAwake = false;

            if (sfxMixerGroup != null)
            {
                src.outputAudioMixerGroup = sfxMixerGroup;
            }

            src.Play();
            Destroy(host, clip.length + 0.1f);
        }
    }
}