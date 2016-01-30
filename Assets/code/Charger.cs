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

        public void TryAgro(Player player)
        {
            float dist = Vector3.Distance(player.transform.position, transform.position);
            if (dist < agroDist)
            {
                state = State.Agro;
            }
        }


    }
}
