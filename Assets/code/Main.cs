﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.code;

public class Main : MonoBehaviour {

    public Camera cam;
    public Transform cubePrefab;
    public Transform followerPrefab;
    public Transform foodPrefab;
    public Player player;

    public Trail trail;
    public List<Transform> foods;

	// Use this for initialization
	void Start () {
        trail = new Trail(this);
        foods = new List<Transform>();
        for (int i = 0; i < 5; i ++)
        {
            Vector2 pos;
            do
            {
                pos = Random.insideUnitCircle;
            }
            while (pos.sqrMagnitude < 0.5f);
            Vector3 pos3d = 30 * new Vector3(pos.x, 0, pos.y);

            Transform newFood = (Transform)Instantiate(foodPrefab, pos3d, Quaternion.identity);
            foods.Add(newFood);
        }
    }

	// Update is called once per frame
	void Update () {
        trail.TotalUpdate();
        player.CheckFood();
        UpdateCamera();

        if (Input.GetButtonDown("Test"))
        {
            int index = Random.Range(0, trail.followers.Count);
            trail.followers[index].GetComponent<Follower>().FlipOut();
            trail.followers.RemoveAt(index);
        }
    }

    void UpdateCamera()
    {
        Vector3 pos = 0.7f * player.transform.position;
        // add look amount
        pos += 0.1f * player.transform.position.magnitude * player.transform.forward;
        pos += -(10 + 0.6f * player.transform.position.magnitude) * cam.transform.forward;
        cam.transform.position = Vector3.Lerp(cam.transform.position, pos, 0.05f);
    }



}
