using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.code;

public class Main : MonoBehaviour {

    public Transform cubePrefab;
    public Transform followerPrefab;
    public Player player;

    public Trail trail;

	// Use this for initialization
	void Start () {
        trail = new Trail(this);
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
