using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.code;

public class Main : MonoBehaviour {

    public Camera cam;
    public Transform followerPrefab;
    public Transform foodPrefab;
    public Transform enemyPrefab;
    public Player player;
    public Transform noTouchCube;

    public Trail trail;
    public List<Transform> foods;
    public List<Transform> enemies;

	// Use this for initialization
	void Start () {
        trail = new Trail(this);
        player.trail = trail;
        foods = new List<Transform>();
        for (int i = 0; i < 5; i ++)
        {
            Vector2 pos;
            do
            {
                pos = Random.insideUnitCircle;
            }
            while (pos.sqrMagnitude < 0.2f);
            Vector3 pos3d = 30 * new Vector3(pos.x, 0, pos.y);

            Transform newFood = (Transform)Instantiate(foodPrefab, pos3d, Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up));
            foods.Add(newFood);
        }

        for (int i = 0; i < 6; i ++)
        {
            Vector2 pos;
            do
            {
                pos = Random.insideUnitCircle;
            }
            while (pos.sqrMagnitude < 0.3f);
            Vector3 pos3d = 40 * new Vector3(pos.x, 0, pos.y);

            Transform enemy = (Transform)Instantiate(enemyPrefab, pos3d, Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up));
            Charger c = enemy.GetComponent<Charger>();
            c.start = enemy.transform.position;
            enemies.Add(enemy);
        }
    }

	// Update is called once per frame
	void Update () {
        trail.TotalUpdate();
        player.CheckFood();
        UpdateCamera();
        foreach (Transform enemy in enemies)
        {
            Charger c = enemy.GetComponent<Charger>();
            c.SpecialUpdate(player);
        }

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
