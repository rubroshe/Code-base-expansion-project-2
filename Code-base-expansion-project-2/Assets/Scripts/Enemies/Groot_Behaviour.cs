using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Groot_Behaviour : MonoBehaviour
{
    private enum States
    {
        Spawn,
        FindCover, //DONE (I think?) (gotta do the meaty get position for new cover code)
        Shoot, //DONE
        Firing, //DONE
        Strafe, //
        Die, //Initialise death subroutine and then enter Empty
        Empty //Do nothing as death resolves DONE
    }

    [SerializeField] private States state = States.Spawn;
    [SerializeField] private NavMeshAgent nav;
    [SerializeField] private GameObject player;
    [SerializeField] private Gun_Controller gun;
    [SerializeField] private float aggression = 0f; //%chance that groot tries to shoot you
    [SerializeField] private float aggroRate = 0.001f;
    [SerializeField] private float cowardMultiplier = 1f;
    [SerializeField] private float strafeChanceMaximum = 1 / 3;
    [SerializeField] private float minStrafeRange, maxStrafeRange;
    [SerializeField] private float hp = 7;
    [SerializeField] private Animator anim;
    [SerializeField] private LaserMail mail;
    private Quaternion playerLook;
    public float playerDist;
    private float initacc;
    // Start is called before the first frame update
    void Start()
    {
        mail = GetComponent<LaserMail>();
        nav = GetComponent<NavMeshAgent>();
        initacc = nav.speed;
        gun = GetComponentInChildren<Gun_Controller>();
        player = FindObjectOfType<PlayerMovement>().gameObject;
        if (player == null)
        {
            //Debug.Log("Enemy can't find player");
            Destroy(transform.parent.gameObject);
        }
        NavMeshHit navPlacer;
        if (NavMesh.SamplePosition(transform.position, out navPlacer, 500, 1))
        {
            gameObject.transform.position = navPlacer.position;
            nav.enabled = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        hp -= mail.damageSince;
        mail.damageSince = 0;
        if(hp<=0 && state != States.Empty)
        {
            state = States.Die;
        }
        playerLook = Quaternion.LookRotation(player.transform.position - transform.position, Vector3.up);
        playerDist = Vector3.Distance(player.transform.position, transform.position);
        switch (state)//ment
        {
            case States.Spawn:
                if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name != "GrootSpawn")
                    state = getNewState();

                break;
            case States.FindCover:
                if (Vector3.Distance(nav.destination, transform.position) < 0.03)
                {
                    //I am at the cover I intended to get to
                    aggression += aggroRate * cowardMultiplier;
                    RaycastHit coverChecker = new RaycastHit();
                    Physics.Raycast(transform.position, player.transform.position - transform.position, out coverChecker, (player.transform.position - transform.position).magnitude);
                    if (coverChecker.collider != null && coverChecker.collider.gameObject == player) //did we hit the player when targeted?
                    {
                        state = getNewState();
                    }
                    if (aggression > 0.5)
                    {
                        state = getNewState();
                    }
                }
                else
                {
                    aggression += aggroRate;
                    if (aggression > 0.8)
                    {
                        state = getNewState();
                    }
                }
                break;

            case States.Shoot:
                //check line of sight
                transform.rotation = Quaternion.Slerp(transform.rotation, playerLook, 0.3f);
                RaycastHit shootChecker = new RaycastHit();
                Physics.Raycast(transform.position, player.transform.position - transform.position, out shootChecker, (player.transform.position - transform.position).magnitude);
                if (shootChecker.collider != null && shootChecker.collider.gameObject == player) //did we hit the player when targeted?
                {
                    //fire
                    if (callOfDutyShootAMan())
                    {
                        nav.SetDestination(transform.position);
                        aggression -= 0.25f;
                        state = States.Firing;
                        //animation bool on goes here
                    }
                    else
                    {
                        nav.SetDestination(transform.position);
                    }
                }
                else
                {
                    //move towards player
                    nav.SetDestination(player.transform.position);
                }
                break;

            case States.Firing:
                anim.SetBool("Attack", true);
                transform.rotation = Quaternion.Slerp(transform.rotation, playerLook, 0.3f);
                nav.speed = 0;
                nav.SetDestination(player.transform.position);
                if (gun.State < 2)
                {
                    anim.SetBool("Attack", false);
                    nav.speed = initacc;
                    nav.SetDestination(transform.position);
                    state = getNewState();
                }
                break;

            case States.Strafe:
                transform.rotation = Quaternion.Slerp(transform.rotation, playerLook, 0.3f);
                //Debug.DrawLine(nav.destination, transform.position, Color.blue, 7f);

                aggression += aggroRate;//increase aggression
                
                if (Vector3.Distance(player.transform.position, nav.destination) > maxStrafeRange || Vector3.Distance(player.transform.position, nav.destination) < minStrafeRange)
                {
                    StrafeTarget(0);
                }

                Physics.Raycast(transform.position, player.transform.position - transform.position, out shootChecker, (player.transform.position - transform.position).magnitude);
                if (shootChecker.collider != null && shootChecker.collider.gameObject == player && aggression > 0.75) //Can we shoot the player?
                {
                    state = getNewState();
                }

                //Debug.Log(Vector3.Distance(nav.destination, transform.position));
                if (Vector3.Distance(nav.destination, transform.position) < 0.5)//if close enough to where was going
                {
                    getNewState();//reroll priorities
                }
                break;

            case States.Die:
                float randmoval = Random.value;
                if (randmoval <= 0.1)
                    gun.Fire(8, true);
                else if (randmoval <= 0.3)
                    gun.Fire(9, true);
                else if (randmoval <= 0.5)
                    gun.Fire(10, true);
                GetComponentInChildren<Animator>().enabled = false;//deactivate the animator
                foreach (Transform c in anim.gameObject.transform)
                    c.gameObject.SetActive(!c.gameObject.activeSelf); //find each collider in this things children, ie groot upp and groot lower, and set them active
                transform.parent.DetachChildren();
                nav.SetDestination(transform.position);
                state = States.Empty; //do no more after the next line
                Destroy(gameObject, Random.Range(3, 5)); //remove this, the prefab model, but leave the bullets
                break;

            default:
                break;
        }
    }

    private bool callOfDutyShootAMan()
    {
        if (gun.State == 0)
        {
            if (Random.value <= 0.75)
            {
                gun.Fire(1, true);
                return true;
            }
            else
            {
                gun.Fire(2, true);
                return true;
            }
        }
        return false;
    }

    private States getNewState()
    {
        float randres = Random.Range(0f, 1f);
        float gap = Mathf.Sin(aggression * Mathf.PI) * strafeChanceMaximum;
        float attack = aggression - gap / 2;
        if (randres <= attack)
        {
            return States.Shoot;
        }
        else if (randres <= attack + gap)
        {
            StrafeTarget(0);
            return States.Strafe;
        }
        else
        {
            CoverTarget();
            return States.FindCover;
        }
    }

    private void StrafeTarget(int attempt)
    {
        if (attempt > 10)
        {
            getNewState();
            return;
        }

        Vector3[] targets = new Vector3[4];
        float[] dists = new float[4];

        float offset = Random.Range(0, maxStrafeRange - minStrafeRange);
        offset += minStrafeRange;
        Vector3 randomPoint = Random.insideUnitCircle;
        randomPoint.z = randomPoint.y;
        randomPoint.y = 0;
        randomPoint.Normalize();
        randomPoint *= offset;

        targets[0] = randomPoint+player.transform.position;
        targets[1] = -randomPoint + player.transform.position;
        targets[2] = new Vector3(randomPoint.z, 0, -randomPoint.x) + player.transform.position;
        targets[3] = -targets[2] + player.transform.position;

        for (int i = 0; i < 4; i++)
        {
            dists[i] = Vector3.Distance(transform.position, targets[i]);
        }
        float mindist = Mathf.Min(dists);
        for (int i = 0; i < 4; i++)
        {
            if (dists[i] == mindist)
            {

                NavMeshPath path = new NavMeshPath();
                nav.SetDestination(targets[i]);
                nav.CalculatePath(targets[i], path);
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    return;
                }
            }
        }
        StrafeTarget(attempt + 1);
    }

    private void CoverTarget()
    {
        RaycastHit shootChecker = new RaycastHit();
        Physics.Raycast(transform.position, player.transform.position - transform.position, out shootChecker, (player.transform.position - transform.position).magnitude);
        if (shootChecker.collider != null && shootChecker.collider.gameObject == player) //did we hit the player when targeted?
        {
            //we are not currently in cover
            Vector3[] testpos = new Vector3[20];
            Vector3 playerToMe = transform.position - player.transform.position;
            playerToMe.y = 0f;
            playerToMe.Normalize();
            Vector3 testDir = playerToMe;
            testDir.x = -playerToMe.z;
            testDir.z = playerToMe.x;

            for (int i = 0; i < 10; i++)
            {
                testpos[2 * i] = transform.position + (i + 1) * testDir;
                testpos[2 * i + 1] = transform.position - (i + 1) * testDir;
            }

            //evens are one direction, odds are the other

            //for each testpos, check the raycast for cover
            for (int i = 0; i < 20; i++)
            {
                bool failure = false;
                RaycastHit wallCheck = new RaycastHit();
                Physics.Raycast(testpos[i], transform.position - testpos[i], out wallCheck);
                Physics.Raycast(testpos[i], player.transform.position - testpos[i], out shootChecker, (player.transform.position - testpos[i]).magnitude);
                if (shootChecker.collider.gameObject != player && wallCheck.collider != null && LayerMask.NameToLayer("Walls") != wallCheck.collider.gameObject.layer)
                {
                    //cover found
                    Vector3 coverTarg = shootChecker.point;
                    playerToMe = coverTarg - player.transform.position;
                    playerToMe.Normalize();
                    coverTarg += playerToMe;
                    Physics.Raycast(coverTarg, transform.position - coverTarg, out wallCheck);
                    if (wallCheck.collider != null && LayerMask.NameToLayer("Walls") != wallCheck.collider.gameObject.layer)
                    {
                        Collider[] FinalChecker = Physics.OverlapSphere(coverTarg, 0.1f);
                        foreach (Collider c in FinalChecker)
                        {
                            if (LayerMask.NameToLayer("Walls") == c.gameObject.layer)
                            {
                                failure = true;
                            }
                        }
                        if (!failure)
                        {
                            nav.SetDestination(coverTarg);
                            return;
                        }
                    }
                }
            }
            StrafeTarget(0);
            aggression += 0.1f;
            getNewState();
        }
    }

    private float Power(int e, int x)
    {
        if (x == 0)
            return 1;
        int power = Mathf.Abs(x);
        float result = 1;
        for (int i = 0; i < power; i++)
        {
            result *= e;
        }
        if (x < 0)
            result = 1 / result;
        return result;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (LayerMask.NameToLayer("Player Bullets") == other.gameObject.layer && state != States.Empty && state != States.Die)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player Bullets"))
            {
                hp -= other.gameObject.GetComponent<PlayerBullet>().damage;
            }
            if (hp <= 0)
            {
                state = States.Die;
            }
            Destroy(other.gameObject);
        }
    }
}
