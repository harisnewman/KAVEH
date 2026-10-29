using UnityEngine;

namespace Kaveh
{
    /// <summary>یک مگس که دور یک آبجکت (مثلاً سطل) می‌چرخد. هر مگس یک آبجکت مستقل است.</summary>
    public class Fly : MonoBehaviour
    {
        public Transform center;
        public Vector2 offset = new Vector2(0f, 0.9f);
        public float radiusX = 0.7f;
        public float radiusY = 0.4f;
        public float angularSpeed = 3f;
        public float phase;
        public float jitter = 0.12f;

        float angle;
        float seed;

        void Start()
        {
            angle = phase;
            seed = Random.value * 100f;
        }

        void Update()
        {
            angle += angularSpeed * Time.deltaTime;
            Vector3 c = (center != null ? center.position : Vector3.zero) + (Vector3)offset;
            float nx = (Mathf.PerlinNoise(seed, Time.time * 1.3f) - 0.5f) * 2f * jitter;
            float ny = (Mathf.PerlinNoise(seed + 10f, Time.time * 1.1f) - 0.5f) * 2f * jitter;
            float rx = radiusX * (1f + 0.25f * Mathf.Sin(angle * 0.37f + seed));
            transform.position = c + new Vector3(Mathf.Cos(angle) * rx + nx, Mathf.Sin(angle * 1.3f) * radiusY + ny, 0f);
            transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Sin(Time.time * 9f + seed) * 14f);
        }
    }
}
