using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.code
{
    public class Player : MonoBehaviour
    {

        public float speed = 0.1f;

        void Update()
        {
            //Move();
        }

        public void Move()
        {
            float xDir = Input.GetAxis("Horizontal");
            float yDir = Input.GetAxis("Vertical");
            Vector3 moveDir = Vector3.ClampMagnitude(new Vector3(xDir, 0, yDir), 1);
            Rigidbody body = GetComponent<Rigidbody>();
            body.MovePosition(transform.position + speed * moveDir);
            if (moveDir.sqrMagnitude > 0)
            {
                body.MoveRotation(Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDir), 0.2f));
            }
        }
    }
}
