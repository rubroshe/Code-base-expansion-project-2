using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_Controller : MonoBehaviour
{
    [Tooltip("Parent of where this enemy stores its bullets")]
    [SerializeField] private GameObject BulletHolder;
    [Tooltip("Minimum Time between shots")] //if all it's bullets go offscreen before this long has passed, we'll keep waiting until this timer is also done.
    [SerializeField] private float CooldownTime;
    [Tooltip("Number of bullets left on screen before firing again")] // if there's too many shots around at once, take a chill pill
    [SerializeField] private int CooldownCount;

    [SerializeField] public int State = 0; //0 is idle, 1 is cooldown, 2 is firing

    [Tooltip("List of all the bullet patterns")]
    [SerializeField] private GameObject[] BulletPatterns;
    [Tooltip("The index of this enemys bullet pattern")]
    [SerializeField] public int BulletIndex;

    private float Cooling = 0;

    /*
     * Possible Gun States
     *      Idle
     *      Firing
     *      Cooldown
     * */
    // Start is called before the first frame update
    private void Start()
    {
        if (BulletIndex < 0 || BulletIndex >= BulletPatterns.Length)
            BulletIndex = 0;
        Cooling = -CooldownTime;
        State = 1;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        switch(State)
        {
            case 0:
                break;
            case 1:
                Cooling += Time.deltaTime;
                if(Cooling > CooldownTime && CooldownCount > BulletHolder.transform.GetComponentsInChildren<MeshFilter>().Length)
                {
                    State = 0;
                    Cooling = 0;
                }
                break;
            case 2:
                break;
            default:
                break;
        }
    }

    public void Fire(int NewIndex, bool immediate = false)
    {
        //fire gun and then change pattern to NewIndex
        //change index and then fire if immediate is true
        if(immediate)
        {
            if(NewIndex > 0 && NewIndex < BulletPatterns.Length)
            {
                BulletIndex = NewIndex;
            }
            Fire();
        }
        else
        {
            Fire();
            if (NewIndex > 0 && NewIndex < BulletPatterns.Length)
            {
                BulletIndex = NewIndex;
            }
        }
    }
    public void Fire()
    {
        State = 2;
        GameObject newBullets = Instantiate(BulletPatterns[BulletIndex], transform.position, Quaternion.identity, BulletHolder.transform);
    }

    public void CompleteFire()
    {
        State = 1;
    }

    internal void FireOnDelay(float v)
    {
        Invoke("Fire", v);
    }
}
