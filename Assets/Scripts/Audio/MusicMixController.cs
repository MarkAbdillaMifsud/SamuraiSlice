using UnityEngine;
using UnityEngine.Audio;

namespace SamuraiSlice
{
    public class MusicMixController : MonoBehaviour
    {
        [Header("Mixer")]
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private AudioMixerSnapshot calmSnapshot;
        [SerializeField] private AudioMixerSnapshot energisedSnapshot;

        [Header("Transition")]
        [SerializeField] private float transitionTime = 0.6f;

        private bool isEnergised;

        private void Start()
        {
            calmSnapshot.TransitionTo(0f);
            isEnergised = false;
        }

        public void EnterEnergised()
        {
            if(isEnergised)
            {
                return;
            }

            energisedSnapshot.TransitionTo(transitionTime);
            isEnergised = true;
        }

        public void ExitEnergised()
        {
            if(!isEnergised)
            {
                return;
            }

            calmSnapshot.TransitionTo(transitionTime);
            isEnergised = false;
        }
    }
}
