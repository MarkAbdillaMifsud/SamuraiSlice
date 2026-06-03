using UnityEngine;

namespace SamuraiSlice
{
    [CreateAssetMenu(menuName = "Samurai Slice/Spawn Curve")]
    public class SpawnCurve : ScriptableObject
    {
        public AnimationCurve spawnInterval = new(
            new Keyframe(0f, 1.0f),
            new Keyframe(20f, 1.0f),
            new Keyframe(45f, 0.7f),
            new Keyframe(75f, 0.5f),
            new Keyframe(300f, 0.5f)
        );

        public AnimationCurve bombProbability = new(
            new Keyframe(0f, 0f),
            new Keyframe(15f, 0f),
            new Keyframe(20f, 0f),
            new Keyframe(45f, 0.04f),
            new Keyframe(75f, 0.08f),
            new Keyframe(300f, 0.08f)
        );

        public Vector2 phase1GroupSizeRange = new(1f, 1f);
        public Vector2 phase2GroupSizeRange = new(1f, 2f);
        public Vector2 phase3GroupSizeRange = new(2f, 4f);

        public AnimationCurve[] ingredientWeights =
        {
            new AnimationCurve(
                new Keyframe(0f, 0.40f),
                new Keyframe(20f, 0.40f),
                new Keyframe(45f, 0.30f),
                new Keyframe(75f, 0.25f),
                new Keyframe(300f, 0.25f)
            ),

            new AnimationCurve(
                new Keyframe(0f, 0.30f),
                new Keyframe(20f, 0.30f),
                new Keyframe(45f, 0.25f),
                new Keyframe(75f, 0.22f),
                new Keyframe(300f, 0.22f)
            ),

            new AnimationCurve(
                new Keyframe(0f, 0.20f),
                new Keyframe(20f, 0.20f),
                new Keyframe(45f, 0.22f),
                new Keyframe(75f, 0.22f),
                new Keyframe(300f, 0.22f)
            ),

            new AnimationCurve(
                new Keyframe(0f, 0.08f),
                new Keyframe(20f, 0.08f),
                new Keyframe(45f, 0.15f),
                new Keyframe(75f, 0.18f),
                new Keyframe(300f, 0.18f)
            ),

            new AnimationCurve(
                new Keyframe(0f, 0.02f),
                new Keyframe(20f, 0.02f),
                new Keyframe(45f, 0.08f),
                new Keyframe(75f, 0.13f),
                new Keyframe(300f, 0.13f)
            )
        };

        public float GetSpawnInterval(float elapsedRunTime)
        {
            return Mathf.Max(0.05f, spawnInterval.Evaluate(elapsedRunTime));
        }

        public float GetBombProbability(float elapsedRunTime)
        {
            return Mathf.Clamp01(bombProbability.Evaluate(elapsedRunTime));
        }

        public Vector2 GetGroupSizeRange(float elapsedRunTime)
        {
            if (elapsedRunTime < 20f)
            {
                return phase1GroupSizeRange;
            }

            if (elapsedRunTime < 45f)
            {
                return phase2GroupSizeRange;
            }

            return phase3GroupSizeRange;
        }

        public float GetIngredientWeight(int index, float elapsedRunTime)
        {
            if (ingredientWeights == null || index < 0 || index >= ingredientWeights.Length)
            {
                return 0f;
            }

            return Mathf.Max(0f, ingredientWeights[index].Evaluate(elapsedRunTime));
        }
    }
}
