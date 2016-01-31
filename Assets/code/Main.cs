using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.code;

public class Main : MonoBehaviour {

    public Camera cam;
    public Transform followerPrefab;
    public Transform[] foodPrefabs;
    public Transform enemyPrefab;
    public Player player;
    public Transform noTouchCube;

    public Trail trail;
    public List<Transform> foods;
    public List<Transform> enemies;

    public float curTime = 0;
    public float dayLength = 20;
    public SpriteRenderer shadow;

	// Use this for initialization
	void Start () {
        trail = new Trail(this);
        player.trail = trail;
        foods = new List<Transform>();
        GoToNextDay();
    }

	// Update is called once per frame
	void Update () {
        if (curTime > dayLength && trail.playerDist <= 0.01)
        {
            // next day
            GoToNextDay();
        }

        UpdateTime();

        trail.TotalUpdate(curTime > dayLength);
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

    void UpdateTime()
    {
        // TODO: Change this with a variable thing once delat times get added elsewhere
        curTime += Time.fixedDeltaTime;
        float alpha = curTime / dayLength; //0.5f + 0.5f * Mathf.Cos(curTime);
        if (alpha > 1)
        {
            alpha = 1f;
        }
        shadow.color = new Color(1, 1, 1, alpha);
    }

    void GoToNextDay()
    {
        curTime = 0;
        foreach (Transform enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
        foreach (Transform food in foods)
        {
            Destroy(food.gameObject);
        }
        enemies.Clear();
        foods.Clear();

        for (int i = 0; i < foodPrefabs.Length; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                Vector2 pos;
                do
                {
                    pos = Random.insideUnitCircle;
                }
                while (pos.sqrMagnitude < 0.6f);
                Transform prefab = foodPrefabs[i];
                float dist = prefab.GetComponent<Food>().distance;
                Vector3 pos3d = dist * new Vector3(pos.x, 0, pos.y);
                Transform newFood = (Transform)Instantiate(prefab, pos3d, Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up));
                foods.Add(newFood);
            }
        }

        for (int i = 0; i < 6; i++)
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

        trail.SetLayer("AntBody");
        trail.GrowUp();
    }
}
