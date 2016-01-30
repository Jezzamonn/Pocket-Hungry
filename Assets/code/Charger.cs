using UnityEngine;

namespace Assets.code
{
    public class Charger : MonoBehaviour
    {

        public enum State
        {
            Idle,
            Agro
        }

        public State state = State.Idle;
        public float agroDist = 20;
        public Vector3 chargeDir;
        public float speed = 0;
        public float maxSpeed = 3f;
        public float moved = 0;
        public float count = 0;

        public void TryAgro(Player player)
        {
            float dist = Vector3.Distance(player.transform.position, transform.position);
            if (dist < agroDist)
            {
                state = State.Agro;
                chargeDir = (player.transform.position - transform.position).normalized;
                //transform.rotation = Quaternion.LookRotation(chargeDir);
                //chargeDir.Normalize();
            }
        }

        public void SpecialUpdate(Player player)
        {
            Rigidbody body = GetComponent<Rigidbody>();
            switch (state)
            {
                case State.Agro:
                    count += Time.fixedDeltaTime;
                    speed += 0.02f;
                    if (speed > maxSpeed)
                    {
                        speed = maxSpeed;
                    }
                    Quaternion newRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(chargeDir), 0.2f);
                    body.MoveRotation(newRot);
                    Vector3 newPos = transform.position + (speed * transform.forward);
                    body.MovePosition(newPos);
                    moved += speed;
                    if (moved > 25)
                    {
                        state = State.Idle;
                        moved = 0;
                    }
                    break;
                case State.Idle:
                    speed *= 0.9f;
                    body.MovePosition(transform.position + speed * transform.forward);
                    if (speed < 0.01f) {
                        TryAgro(player);
                    }
                    break;
            }
        }


    }
}
