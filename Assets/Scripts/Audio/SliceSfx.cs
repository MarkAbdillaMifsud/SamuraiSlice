using UnityEngine;
using UnityEngine.Audio;

namespace SamuraiSlice
{
    public class SliceSfx : MonoBehaviour
    {
        [SerializeField] private ScoreManager scoreManager;

        [SerializeField] private AudioClip[] sliceVariants;
        [SerializeField] private AudioClip chimeClip;
        [SerializeField] private AudioMixerGroup sfxMixerGroup;

        private const float SlicePitchStop = 0.02f;
        private const float ChimePitchStep = 0.15f;

        private int _sliceIndex = 0;

        private void OnEnable()
        {
            if(scoreManager != null)
            {
                scoreManager.OnSliceScored += HandleSliceScored;
            }
        }

        private void OnDisable()
        {
            if(scoreManager != null)
            {
                scoreManager.OnSliceScored -= HandleSliceScored;
            }
        }

        public void Play(Vector2 worldPos, int comboLevel)
        {
            if(sliceVariants == null || sliceVariants.Length == 0)
            {
                Debug.LogWarning("[SliceSfx] No slice variants assigned.");
                return;
            }

            AudioClip sliceClip = sliceVariants[_sliceIndex];
            _sliceIndex = (_sliceIndex + 1) % sliceVariants.Length;

            float slicePitch = 1f + SlicePitchStop * (comboLevel - 1);
            PlayOneShot(sliceClip, worldPos, slicePitch);

            if(comboLevel >= 2 && chimeClip != null)
            {
                float chimePitch = 1f + ChimePitchStep * (comboLevel - 2);
                PlayOneShot(chimeClip, worldPos, chimePitch);
            }
        }

        private void PlayOneShot(AudioClip clip, Vector2 worldPos, float pitch)
        {
            if(clip == null)
            {
                return;
            }

            GameObject host = new GameObject($"[SFX] {clip.name}");
            host.transform.position = new Vector3(worldPos.x, worldPos.y, 0f);

            AudioSource src = host.AddComponent<AudioSource>();
            src.clip = clip;
            src.pitch = pitch;
            src.volume = 1f;
            src.spatialBlend = 0f;
            src.playOnAwake = false;

            if(sfxMixerGroup != null)
            {
                src.outputAudioMixerGroup = sfxMixerGroup;
            }

            src.Play();

            Destroy(host, clip.length / Mathf.Max(pitch, 0.01f) + 0.1f);
        }

        private void HandleSliceScored(int finalPoints, int multiplier, Vector3 worldPos, Color accentColour)
        {
            Play(worldPos, multiplier);
        }
    }
}
