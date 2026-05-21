using UnityEngine;

namespace SamuraiSlice
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class IngredientHalf : MonoBehaviour
    {
        [SerializeField] private float lifetime = 1.5f;

        private IngredientPool _pool;
        private Rigidbody2D _rb;
        private float _timeAlive;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        public void Configure(IngredientPool pool)
        {
            _pool = pool;
        }

        public void Launch(Vector3 position, Quaternion rotation, Vector2 velocity, float angularVelocity, bool flipX)
        {
            transform.SetPositionAndRotation(position, rotation);

            Vector3 s = transform.localScale;
            float absX = Mathf.Abs(s.x);
            transform.localScale = new Vector3(flipX ? -absX : absX, s.y, s.z);

            _timeAlive = 0f;
            gameObject.SetActive(true);
            _rb.linearVelocity = velocity;
            _rb.angularVelocity = angularVelocity;
        }

        private void Update()
        {
            _timeAlive += Time.deltaTime;
            if ((_timeAlive >= lifetime))
            {
                ReturnToPool();
            }
        }

        private void ReturnToPool()
        {
            if(_pool != null )
            {
                _pool.Halves.Release(this);
            } else
            {
                Destroy(gameObject);
            }
        }
    }

}