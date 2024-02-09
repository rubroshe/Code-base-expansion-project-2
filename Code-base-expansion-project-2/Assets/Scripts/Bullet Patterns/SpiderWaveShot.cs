using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderWaveShot : MonoBehaviour
{
    public Gun_Controller gunner;
    public GameObject bulleter;
    public float spread1, spread2, spread3;
    public float speed1, speed2, speed3;
    public int count1, count2, count3;
    // Start is called before the first frame update
    void Start()
    {
        GameObject Spawned = Instantiate(bulleter, transform.position, Quaternion.identity, transform.parent);
        Spawned.GetComponent<ShotgunBulleter>().Spread = spread1;
        Spawned.GetComponent<ShotgunBulleter>().BulletCount = count1;
        Spawned.GetComponent<ShotgunBulleter>().BulletSpeeds = speed1;
        Spawned = Instantiate(bulleter, transform.position, Quaternion.identity, transform.parent);
        Spawned.GetComponent<ShotgunBulleter>().Spread = spread2;
        Spawned.GetComponent<ShotgunBulleter>().BulletCount = count2;
        Spawned.GetComponent<ShotgunBulleter>().BulletSpeeds = speed2;
        Spawned = Instantiate(bulleter, transform.position, Quaternion.identity, transform.parent);
        Spawned.GetComponent<ShotgunBulleter>().Spread = spread3;
        Spawned.GetComponent<ShotgunBulleter>().BulletCount = count3;
        Spawned.GetComponent<ShotgunBulleter>().BulletSpeeds = speed3;
        if (transform.parent != null)
            gunner = transform.parent.parent.GetComponentInChildren<Gun_Controller>();
        gunner.CompleteFire();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
