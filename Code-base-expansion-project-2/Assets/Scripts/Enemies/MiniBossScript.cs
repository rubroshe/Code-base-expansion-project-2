using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBossScript : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private UIScript yooie;
    [SerializeField] private Vulnerability V;
    [SerializeField] private LaserMail Mail;
    [SerializeField] private float hp;
    [SerializeField] private float desync;
    [SerializeField] private float openTime;
    [SerializeField] private float closeTime;
    [SerializeField] private float pastTime;
    [SerializeField] private bool state = true; //open
    // Start is called before the first frame update
    void Start()
    {
        yooie = FindObjectOfType<UIScript>();
        pastTime = desync;
        if (state)
            desync = UnwindOpen(desync, out state);
        else
            desync = UnwindClosed(desync, out state);
        if (!state)
        {
            anim.SetTrigger("Change");
        }
        anim = GetComponentInChildren<Animator>();
        V = GetComponentInChildren<Vulnerability>();

    }

    private float UnwindOpen(float desync, out bool cond)
    {
        if (desync < openTime)
        {
            cond = true;
            return desync;
        }
        return UnwindClosed(desync - openTime, out cond);
    }

    private float UnwindClosed(float desync, out bool cond)
    {
        if (desync < closeTime)
        {
            cond = false;
            return desync;
        }
        return UnwindClosed(desync - openTime, out cond);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (V.vuln)
        {
            hp -= Mail.damageSince;
            Mail.damageSince = 0;
        }
        else
        {
            Mail.damageSince = 0;
        }
        yooie.bossHealth = hp;
        if(hp<=0)
        {
            //put what to do on death here
            yooie.updateHP = false;
            FindObjectOfType<PlayerMovement>().healthCurrent = 9000;
            Invoke("endGame", 0.45f);
            Destroy(gameObject, 0.5f);

            /*from the sounds of it, disable the models, enable the explosion particles, and then Destroy(gameObject, explosionDelay);
             * 
             * 
             * 
             * */
        }


        pastTime += Time.deltaTime;
        if (state && pastTime > openTime)
        {
            state = !state;
            anim.SetTrigger("Change");
            pastTime -= openTime;
        }
        else if (!state && pastTime > closeTime)
        {
            state = !state;
            anim.SetTrigger("Change");
            pastTime -= closeTime;
        }
    }

    private void endGame()
    {
        yooie.gameOver(true);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (V.vuln && (LayerMask.NameToLayer("BulletKiller") == other.gameObject.layer || LayerMask.NameToLayer("Player Bullets") == other.gameObject.layer))
        {
            hp--;
            if (LayerMask.NameToLayer("Player Bullets") == other.gameObject.layer)
                Destroy(other.gameObject);
        }
    }
}
