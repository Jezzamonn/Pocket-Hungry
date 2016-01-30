using UnityEngine;

namespace Assets.code
{
    public class Follower : MonoBehaviour
    {
        public float distBehind = 0;
        public bool attached = true;
        public float deadCount = 0;

        void Update()
        {
            //if (!attached)
            //{
            //    deadCount += Time.deltaTime;
            //    if (deadCount > 3)
            //    {
            //        Destroy(gameObject);
            //    }
            //}
        }

        public void FlipOut()
        {
            Destroy(gameObject);
            //Rigidbody bod = GetComponent<Rigidbody>();
            //bod.drag = 1;
            //bod.angularDrag = 1;
            //Vector2 sideways;
            //do {
            //    sideways = Random.insideUnitCircle;
            //}
            //while (sideways.SqrMagnitude() < 0.2f);
            //bod.force
            //deadCount = 0;
            //attached = false;
        }
    }
}
