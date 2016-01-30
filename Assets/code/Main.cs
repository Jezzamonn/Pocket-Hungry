using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.code;

public class Main : MonoBehaviour {

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
            Vector3 pos3d = 10 * new Vector3(pos.x, 0, pos.y);

            Transform newFood = (Transform)Instantiate(foodPrefab, pos3d, Quaternion.identity);
            foods.Add(newFood);
        }
    }

	// Update is called once per frame
	void Update () {
        trail.player.Move();
        trail.TryGrowPath();
        trail.CheckPull();
        trail.UpdateDistance();
        trail.UpdateFollowers();
    }



}
