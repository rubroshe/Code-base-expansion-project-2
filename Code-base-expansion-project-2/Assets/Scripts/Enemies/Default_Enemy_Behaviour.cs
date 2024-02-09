using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Default_Enemy_Behaviour : MonoBehaviour
{
    public Gun_Controller gun;
    public float timeToStop = 1;
    public float speed = 1;
    public float TimeToResume = 1;

    public float minPlayerDist, maxPlayerDist;










    private GameObject player;
    private NavMeshAgent Agent;
    private Vector3 moveTarget;
    public enum EnemyStates
    {
        IDLE,
        STOP,
        FIRING,
        RESUME,
        PATROL,
        COMBAT,
        TURNTOFACE
    }

    public EnemyStates state;
    /*
     * Possible Enemy States
     *      Patroling
     *      Stopping
     *      Firing
     *      Resuming
     * */


    /*
     * Laser deets
     * need system to take damage from laser, and cooldown on damage
     * */

    // Start is called before the first frame update
    void Start()
    {
        gun = GetComponentInChildren<Gun_Controller>();
        GameObject[] Group = FindObjectsOfType<GameObject>();
        player = null;
        foreach (GameObject g in Group)
        {
            if (g.CompareTag("Player"))
                player = g;
        }
        if (player == null)
        {
            Debug.Log("Enemy can't find player");
            Destroy(transform.parent.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {




        //below is original slide and shoot commented out
        {
            /* Original Test Code (move back and forth and stop to fire bullets
            if (State == 0)
            {
                time += Time.deltaTime;
                float movZ = outersize * Mathf.Sin(time);
                Vector3 difference = new Vector3(0, 0, movZ);
                target = centerpos + difference;
                if(gun.State == 0)
                {
                    State = 1;
                }
            }
            else if(State == 1)
            {
                stoptime += Time.deltaTime;
                stoptime = Mathf.Clamp(stoptime, 0, timeToStop);
                time += Time.deltaTime * (timeToStop - stoptime) / (timeToStop);
                float movZ = outersize * Mathf.Sin(time);
                Vector3 difference = new Vector3(0, 0, movZ);
                target = centerpos + difference;
                if(stoptime == timeToStop)
                {
                    State = 2;
                    gun.Fire();
                    stoptime = 0;
                }
            }
            else if (State == 2)
            {
                if (gun.State < 2)
                {
                    State = 3;
                }
            }
            else if (State == 3)
            {
                ResumeTime += Time.deltaTime;
                ResumeTime = Mathf.Clamp(ResumeTime, 0, TimeToResume);
                time += Time.deltaTime * (ResumeTime) / (TimeToResume);
                float movZ = outersize * Mathf.Sin(time);
                Vector3 difference = new Vector3(0, 0, movZ);
                target = centerpos + difference;
                if (ResumeTime == timeToStop)
                {
                    State = 0;
                    ResumeTime = 0;
                }
            }
            transform.position = Vector3.Lerp(transform.position, target, speed);
            */
        }
    }

    private Vector3 GetTargetPos()
    {  


        return Vector3.zero;
    }

}
