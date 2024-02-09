using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserMail : MonoBehaviour
{
    public float damageSince = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    public bool laserTicked = false;

    public void LaserDamage(float nextTick, float amount)
    {
        if (!laserTicked)
        {
            damageSince += amount;
            laserTicked = true;
            Invoke(nameof(UntickedLaserFunction), nextTick);
        }
    }

    public void LaserDamage(float nextTick)
    {
        if (!laserTicked)
        {
            damageSince += 1;
            laserTicked = true;
            Invoke(nameof(UntickedLaserFunction), nextTick);
        }
    }

    public void UntickedLaserFunction()
    {
        laserTicked = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.NameToLayer("BulletKiller") == other.gameObject.layer)
        {
            damageSince += 5;
        }
    }
}
