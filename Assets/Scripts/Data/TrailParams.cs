using UnityEngine;

[CreateAssetMenu(fileName = "TrailParams", menuName = "Samurai Slice/Trail Params")]
public class TrailParams : ScriptableObject
{
    public AnimationCurve widthByCombo = AnimationCurve.Linear(0f, 0.15f, 1f, 0.30f);
    public Gradient colourByCombo = DefaultGradient();

    public static float NormalisedLevel(int comboLevel, int maxCombo = 5) => Mathf.Clamp01((comboLevel - 1f) / (maxCombo - 1f));

    private static Gradient DefaultGradient()
    {
        var g = new Gradient();
        g.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(new Color(0.5f, 0.9f, 1f), 0.5f),
                new GradientColorKey(new Color(0f, 0.85f, 1f), 1f),
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f),
            }
        );
        return g;
    }
}
