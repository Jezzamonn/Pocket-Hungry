using UnityEngine;

namespace Assets.code
{
    public class Follower : MonoBehaviour
    {
        public float distBehind = 0;
        public bool attached = true;

        public void FlipOut()
        {
            Rigidbody bod = GetComponent<Rigidbody>();
            bod.drag = 1;
            bod.angularDrag = 1;
            Vector2 sideways;
            do {
                sideways = Random.insideUnitCircle;
            }
            while (sideways.SqrMagnitude() < 0.2f);
            bod.AddRelativeForce(2 * new Vector3(sideways.x, 1, sideways.y));
            bod.AddRelativeTorque(Random.onUnitSphere);
        }
    }
}
