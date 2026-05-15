using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SamuraiSlice
{
    public class SwipeInput : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private InputActionAsset playerInputActions;
        [SerializeField] private string actionMapName = "Gameplay";
        [SerializeField] private string pressActionName = "PointerPress";
        [SerializeField] private string positionActionName = "Pointer";

        [Header("Camera")]
        [SerializeField] private Camera worldCamera;

        [Header("Sampling")]
        [SerializeField] private float minSampleDistance = 0.02f;

        private InputAction _pressAction;
        private InputAction _positionAction;

        private Vector2 _lastWorldPoint;
        private Vector2 _currentWorldPoint;
        private bool _hasFirstSample;

        public bool IsSwiping { get; private set; }
        public Vector2 CurrentWorldPoint => _currentWorldPoint;

        // Will be called by SwipeDetector to decide whether to run the Linecast or not 
        public bool TryGetCurrentSegment(out Vector2 from, out Vector2 to)
        {
            if (IsSwiping && _hasFirstSample)
            {
                from = _lastWorldPoint;
                to = _currentWorldPoint;
                return from != to;
            }
            from = to = default;
            return false;
        }

        private void Awake()
        {
            if (worldCamera == null)
            {
                worldCamera = Camera.main;
            }

            // Ensures immediate error if action map is renamed in the asset but not in this script's relevant fields
            var map = playerInputActions.FindActionMap(actionMapName, throwIfNotFound: true);
            _pressAction = map.FindAction(pressActionName, throwIfNotFound: true);
            _positionAction = map.FindAction(positionActionName, throwIfNotFound: true);
        }

        private void OnEnable()
        {
            playerInputActions.Enable();
            _pressAction.started += HandlePressStarted;
            _pressAction.canceled += HandlePressEnded;
        }

        private void OnDisable()
        {
            _pressAction.started -= HandlePressStarted;
            _pressAction.canceled -= HandlePressEnded;
            playerInputActions.Disable();
            ResetSwipe();
        }

        private void Update()
        {
            if (!IsSwiping)
            {
                return;
            }

            Vector2 sampled = ScreenToWorld(_positionAction.ReadValue<Vector2>());

            if (!_hasFirstSample)
            {
                _lastWorldPoint = sampled;
                _currentWorldPoint = sampled;
                _hasFirstSample = true;
                return;
            }

            // Ensure a swipe is valid first rather than processing everything every frame
            if (Vector2.Distance(sampled, _currentWorldPoint) >= minSampleDistance)
            {
                _lastWorldPoint = _currentWorldPoint;
                _currentWorldPoint = sampled;
            }
        }

        private void HandlePressStarted(InputAction.CallbackContext _)
        {
            IsSwiping = true;
            _hasFirstSample = false;
        }

        private void HandlePressEnded(InputAction.CallbackContext _) => ResetSwipe();

        private void ResetSwipe()
        {
            IsSwiping = false;
            _hasFirstSample = false;
        }

        private Vector2 ScreenToWorld(Vector2 screenPoint)
        {
            Vector3 sp = new(screenPoint.x, screenPoint.y, -worldCamera.transform.position.z);
            return worldCamera.ScreenToWorldPoint(sp);
        }

        //TODO: Remove later - intended for gizmo testing of swiping
        private void OnDrawGizmos()
        {
            if (!IsSwiping || !_hasFirstSample) return;
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(_lastWorldPoint, _currentWorldPoint);
            Gizmos.DrawSphere(_currentWorldPoint, 0.1f);
        }
    }
}
