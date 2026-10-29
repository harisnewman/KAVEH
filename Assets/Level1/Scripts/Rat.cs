using UnityEngine;

namespace Kaveh
{
    /// <summary>موش که روی زمین به چپ و راست می‌رود، گاهی می‌ایستد و برمی‌گردد.</summary>
    public class Rat : KavehInteractable
    {
        public float patrolHalfWidth = 2f;
        public float speed = 1.1f;
        public Vector2 pauseRange = new Vector2(0.4f, 1.6f);
        public bool spriteFacesRight = true;
        public bool canMove = true;

        float minX, maxX, dir, pause, curSpeed;
        SpriteRenderer sr;

        void Start()
        {
            sr = GetComponent<SpriteRenderer>();
            minX = transform.position.x - patrolHalfWidth;
            maxX = transform.position.x + patrolHalfWidth;
            dir = Random.value < 0.5f ? -1f : 1f;
            curSpeed = speed * Random.Range(0.8f, 1.25f);
            ApplyFacing();
        }

        void Update()
        {
            if (!canMove) return;
            if (pause > 0f) { pause -= Time.deltaTime; return; }

            var p = transform.position;
            p.x += dir * curSpeed * Time.deltaTime;
            if (p.x >= maxX) { p.x = maxX; Turn(); }
            else if (p.x <= minX) { p.x = minX; Turn(); }
            else if (Random.value < 0.2f * Time.deltaTime)
            {
                // گاهی وسط راه می‌ایستد، و گاهی برمی‌گردد
                pause = Random.Range(pauseRange.x, pauseRange.y);
                if (Random.value < 0.5f) { dir = -dir; ApplyFacing(); }
            }
            transform.position = p;
        }

        void Turn()
        {
            dir = -dir;
            pause = Random.Range(pauseRange.x, pauseRange.y);
            curSpeed = speed * Random.Range(0.8f, 1.25f);
            ApplyFacing();
        }

        void ApplyFacing()
        {
            if (sr != null) sr.flipX = (dir > 0f) != spriteFacesRight;
        }
    }
}
