using UnityEngine;

namespace SamuraiSlice
{
    [RequireComponent(typeof(TrailRenderer))]
    public class BladeTrailController : MonoBehaviour
    {
        [SerializeField] private TrailParams trailParams;
        [SerializeField] private ComboTracker comboTracker;

        private TrailRenderer _trail;

        private void Awake()
        {
            _trail = GetComponent<TrailRenderer>();
        }

        private void OnEnable()
        {
            if (comboTracker != null)
            {
                comboTracker.OnMultiplierChanged += HandleMultiplierChanged;
            }
        }

        private void OnDisable()
        {
            if(comboTracker != null)
            {
                comboTracker.OnMultiplierChanged -= HandleMultiplierChanged;
            }
        }

        private void HandleMultiplierChanged(int newMultiplier)
        {
            if(trailParams == null)
            {
                return;
            }

            float t = TrailParams.NormalisedLevel(newMultiplier);

            _trail.widthMultiplier = trailParams.widthByCombo.Evaluate(t);

            Color sampleColour = trailParams.colourByCombo.Evaluate(t);
            ApplyTrailColour(sampleColour);
        }

        private readonly Gradient _trailGradient = new Gradient();
        private readonly GradientColorKey[] _colourKeys = new GradientColorKey[2];
        private readonly GradientAlphaKey[] _alphaKeys = new GradientAlphaKey[2]
        {
        new GradientAlphaKey(1f, 0f),
        new GradientAlphaKey(0f, 1f),
        };

        private void ApplyTrailColour(Color leadColour)
        {
            _colourKeys[0] = new GradientColorKey(leadColour, 0f);
            _colourKeys[1] = new GradientColorKey(leadColour * 0.4f, 1f);
            _trailGradient.SetKeys(_colourKeys, _alphaKeys);
            _trail.colorGradient = _trailGradient;
        }
    }
}
