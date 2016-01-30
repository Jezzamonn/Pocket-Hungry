using UnityEngine;

namespace Assets.code
{
    public class Charger : MonoBehaviour
    {

        public enum State
        {
            Idle,
            Agro,
            Backing
        }

        public Vector3 start;
        public State state = State.Idle;
        public float agroDist = 20;
        public float speed = 0;
        public float maxSpeed = 3f;
        public float moved = 0;

        public void TryAgro(Player player)
        {
            float dist = Vector3.Distance(player.transform.position, transform.position);
            if (dist < agroDist)
            {
                state = State.Agro;
                //transform.rotation = Quaternion.LookRotation(chargeDir);
                //chargeDir.Normalize();
            }
        }

        public void SpecialUpdate(Player player)
        {
            //Rigidbody body = GetComponent<Rigidbody>();
            if (state == State.Agro) {
                speed += 0.02f;
                if (speed > maxSpeed)
                {
                    speed = maxSpeed;
                }
                //Quaternion newRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(chargeDir), 0.2f);
                //body.MoveRotation(newRot);
                Vector3 newPos = transform.position + (speed * transform.forward);
                transform.position = newPos;
                //body.MovePosition(newPos);
                moved += speed;
                if (moved > 25)
                {
                    state = State.Backing;
                    moved = 0;
                }
            }
            else if (state == State.Idle)
            {
                //speed *= 0.9f;
                //Vector3 newPos = transform.position + speed * transform.forward;
                //body.MovePosition(transform.position + speed * transform.forward);
                //transform.position = newPos;
                //if (speed < 0.01f)
                //{
                    TryAgro(player);
                //}
            }
            else if (state == State.Backing)
            {
                Vector3 pos = Vector3.MoveTowards(transform.position, start, 0.1f);
                transform.position = pos;
                //body.MovePosition(pos);
                if (Vector3.Distance(start, transform.position) < 0.01f)
                {
                    state = State.Idle;
                }
            }
        }


    }
}
