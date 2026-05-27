using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

namespace SamuraiSlice
{
    public class ScorePopup : MonoBehaviour
    {
        [Header("Text references")]
        [SerializeField] private TMP_Text pointsText;
        [SerializeField] private TMP_Text comboText;

        [Header("Animation")]
        [SerializeField] private float scalePunchDuration = 0.10f;
        [SerializeField] private float fadeDuration = 0.70f;
        [SerializeField] private float riseDistance = 1.0f;

        private IObjectPool<ScorePopup> _pool;
        private Coroutine _anim;

        public void Init(int finalPoints, int multiplier, Vector3 worldPos, Color accentColour)
        {
            transform.position = worldPos;
            transform.localScale = Vector3.one;

            if (pointsText != null)
            {
                pointsText.text = $"+{finalPoints}";
                pointsText.color = accentColour;
            }

            if (comboText != null)
            {
                if (multiplier >= 2)
                {
                    comboText.text = $"×{multiplier} COMBO";
                    comboText.color = new Color(accentColour.r, accentColour.g, accentColour.b, 0.85f);
                    comboText.gameObject.SetActive(true);
                }
                else
                {
                    comboText.gameObject.SetActive(false);
                }
            }

            gameObject.SetActive(true);

            if (_anim != null)
            {
                StopCoroutine(_anim);
            }
            _anim = StartCoroutine(Animate());
        }

        public void SetPool(IObjectPool<ScorePopup> pool)
        {
            _pool = pool;
        }

        private IEnumerator Animate()
        {
            // Phase 1: scale punch 1 → 1.2
            float t = 0f;
            while (t < scalePunchDuration)
            {
                t += Time.deltaTime;
                float k = Mathf.Clamp01(t / scalePunchDuration);
                float s = Mathf.Lerp(1f, 1.2f, k);
                transform.localScale = new Vector3(s, s, 1f);
                yield return null;
            }

            // Phase 2: rise + fade
            Vector3 startPos = transform.position;
            Vector3 endPos = startPos + Vector3.up * riseDistance;
            Color pointsStart = pointsText != null ? pointsText.color : Color.white;
            Color comboStart = (comboText != null && comboText.gameObject.activeSelf)
                                  ? comboText.color : Color.clear;

            t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                float k = Mathf.Clamp01(t / fadeDuration);

                transform.position = Vector3.Lerp(startPos, endPos, k);

                if (pointsText != null)
                {
                    Color c = pointsStart;
                    c.a = Mathf.Lerp(1f, 0f, k);
                    pointsText.color = c;
                }

                if (comboText != null && comboText.gameObject.activeSelf)
                {
                    Color c = comboStart;
                    c.a = Mathf.Lerp(comboStart.a, 0f, k);
                    comboText.color = c;
                }

                yield return null;
            }

            _anim = null;
            ReturnToPool();
        }

        private void ReturnToPool()
        {
            if (_pool != null)
            {
                _pool.Release(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}