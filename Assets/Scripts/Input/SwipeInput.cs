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

        public event Action OnPressStarted;
        public event Action OnPressReleased;

        private InputAction _pressAction;
        private InputAction _positionAction;

        private Vector2 _lastWorldPoint;
        private Vector2 _currentWorldPoint;
        private bool _hasFirstSample;
        private Vector2 _currentScreenPoint;

        public bool IsSwiping { get; private set; }
        public Vector2 CurrentWorldPoint => _currentWorldPoint;
        public Vector2 CurrentScreenPoint => _currentScreenPoint;

        // Will be called by SwipeDetector to decide whether to run the Linecast or not 
        public bool TryGetCurrentSegment(out Vector2 from, out Vector2 to)
        {
            if (IsSwiping && _hasFirstSample)
            {
                from = _lastWorldPoint;
                to = _currentWorldPoint;
                return (to - from).sqrMagnitude > 0.000001f;
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

            _currentScreenPoint = ReadPointerScreenPosition();
            Vector2 sampled = ScreenToWorld(_currentScreenPoint);

            if (!_hasFirstSample)
            {
                _lastWorldPoint = sampled;
                _currentWorldPoint = sampled;
                _hasFirstSample = true;
                return;
            }

            float distance = Vector2.Distance(sampled, _currentWorldPoint);

            if (distance >= minSampleDistance)
            {
                _lastWorldPoint = _currentWorldPoint;
                _currentWorldPoint = sampled;
            }
        }

        private void HandlePressStarted(InputAction.CallbackContext _)
        {
            IsSwiping = true;

            _currentScreenPoint = ReadPointerScreenPosition();
            Vector2 sampled = ScreenToWorld(_currentScreenPoint);

            _lastWorldPoint = sampled;
            _currentWorldPoint = sampled;
            _hasFirstSample = true;

            OnPressStarted?.Invoke();
        }

        private void HandlePressEnded(InputAction.CallbackContext _)
        {
            ResetSwipe();
            OnPressReleased?.Invoke();
        }

        private void ResetSwipe()
        {
            IsSwiping = false;
            _hasFirstSample = false;
        }

        private Vector2 ScreenToWorld(Vector2 screenPoint)
        {
            Ray ray = worldCamera.ScreenPointToRay(screenPoint);
            Plane gameplayPlane = new Plane(Vector3.forward, Vector3.zero);

            if (gameplayPlane.Raycast(ray, out float distance))
            {
                Vector3 worldPoint = ray.GetPoint(distance);
                return new Vector2(worldPoint.x, worldPoint.y);
            }

            Vector3 fallback = worldCamera.ScreenToWorldPoint(
                new Vector3(screenPoint.x, screenPoint.y, Mathf.Abs(worldCamera.transform.position.z))
            );

            return new Vector2(fallback.x, fallback.y);
        }

        private Vector2 ReadPointerScreenPosition()
        {
            if (Touchscreen.current != null)
            {
                foreach (var touch in Touchscreen.current.touches)
                {
                    if (touch.press.isPressed)
                    {
                        return touch.position.ReadValue();
                    }
                }
            }

            if (Mouse.current != null)
            {
                return Mouse.current.position.ReadValue();
            }

            return _positionAction.ReadValue<Vector2>();
        }
    }
}
