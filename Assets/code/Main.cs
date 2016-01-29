using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.code;

public class Main : MonoBehaviour {

    public Transform cubePrefab;
    public Transform followerPrefab;

    public List<Transform> trail;
    public float trailGap = 2f;
    public float zipSpeed = 1f;
    public float followerDist = 1.5f;
    public float playerDist = 0f;

    public Player player;

	// Use this for initialization
	void Start () {
        trail = new List<Transform>();
        Transform pathStart = (Transform)Instantiate(cubePrefab, player.transform.position, Quaternion.identity);
        trail.Add(pathStart);
    }
	
	// Update is called once per frame
	void Update () {
        player.Move();
        TryGrowPath();
        if (Input.GetButton("Pull Back"))
        {
            PullBack();
        }
    }

    void UpdateDistance()
    {
        Transform end = trail[trail.Count - 1];
        float endDist = Vector3.Distance(end.position, player.transform.position);
        playerDist = trail.Count * trailGap + endDist;
    }

    void TryGrowPath()
    {
        // TODO: Scale this with Time
        Transform end = trail[trail.Count - 1];
        if (Vector3.Distance(end.position, player.transform.position) > trailGap)
        {
            Vector3 newPos = Vector3.MoveTowards(end.position, player.transform.position, trailGap);
            Transform newPart = (Transform)Instantiate(cubePrefab, newPos, Quaternion.identity);
            trail.Add(newPart);
        }

        //Transform end = trail[trail.Count - 1];
        //Transform newPart = (Transform)Instantiate(cubePrefab, end.position, end.rotation);
        //newPart.position += 0.5f * newPart.forward;
        //newPart.Rotate(new Vector3(0, Random.Range(-50, 50), 0));
        //trail.Add(newPart);
    }

    void PullBack()
    {
        // TODO: Change this with time
        float zipAmt = zipSpeed;
        while (zipAmt > 0)
        {
            Transform end = trail[trail.Count - 1];
            float dist = Vector3.Distance(end.position, player.transform.position);
            if (dist > zipAmt + 0.001) // fudge factor to avoid rounding errors
            {
                Vector3 newPos = Vector3.MoveTowards(end.position, player.transform.position, zipAmt);
                player.transform.position = newPos;
                break; // moved as much as we need to, exit the loop.
            }
            else
            {
                player.transform.position = end.position;
                zipAmt -= dist;
                if (trail.Count > 1)
                {
                    Destroy(end.gameObject);
                    trail.RemoveAt(trail.Count - 1);
                }
                else
                {
                    break;
                }
            }
        }
    }

    void UpdateFollowers()
    {

    }
}
