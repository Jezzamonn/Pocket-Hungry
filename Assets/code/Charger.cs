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
        public float agroDist = 10;
        public Vector3 chargeDir;
        public float speed = 0.5f;
        public float count = 0;

        public void TryAgro(Player player)
        {
            float dist = Vector3.Distance(player.transform.position, transform.position);
            if (dist < agroDist)
            {
                state = State.Agro;
                chargeDir = (player.transform.position - transform.position);
                transform.rotation = Quaternion.LookRotation(chargeDir);
                chargeDir.Normalize();
            }
        }

        public void SpecialUpdate(Player player)
        {
            switch (state)
            {
                case State.Agro:
                    count += Time.fixedDeltaTime;
                    transform.position += speed * chargeDir;
                    if (count > 1)
                    {
                        state = State.Idle;
                    }
                    break;
                case State.Idle:
                    TryAgro(player);
                    break;
            }
        }


    }
}
