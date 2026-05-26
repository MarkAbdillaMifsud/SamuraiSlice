using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SamuraiSlice
{
    [RequireComponent(typeof(Volume))]
    public class RedVignettePulse : MonoBehaviour
    {
        [SerializeField] private Volume globalVolume;
        [SerializeField, Range(0f, 1f)] private float peakIntensity;
        [SerializeField] private float riseSeconds = 0.05f;
        [SerializeField] private float fallSeconds = 0.35f;

        private Vignette _vignette;
        private Coroutine _running;

        private void Awake()
        {
            if(globalVolume == null)
            {
                globalVolume = GetComponent<Volume>();
            }

            if(globalVolume == null || globalVolume.profile == null || !globalVolume.profile.TryGet(out _vignette))
            {
                Debug.LogWarning("[RedVignettePulse] Vignette override not found on assigned Volume.", this);
                return;
            }

            _vignette.intensity.value = 0f;
        }

        public void Pulse()
        {
            if(_vignette == null)
            {
                return;
            }

            if(_running != null)
            {
                StopCoroutine(_running);
            }
            _running = StartCoroutine(PulseRoutine());
        }

        private IEnumerator PulseRoutine()
        {
            float t = 0f;
            while (t < riseSeconds)
            {
                t += Time.unscaledDeltaTime;
                _vignette.intensity.value = Mathf.Lerp(0f, peakIntensity, t / riseSeconds);
                yield return null;
            }
            _vignette.intensity.value = peakIntensity;

            t = 0f;
            while (t < fallSeconds)
            {
                t += Time.unscaledDeltaTime;
                _vignette.intensity.value = Mathf.Lerp(peakIntensity, 0f, t / fallSeconds);
                yield return null;
            }
            _vignette.intensity.value = 0f;
            _running = null;
        }
    }
}
