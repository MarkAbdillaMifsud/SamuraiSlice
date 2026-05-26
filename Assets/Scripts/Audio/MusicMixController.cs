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

        [Header("Combo Source")]
        [SerializeField] private ComboTracker comboTracker;

        [Header("Transition")]
        [SerializeField] private float transitionTime = 0.6f;

        private bool isEnergised;

        private void Start()
        {
            calmSnapshot.TransitionTo(0f);
            isEnergised = false;
        }

        private void OnEnable()
        {
            if(comboTracker != null)
            {
                comboTracker.OnEnergisedEntered += EnterEnergised;
                comboTracker.OnStrokeEnded += ExitEnergised;
            }
        }

        private void OnDisable()
        {
            if(comboTracker != null )
            {
                comboTracker.OnEnergisedEntered -= EnterEnergised;
                comboTracker.OnStrokeEnded -= ExitEnergised;
            }
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
