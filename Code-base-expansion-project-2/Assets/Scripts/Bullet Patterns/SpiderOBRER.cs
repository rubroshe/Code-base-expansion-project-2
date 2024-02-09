using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderOBRER : MonoBehaviour
{
    [Header("This Bullet Pattern Script Fires in a straight line")]

    public float BulletSpeeds;
    public float MaxTravel;
    public float sizeMarkipiler;
    public int damage;
    public GameObject boolet;
    [Header("This entity spawned these bullets (assigned by this script)")]
    public Gun_Controller Spawner;
    // Start is called before the first frame update
    void Start()
    {
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
            Debug.Log("Bullet can't find player");
            Destroy(this.gameObject);
        }
        Vector3 Direction = player.transform.position - transform.position;
        Direction.y = 0;
        Direction.Normalize();
        transform.rotation = Quaternion.LookRotation(Direction, Vector3.up);
        /*
        DefaultBullet[] childs = GetComponentsInChildren<DefaultBullet>();
        foreach (DefaultBullet boolet in childs)
        {
            boolet.Speed = BulletSpeeds;
            boolet.Direction = new Vector3(0, 0, 1);
            boolet.MaxTravel = MaxTravel;
            boolet.transform.localScale *= sizeMarkipiler;
        }*/
        GameObject newb = Instantiate(boolet, transform.position, Quaternion.identity, transform);
        newb.GetComponent<DefaultBullet>().Speed = BulletSpeeds;
        newb.GetComponent<DefaultBullet>().MaxTravel = MaxTravel;
        newb.GetComponent<DefaultBullet>().damage = damage;
        newb.GetComponent<DefaultBullet>().Direction = new Vector3(0,0,1);
        newb.transform.localScale *= sizeMarkipiler;
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
