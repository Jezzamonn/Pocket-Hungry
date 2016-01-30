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
        public List<Transform> points;
        /// <summary>
        /// The gap between each point in the trail
        /// </summary>
        public float pointGap = 1f;
        public float zipSpeed = 0.5f;
        public float curZipSpeed = 0f;
        public float followerDist = 1.5f;

        public float playerDist = 0f;

        public List<Transform> followers;

        public Trail(Main main)
        {
            this.main = main;
            points = new List<Transform>();
            followers = new List<Transform>();

            Transform pathStart = (Transform)UnityEngine.Object.Instantiate(main.cubePrefab, player.transform.position, Quaternion.identity);
            points.Add(pathStart);
        }

        public Player player {
            get
            {
                return main.player;
            }
        }

        public Vector3 GetPositionAt(float dist)
        {
            int index = (int)(dist / pointGap);
            float remainder = dist - (pointGap * index);
            Transform start = points[index];
            Transform end = player.transform;
            if (index + 1 < points.Count) // Near the end
            {
                end = points[index + 1];
            }
            return Vector3.MoveTowards(start.position, end.position, remainder);
        }

        public Quaternion GetRotationAt(float dist)
        {
            // Code duplication!
            int index = (int)(dist / pointGap);
            float remainder = (dist - (pointGap * index)) / pointGap;
            Transform start = points[index];
            Transform end = player.transform;
            if (index + 1 < points.Count) // Near the end
            {
                end = points[index + 1];
            }
            return Quaternion.Lerp(start.rotation, end.rotation, remainder);
        }

        public void UpdateDistance()
        {
            Transform end = points[points.Count - 1];
            float endDist = Vector3.Distance(end.position, player.transform.position);
            playerDist = (points.Count - 1) * pointGap + endDist;
        }

        public void TryGrowPath()
        {
            // TODO: Scale this with Time
            Transform end = points[points.Count - 1];
            if (Vector3.Distance(end.position, player.transform.position) > pointGap)
            {
                Vector3 newPos = Vector3.MoveTowards(end.position, player.transform.position, pointGap);
                Transform newPart = (Transform)UnityEngine.Object.Instantiate(main.cubePrefab, newPos, player.transform.rotation);
                points.Add(newPart);
            }

            //Transform end = trail[trail.Count - 1];
            //Transform newPart = (Transform)Instantiate(cubePrefab, end.position, end.rotation);
            //newPart.position += 0.5f * newPart.forward;
            //newPart.Rotate(new Vector3(0, Random.Range(-50, 50), 0));
            //trail.Add(newPart);
        }

        public void PullBack()
        {
            // TODO: Change this with time
            float zipAmt = curZipSpeed;
            while (zipAmt > 0)
            {
                Transform end = points[points.Count - 1];
                float dist = Vector3.Distance(end.position, player.transform.position);
                if (dist > zipAmt + 0.0001) // fudge factor to avoid rounding errors
                {
                    Vector3 newPos = Vector3.MoveTowards(player.transform.position, end.position, zipAmt);
                    player.transform.position = newPos;
                    break; // moved as much as we need to, exit the loop.
                }
                else
                {
                    player.transform.position = end.position;
                    zipAmt -= dist;
                    if (points.Count > 1)
                    {
                        UnityEngine.Object.Destroy(end.gameObject);
                        points.RemoveAt(points.Count - 1);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        public void UpdateFollowers()
        {
            int numFollowers = (int)Mathf.FloorToInt(playerDist / followerDist);
            while (numFollowers > followers.Count)
            {
                Transform follower = (Transform)UnityEngine.Object.Instantiate(main.followerPrefab);
                followers.Add(follower);
            }
            while (numFollowers < followers.Count)
            {
                Transform follower = followers[followers.Count - 1];
                UnityEngine.Object.Destroy(follower.gameObject);
                followers.RemoveAt(followers.Count - 1);
            }

            float remainder = playerDist - (followerDist * numFollowers);
            for (int i = 0; i < followers.Count; i ++)
            {
                float dist = followerDist * i + remainder;
                followers[i].position = GetPositionAt(dist);
                followers[i].rotation = GetRotationAt(dist);
            }
        }

        public void CheckPull()
        {
            if (Input.GetButton("Pull Back"))
            {
                curZipSpeed += 0.01f;
                if (curZipSpeed > zipSpeed)
                {
                    curZipSpeed = zipSpeed;
                }
                PullBack();
            }
            else
            {
                curZipSpeed = 0;
            }
        }
    }
}
