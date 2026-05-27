using System;
using UnityEngine;

namespace SamuraiSlice
{
    [RequireComponent(typeof(ParticleSystem))]
    public class SliceParticles : MonoBehaviour
    {
        private ParticleSystem _ps;
        private Action<SliceParticles> _onComplete;

        private void Awake()
        {
            _ps = GetComponent<ParticleSystem>();
        }

        public void Play(Color accentColour, Action<SliceParticles> onComplete)
        {
            _onComplete = onComplete;

            ParticleSystem.MainModule main = _ps.main;
            main.startColor = new ParticleSystem.MinMaxGradient(accentColour);

            _ps.Clear();
            _ps.Play();
        }

        private void OnParticleSystemStopped()
        {
            _onComplete?.Invoke(this);
            _onComplete = null;
        }
    }
}
