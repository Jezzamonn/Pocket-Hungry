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
        public List<Transform> noTouchPoints;
        public int noTouchPointDist = 2;
        public int nextNoTouchPoint = 0;
        public int noTouchPointFreq = 12;
        /// <summary>
        /// The gap between each point in the trail
        /// </summary>
        public float pointGap = 0.1f;
        //public float zipSpeed = 0.5f;
        public float curZipSpeed = 0f;
        public float followerDist = 2.0f;
        public bool recovering = false;

        public int maxLength = 10;
        public int foodGotten = 0;

        public float playerDist = 0f;

        public List<Transform> followers;

        public Trail(Main main)
        {
            this.main = main;
            points = new List<TrailPoint>();
            followers = new List<Transform>();
            noTouchPoints = new List<Transform>();

            //Transform pathStart = (Transform)UnityEngine.Object.Instantiate(main.cubePrefab, player.transform.position, Quaternion.identity);
            TrailPoint pathStart = new TrailPoint
            {
                position = player.transform.position,
                rotation = Quaternion.identity
            };
            points.Add(pathStart);
        }

        internal void TotalUpdate(bool nightTime)
        {
            MovePlayer(nightTime);
            UpdateDistance();
            UpdateFollowers();
        }

        public Player player {
            get
            {
                return main.player;
            }
        }

        public float PlayerSpeed
        {
            get
            {
                return 0.01f * maxLength;
            }
        }

        public float ZipSpeed
        {
            get
            {
                return 0.05f * maxLength;
            }
        }

        public void MovePlayer(bool nightTime)
        {
            // try pull
            if (Input.GetButton("Pull Back") || nightTime)
            {
                SetLayer("NoCollide");
                curZipSpeed += 0.01f;
                if (curZipSpeed > ZipSpeed)
                {
                    curZipSpeed = ZipSpeed;
                }
                MoveBack();
            }
            else
            {
                curZipSpeed = 0;
                SetLayer("AntBody");
                if (player.pulledFood == null)
                {
                    player.Move(playerDist < maxLength * followerDist);
                    TryGrowPath();
                    TryAddMoreFollowers();
                }
            }
        }

        public void SetLayer(string name)
        {
            player.transform.SetLayer(name);
            foreach (var follower in followers)
            {
                follower.SetLayer(name);
            }
        }

        public Vector3 GetPositionAt(float dist)
        {
            int index = (int)(dist / pointGap);
            if (index < 0)
            {
                return points[0].position;
            }
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
            if (index < 0)
            {
                return points[0].rotation;
            }
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

                    if (nextNoTouchPoint < points.Count)
                    {
                        TrailPoint checkPoint = points[nextNoTouchPoint];
                        if (Vector3.Distance(checkPoint.position, player.transform.position) > noTouchPointDist)
                        {
                            Transform noTouchCube = (Transform)UnityEngine.Object.Instantiate(main.noTouchCube, checkPoint.position, checkPoint.rotation);
                            noTouchPoints.Add(noTouchCube);
                            checkPoint.cube = noTouchCube;
                            nextNoTouchPoint += noTouchPointFreq;
                        }
                    }
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
                        int cubePoint = nextNoTouchPoint - noTouchPointFreq;
                        if (cubePoint >= 0)
                        {
                            TrailPoint part = points[cubePoint];
                            if (Vector3.Distance(part.position, player.transform.position) < noTouchPointDist)
                            {
                                UnityEngine.Object.Destroy(part.cube.gameObject);
                                part.cube = null;
                                nextNoTouchPoint -= noTouchPointFreq;
                            }
                        }
                        points.RemoveAt(points.Count - 1);
                    }
                    else
                    {
                        // We've got stuff back home now!
                        if (player.pulledFood != null)
                        {
                            Transform food = player.pulledFood;
                            player.EndPull(food);
                            main.foods.Remove(food);
                            Food f = food.GetComponent<Food>();
                            foodGotten += f.ants;
                            UnityEngine.Object.Destroy(food.gameObject);
                            //maxLength = (int)(1.2 * maxLength) + 1;
                            //player.speed *= 1.2f;
                            //zipSpeed *= 1.2f;
                        }
                        break;
                    }
                }
            }
            TryRemoveFollowers();
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
                f.distBehind = lastFollowerDist + followerDist;
                followers.Add(follower);
                lastFollowerDist = f.distBehind;
            }
        }

        public void TryRemoveFollowers()
        {
            while (followers.Count > 0 && followers[followers.Count - 1].GetComponent<Follower>().distBehind >= playerDist)
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

        public void GrowUp()
        {
            maxLength += foodGotten;
            foodGotten = 0;
        }

    }
}
