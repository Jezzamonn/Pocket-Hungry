using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.code
{
    public class Trail
    {
        public Main main;

        // TODO: Change this to a list of points
        public List<TrailPoint> points;
        /// <summary>
        /// The gap between each point in the trail
        /// </summary>
        public float pointGap = 0.1f;
        public float zipSpeed = 0.5f;
        public float curZipSpeed = 0f;
        public float followerDist = 2.5f;
        public bool recovering = false;

        public int maxLength = 10;

        public float playerDist = 0f;

        public List<Transform> followers;

        public Trail(Main main)
        {
            this.main = main;
            points = new List<TrailPoint>();
            followers = new List<Transform>();

            //Transform pathStart = (Transform)UnityEngine.Object.Instantiate(main.cubePrefab, player.transform.position, Quaternion.identity);
            TrailPoint pathStart = new TrailPoint
            {
                position = player.transform.position,
                rotation = Quaternion.identity
            };
            points.Add(pathStart);
        }

        internal void TotalUpdate()
        {
            MovePlayer();
            UpdateDistance();
            UpdateFollowers();
        }

        public Player player {
            get
            {
                return main.player;
            }
        }

        public void MovePlayer()
        {
            // try pull
            bool pullResult = CheckPull();
            if (pullResult)
            {
                TryRemoveFollowers();
                player.transform.SetLayer("NoCollide");
                foreach (var follower in followers)
                {
                    follower.SetLayer("NoCollide");
                }
            }
            else
            {
                player.transform.SetLayer("AntBody");
                foreach (var follower in followers)
                {
                    follower.SetLayer("AntBody");
                }
                if (player.pulledFood == null)
                {
                    player.Move(followers.Count < maxLength);
                    TryGrowPath();
                    TryAddMoreFollowers();
                }
            }
        }

        public Vector3 GetPositionAt(float dist)
        {
            int index = (int)(dist / pointGap);
            float remainder = dist - (pointGap * index);
            Vector3 start = points[index].position;
            Vector3 end = player.transform.position;
            if (index + 1 < points.Count) // Near the end
            {
                end = points[index + 1].position;
            }
            return Vector3.MoveTowards(start, end, remainder);
        }

        public Quaternion GetRotationAt(float dist)
        {
            // Code duplication!
            int index = (int)(dist / pointGap);
            float remainder = (dist - (pointGap * index)) / pointGap;
            Quaternion start = points[index].rotation;
            Quaternion end = player.transform.rotation;
            if (index + 1 < points.Count) // Near the end
            {
                end = points[index + 1].rotation;
            }
            return Quaternion.Lerp(start, end, remainder);
        }

        public void UpdateDistance()
        {
            TrailPoint end = points[points.Count - 1];
            float endDist = Vector3.Distance(end.position, player.transform.position);
            playerDist = (points.Count - 1) * pointGap + endDist;
        }

        public void TryGrowPath()
        {
            // TODO: Scale this with Time
            while (true)
            {
                TrailPoint end = points[points.Count - 1];
                if (Vector3.Distance(end.position, player.transform.position) > pointGap)
                {
                    Vector3 newPos = Vector3.MoveTowards(end.position, player.transform.position, pointGap);
                    //Transform newPart = (Transform)UnityEngine.Object.Instantiate(main.cubePrefab, newPos, player.transform.rotation);
                    TrailPoint newPart = new TrailPoint
                    {
                        position = newPos,
                        rotation = player.transform.rotation
                    };
                    points.Add(newPart);
                }
                else
                {
                    break;
                }
            }

            //Transform end = trail[trail.Count - 1];
            //Transform newPart = (Transform)Instantiate(cubePrefab, end.position, end.rotation);
            //newPart.position += 0.5f * newPart.forward;
            //newPart.Rotate(new Vector3(0, Random.Range(-50, 50), 0));
            //trail.Add(newPart);
        }

        public void MoveBack()
        {
            PullBack(curZipSpeed);
        }

        public void PullBack(float amt)
        {
            float zipAmt = amt;
            while (zipAmt > 0)
            {
                TrailPoint end = points[points.Count - 1];
                float dist = Vector3.Distance(end.position, player.transform.position);
                if (dist > zipAmt + 0.0001) // fudge factor to avoid rounding errors
                {
                    Vector3 newPos = Vector3.MoveTowards(player.transform.position, end.position, zipAmt);
                    player.transform.position = newPos;
                    player.transform.rotation = Quaternion.Lerp(player.transform.rotation, end.rotation, zipAmt / pointGap);
                    break; // moved as much as we need to, exit the loop.
                }
                else
                {
                    player.transform.position = end.position;
                    player.transform.rotation = end.rotation;
                    zipAmt -= dist;
                    if (points.Count > 1)
                    {
                        //UnityEngine.Object.Destroy(end.gameObject);
                        points.RemoveAt(points.Count - 1);
                    }
                    else
                    {
                        // We've got stuff back home now!
                        if (player.pulledFood != null)
                        {
                            Transform food = player.pulledFood;
                            player.EndPull(food);
                            UnityEngine.Object.Destroy(food.gameObject);
                            maxLength = (int)(1.2 * maxLength) + 1;
                            player.speed *= 1.1f;
                            zipSpeed *= 1.1f;
                        }
                        break;
                    }
                }
            }
        }

        public void TryAddMoreFollowers()
        {
            float lastFollowerDist = 0;
            if (followers.Count > 0)
            {
                lastFollowerDist = followers[followers.Count - 1].GetComponent<Follower>().distBehind;
            }
            while (lastFollowerDist + followerDist < playerDist)
            {
                Transform follower = (Transform)UnityEngine.Object.Instantiate(main.followerPrefab);
                Follower f = follower.GetComponent<Follower>();
                f.distBehind = followerDist * (followers.Count + 1);
                followers.Add(follower);
                lastFollowerDist = f.distBehind;
            }
        }

        public void TryRemoveFollowers()
        {
            while (followers.Count > 0 && followers[followers.Count - 1].GetComponent<Follower>().distBehind > playerDist)
            {
                Transform follower = followers[followers.Count - 1];
                UnityEngine.Object.Destroy(follower.gameObject);
                followers.RemoveAt(followers.Count - 1);
            }
        }

        public void UpdateFollowers()
        {
            foreach (var follower in followers)
            {
                Follower f = follower.GetComponent<Follower>();
                float dist = playerDist - f.distBehind;
                follower.position = GetPositionAt(dist);
                follower.rotation = GetRotationAt(dist);
            }

            //float remainder = playerDist - (followerDist * numFollowers);
            //for (int i = 0; i < followers.Count; i ++)
            //{
            //    float dist = followerDist * i + remainder;
            //    followers[i].position = GetPositionAt(dist);
            //    followers[i].rotation = GetRotationAt(dist);
            //}
        }

        public bool CheckPull()
        {
            if (Input.GetButton("Pull Back"))
            {
                curZipSpeed += 0.01f;
                if (curZipSpeed > zipSpeed)
                {
                    curZipSpeed = zipSpeed;
                }
                MoveBack();
                return true;
            }
            else
            {
                curZipSpeed = 0;
                return false;
            }
        }
    }
}
