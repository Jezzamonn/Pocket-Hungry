using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.code
{
    public class Player : MonoBehaviour
    {

        public Transform pullableFood;
        public Transform pulledFood;
        public Trail trail;

        void Update()
        {
            //Move();
            //if (pullFood != null) {
            //    pullFood.position = Vector3.MoveTowards(transform.position, pullFood.position, 1f);
            //}
        }

        public void CheckFood()
        {
            if (Input.GetButtonDown("Grab"))
            {
                if (pulledFood != null)
                {
                    // let go
                    EndPull(pulledFood);
                }
                else if (pullableFood)
                {
                    // grab it
                    BeginPull(pullableFood);
                }
            }
        }

        public bool Move(bool canMove)
        {
            float xDir = Input.GetAxis("Horizontal");
            float yDir = Input.GetAxis("Vertical");
            Vector3 moveDir = Vector3.ClampMagnitude(new Vector3(xDir, 0, yDir), 1);
            Rigidbody body = GetComponent<Rigidbody>();
            if (moveDir.sqrMagnitude > 0)
            {
                //if (transform.position.y < 0)
                //{
                //    transform.position += speed * moveDir.magnitude * Vector3.up;
                //    if (transform.position.y > 0)
                //    {
                //        transform.position = new Vector3(transform.position.x, transform.position.z);
                //    }
                //}
                //else
                {
                    body.MoveRotation(Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDir), 0.2f));
                    if (canMove)
                    {
                        //body.MovePosition(transform.position + speed * moveDir);
                        body.MovePosition(transform.position + transform.forward * trail.PlayerSpeed * moveDir.magnitude);
                        return true;
                    }
                }
            }
            return false;
        }

        public void BeginPull(Transform pullee)
        {
            if (pulledFood)
            {
                EndPull(pulledFood);
            }
            pullee.SetParent(this.transform);
            pulledFood = pullee;
        }

        public void EndPull(Transform pullee)
        {
            pullee.SetParent(null);
            pullee.SetLayer("Default");
            pulledFood = null;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.transform != null)
            {
                if (collision.transform.tag == "Food" && pullableFood == null)
                {
                    pullableFood = collision.transform;
                }
                //else if (collision.transform.tag == "BlockingCube")
                //{
                //    trail.PullBack(0.1f);
                //}
            }

        }

        void OnCollisionExit(Collision collision)
        {
            pullableFood = null;
        }

        void OnTriggerEnter(Collider other)
        {
            trail.maxLength -= 1;
            trail.PullBack(trail.followerDist);
            //Destroy(other.gameObject);
        }

    }
}
