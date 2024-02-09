using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomScript : MonoBehaviour
{
    [Header("Needed info")]
    public GameObject[] plannedEnemies;
    public int MaxEnemies = 3;
    public bool bossfight = false;
    public bool roomActive = false;
    
    [Header("dont worry about it")]
    public int enemyIndex;
    public PlayerMovement player;
    public List<GameObject> spawnedEnemies;
    
    public GameObject[] allSpawners;
    public List<GameObject> spawners;
    public GameObject[] allDoors;
    public List<GameObject> doors;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        player = FindObjectOfType<PlayerMovement>();
            
        float xDist = gameObject.transform.localScale.x/2;
        float zDist = gameObject.transform.localScale.z/2;
        //Gizmos.DrawCube(transform.position,new Vector3(xDist,50,zDist));
        
        allSpawners = GameObject.FindGameObjectsWithTag("Spawner");
        for (int i = 0; i < allSpawners.Length; i++)
        {
            //float xDist = Vector3.Distance()
            if (allSpawners[i].transform.position.x - transform.position.x > -xDist && allSpawners[i].transform.position.x - transform.position.x < xDist &&
                allSpawners[i].transform.position.z - transform.position.z > -zDist && allSpawners[i].transform.position.z - transform.position.z < zDist)
            {
                spawners.Add(allSpawners[i]);
            }
        }
        
        allDoors = GameObject.FindGameObjectsWithTag("Door");
        for (int i = 0; i < allDoors.Length; i++)
        {
            if (allDoors[i].transform.position.x - transform.position.x > -xDist && allDoors[i].transform.position.x - transform.position.x < xDist &&
                allDoors[i].transform.position.z - transform.position.z > -zDist && allDoors[i].transform.position.z - transform.position.z < zDist)
            {
                doors.Add(allDoors[i]);
                
            }
        }

        for (int i = 0; i < doors.Count; i++)
        {
            doors[i].SetActive(false);
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (roomActive)
        {
            for (int j = 0; j < spawnedEnemies.Count; j++)
            {
                if (spawnedEnemies[j].gameObject == null)
                {
                    spawnedEnemies.Remove(spawnedEnemies[j]);
                }
            }
            
            if (spawnedEnemies.Count < MaxEnemies && enemyIndex < plannedEnemies.Length)
            {
                spawnEnemy();
                enemyIndex++;
                if (bossfight && enemyIndex == plannedEnemies.Length) enemyIndex = 0;
            }

            if (spawnedEnemies.Count ==                                        0)
            {
                roomActive = false;
                for (int i = 0; i < doors.Count; i++)
                {
                    doors[i].SetActive(false);
                }
            }
        }
    }

    public void spawnEnemy()
    {
        float[] distancesToPlayer = new float[spawners.Count];
        float[] spawnChances = new float[spawners.Count];
        float totalDist = 0;
        Vector3 spawnPos = Vector3.zero;

        for (int i = 0; i < spawners.Count; i++)
        {
            distancesToPlayer[i] = (spawners[i].transform.position - player.transform.position).magnitude; // finds the distances to player and adds them up
            totalDist += distancesToPlayer[i] *1f;
        }

        float rand = Random.Range(0, 1f);
        //print(rand + "rand");
        for (int i = 0; i < spawners.Count; i++)
        {
            spawnChances[i] = distancesToPlayer[i] / totalDist * 1f; // finds out the chance of using a particular spawner, greater distances have higher chances
            float thisChance = spawnChances[i];
            //print(thisChance + " spawn chance un, added for spawner " + i);
            for (int j = 0; j < i; j++)
            {
                thisChance += spawnChances[j]; // if rand is less than this chance + all chances below this, so for a 60% chance is after a 10% chance, use that one instead
            }
           // print(thisChance + "b");
            if (rand < thisChance)
            {
                spawnPos = spawners[i].transform.position;
               // print("spawned at spawner "+i);
                break;
            }
        }
        
        GameObject newEnemy = Instantiate(plannedEnemies[enemyIndex], spawnPos, Quaternion.Euler(Vector3.back));
        spawnedEnemies.Add(newEnemy);
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.tag == "Player")
        {
           // print("Activate");
            for (int i = 0; i < doors.Count; i++)
            {
                doors[i].SetActive(true);
            }

            roomActive = true;
            if (bossfight && roomActive)
            {
                UIScript ui = FindObjectOfType<UIScript>();
                ui.bossFight = true;
            }
        }
    }
}
