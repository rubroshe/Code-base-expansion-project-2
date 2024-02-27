using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    // Entire scipt added by RB 
    public float delay = 3f;
    public float blastRadius = 5f;
    public float explosionForce = 10f;
    public int explosionDamage = 50; // would rec changing damage value as lvl progresses
    public ParticleSystem explosionEffect; // Assign in inspector

    
    private bool hasExploded = false;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("Explode", delay);
    }

    // Update is called once per frame
    /*void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0f && !hasExploded)
        {
            Explode();
            hasExploded = true;
        }
    }*/

    void Explode()
    {
        // Show explosion effect
        // Instantiate(explosionEffect, transform.position, transform.rotation);
        if (hasExploded) return;

        // Show explosion effect
        Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, blastRadius);
            }

            // Directly apply damage to enemies
            BabuBehaviour enemy = hit.GetComponent<BabuBehaviour>(); // consolidate enemy scripts into script named Enemy and replace BabuBehavior with "Enemy"
            if (enemy != null)
            {
                enemy.TakeExplosionDamage(explosionDamage);
            }
        }

        hasExploded = true;
        // Destroy grenade object after explosion
        Destroy(gameObject);
    }
}
