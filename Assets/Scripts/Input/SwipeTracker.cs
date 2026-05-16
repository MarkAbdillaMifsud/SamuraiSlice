using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SamuraiSlice
{
    public class SwipeTracker : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SwipeInput swipeInput;

        [Header("Buffer")]
        [SerializeField, Min(2)] private int maxSamples = 8;

        [Header("Blade Visual")]
        [SerializeField] private TrailRenderer bladeTrail;

        [SerializeField] private float clearGrace = 0.1f;

        [Header("Debug")]
        [SerializeField] private bool drawDebug = true;
        [SerializeField] private Color debugColour = Color.cyan;

        private readonly List<Vector2> _path = new();
        private float _releaseTime = -1.0f;

        public IReadOnlyList<Vector2> CurrentPath => _path; //readonly view of the current swipe path (empty when player is not swiping)

        private void Awake()
        {
            if(swipeInput == null)
            {
                swipeInput = GetComponent<SwipeInput>();
            }
            if(swipeInput == null) //runs if GetComponent returns false
            {
                Debug.LogError($"{nameof(SwipeTracker)} requires a {nameof(SwipeInput)} reference.", this);
                enabled = false;
            }
        }

        private void Update()
        {
            if (swipeInput.IsSwiping)
            {
                _releaseTime = -1f;

                if (bladeTrail != null && !bladeTrail.emitting)
                {
                    bladeTrail.transform.position = swipeInput.CurrentWorldPoint;
                    bladeTrail.Clear();
                    bladeTrail.emitting = true;
                }

                if (bladeTrail != null)
                    bladeTrail.transform.position = swipeInput.CurrentWorldPoint;

                AppendSample(swipeInput.CurrentWorldPoint);
            }
            else
            {
                if (bladeTrail != null && bladeTrail.emitting)
                    bladeTrail.emitting = false;

                if (_path.Count > 0)
                {
                    if (_releaseTime < 0f)
                        _releaseTime = Time.time;
                    else if (Time.time - _releaseTime >= clearGrace)
                    {
                        _path.Clear();
                        _releaseTime = -1f;
                    }
                }
            }

            if (drawDebug)
            {
                for(int i = 1; i < _path.Count; i++)
                {
                    Debug.DrawLine(_path[i - 1], _path[i], debugColour);
                }
            }
        }

        private void AppendSample(Vector2 sample)
        {
            _path.Add(sample);
            if(_path.Count > maxSamples)
            {
                _path.RemoveAt(0);
            }
        }
    }
}
