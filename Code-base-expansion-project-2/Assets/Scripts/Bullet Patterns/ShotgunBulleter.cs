using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunBulleter : MonoBehaviour
{
    [Header("This Bullet Pattern Script Fires projectiles in a fan arc, with the center aimed at the player")]
    public float Spread;
    public float BulletSpeeds;
    public int BulletCount;
    public GameObject BulletType;
    [Header("This entity spawned these bullets (assigned by this script)")]
    public Gun_Controller Spawner;
    public bool overrider = false;
    // Start is called before the first frame update
    void Start()
    {
        if (transform.parent != null)
            Spawner = transform.parent.parent.GetComponentInChildren<Gun_Controller>();
        GameObject[] Group = FindObjectsOfType<GameObject>();
        GameObject player = null;
        foreach (GameObject g in Group)
        {
            if (g.CompareTag("Player"))
                player = g;
        }
        if (player == null)
        {
            //Debug.Log("Bullet can't find player");
            Destroy(this.gameObject);
        }
        if (!overrider)
        {
            Vector3 Direction = player.transform.position - transform.position;
            Direction.y = 0;
            Direction.Normalize();
            transform.rotation = Quaternion.LookRotation(Direction, Vector3.up);
        }
        else
        {

        }
        Spread *= Mathf.Deg2Rad;
        float cA = -Spread / 2; //currentAngle
        for (int i = 0; i < BulletCount; i++)
        {
            GameObject newbullet = Instantiate(BulletType, transform.position, Quaternion.identity, transform);
            Vector3 tempDir = new Vector3(Mathf.Sin(Mathf.Asin(0) + cA), 0, Mathf.Cos(Mathf.Asin(0) + cA));
            newbullet.transform.rotation = Quaternion.LookRotation(tempDir, Vector3.up);
            cA += Spread / (BulletCount - 1);
            newbullet.GetComponent<DefaultBullet>().Speed = BulletSpeeds;
            newbullet.GetComponent<DefaultBullet>().Direction = tempDir;
        }


        if (Spawner != null)
            Spawner.CompleteFire();//this bullet pattern is so simple that it's finished firing immediately, there's no circular pattern spawned one at a time or something
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.childCount == 0)
        {
            Destroy(gameObject);
        }
    }
}
