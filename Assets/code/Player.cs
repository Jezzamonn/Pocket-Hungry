using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.code
{
    public class Player : MonoBehaviour
    {

        public Transform pullFood;
        public float speed = 0.5f;

        void Update()
        {
            //Move();
            //if (pullFood != null) {
            //    pullFood.position = Vector3.MoveTowards(transform.position, pullFood.position, 1f);
            //}
        }

        public void CheckFood()
        {
            if (!Input.GetButton("Grab") && pullFood != null)
            {
                pullFood.SetParent(null);
                if (pullFood != null)
                {
                    foreach (Transform trans in pullFood.gameObject.GetComponentsInChildren<Transform>(true))
                    {
                        trans.gameObject.layer = LayerMask.NameToLayer("Default");
                    }
                }
                pullFood = null;
            }
        }

        public void Move(bool canMove)
        {
            float xDir = Input.GetAxis("Horizontal");
            float yDir = Input.GetAxis("Vertical");
            Vector3 moveDir = Vector3.ClampMagnitude(new Vector3(xDir, 0, yDir), 1);
            Rigidbody body = GetComponent<Rigidbody>();
            if (moveDir.sqrMagnitude > 0)
            {
                body.MoveRotation(Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDir), 0.2f));
                if (canMove)
                {
                    //body.MovePosition(transform.position + speed * moveDir);
                    body.MovePosition(transform.position + transform.forward * speed * moveDir.magnitude);
                }
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (pullFood == null && collision.transform != null) {
                pullFood = collision.transform;
                pullFood.SetParent(transform);
                if (pullFood != null)
                {
                    foreach (Transform trans in pullFood.gameObject.GetComponentsInChildren<Transform>(true))
                    {
                        trans.gameObject.layer = LayerMask.NameToLayer("AntBody");
                    }
                }
            }
        }
    }
}
