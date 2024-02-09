using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Info")]
    public float speed;
    public float lasTurnSpeed = 2;
    private Vector3 motion;
    [Header("")]
    [Header("Dash")]
    public float distance = 2;
    public bool dodging = false; [Tooltip("Don't touch")]
    public float dashTime;
    private Vector3 dashLocation;
    [Header("")]
    [Header("Health")]
    public int healthMax = 8;
    public int healthCurrent;
    private bool invincible = false;
    public float iFrames = 0.7f; [Tooltip("Invincibility over time")]
    [Header("")]
    [Header("Item Pick Ups")]
    public int heal = 2;
    public int smgAmmo = 15;
    public int laserAmmo = 2;

    [Header ("")]
    [Header("References")]
    private CharacterController playerControl;
    public BulletJam inputSystem; [Tooltip("Don't touch")]
    public Camera playerCam;
    public Animator playerAnim;

    bool mouseActive;

    // Start is called before the first frame update
    void Awake()
    {
        playerControl = GetComponent<CharacterController>();
        inputSystem = new BulletJam();

        healthCurrent = healthMax;
    }

    private void OnEnable()
    {
        inputSystem.Player.Enable();
    }
    private void OnDisable()
    {
        inputSystem.Player.Disable();
    }

    private void Update()
    {

        mouseActive = GetComponent<PlayerInput>().currentControlScheme.ToString() == "Keyboard&Mouse";

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //reads the axises on the left joystick
        Vector2 move = inputSystem.Player.Move.ReadValue<Vector2>();

        playerAnim.SetBool("Moving", move != new Vector2());
        playerAnim.SetBool("Strafe", Vector3.Angle(transform.forward, new Vector3(move.x, 0, move.y)) > 45 && Vector3.Angle(transform.forward, new Vector3(move.x, 0, move.y)) < 135 ||
            Vector3.Angle(transform.forward, new Vector3(move.x, 0, move.y)) < -45 && Vector3.Angle(transform.forward, new Vector3(move.x, 0, move.y)) > -135);
        playerAnim.SetBool("Negative", Vector3.Angle(transform.forward, new Vector3(move.x, 0, move.y)) < -45 || Vector3.Angle(transform.forward, new Vector3(move.x, 0, move.y)) > 135);
        OnMove(move);

        

        Vector2 look = inputSystem.Player.Look.ReadValue<Vector2>();

        if (mouseActive)
        {
            look = Input.mousePosition - playerCam.WorldToScreenPoint(transform.position);
        }

        if(look.x > 0.05f || look.y > 0.05f || look.x < -0.05f || look.y < -0.05f)
        {
             look = Vector2.ClampMagnitude(look, 1);

            OnLook(look);
        }

        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    public void OnMove(Vector2 moveInput)
    {


        //stops normal movement to allow for dodging
        if (dodging)
        {
            //moves the player to the location gained from the dodge input
            if (Vector3.Distance(transform.position, dashLocation) > 0.5f)
                transform.position = Vector3.Lerp(transform.position, dashLocation, dashTime * Time.deltaTime);
            else
                dodging = false;

        }
        else
        {
            motion = new Vector3(moveInput.x * speed * Time.deltaTime, 0, moveInput.y * speed * Time.deltaTime);

            playerControl.Move(motion);
        }

    }

    public void OnLook(Vector2 lookInput)
    {

        Quaternion lookRoatation = Quaternion.LookRotation(new Vector3(lookInput.x, 0, lookInput.y), Vector3.up);

        if(GetComponent<PlayerGun>().laserActive)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRoatation, lasTurnSpeed * Time.deltaTime);
            return;
        }
        if(!dodging)
            transform.rotation = lookRoatation;

    }

    public void Dodge(InputAction.CallbackContext context)
    {

        RaycastHit hit;
        if (context.phase.ToString() == "Started" && !dodging)
        {
            float dashDistance = distance;
            playerControl.Move(new Vector3());

            //checks for player input on left stick
            if (motion != new Vector3(0, 0, 0))
            {
                //faces player towards dash direction
                Vector3 directionFinder = Vector3.ClampMagnitude(motion, 1);
                transform.rotation = Quaternion.LookRotation(directionFinder);
            }

            //checks that the player will not go out of bounds
            if (Physics.Raycast(transform.position + new Vector3(0, 1, 0), transform.forward, out hit, distance, 0, QueryTriggerInteraction.Ignore))
            {
                Debug.Log(hit.transform.name);
                dashDistance = hit.distance - 0.7f; //lowers dash to just before the wall keeping in mind the player's radius

                if (dashDistance < 0.5f)
                    return;
            }
            //finds where the dash will finish
            dashLocation = transform.position + transform.forward * dashDistance;
            dodging = true;
            GetComponent<PlayerGun>().LaserEnd();
        }

    }
    //secondary method to stop dash
    private void OnCollisionEnter(Collision collision)
    {

        dodging = false;

    }

    //take damage
    private void OnTriggerEnter(Collider other)
    {

        if(other.gameObject.layer == LayerMask.NameToLayer("Enemy Bullets") && !dodging && !invincible && !GetComponent<PlayerGun>().chainsawing) //placeholder bullet name
        {
            healthCurrent -= other.GetComponent<DefaultBullet>().damage;
            Destroy(other.gameObject);
            //send health to UI
            invincible = true;
            Invoke(nameof(InvincibilityEnd), iFrames);
            GetComponent<MeshRenderer>().material.color = Color.red;//marking player as hit change this to model mesh later 
        }

        if(other.gameObject.tag == "Object Pickup")
        {
            string firstLetters = other.gameObject.name[0].ToString() + other.gameObject.name[1].ToString();
            switch (firstLetters)
            {

                default:
                    Debug.Log("Failed Object Check");
                    break;
                case ("HP"):
                    HealthChange(heal);
                    Destroy(other.gameObject);
                    break;
                case ("SM"):
                    GetComponent<PlayerGun>().smgInventoryAmmo += smgAmmo;
                    Destroy(other.gameObject);
                    break;
                case ("LA"):
                    GetComponent<PlayerGun>().laserInventoryAmmo += laserAmmo;
                    Destroy(other.gameObject);
                    break;
                
            }

        }
    }

    private void HealthChange(int healthChange)
    {

        if(healthChange > 0 && healthCurrent != healthMax)
        {
            healthCurrent += healthChange;
            
        }
        else if (healthChange < 0)
        {
            healthCurrent += healthChange;
        }

        if(healthCurrent <= 0)
        {
            Die();
        }

    }

    private void InvincibilityEnd()
    {
        invincible = false;
        GetComponent<MeshRenderer>().material.color = Color.white; //marking end of iFrames change this to model mesh later 
    }

    private void Die()
    {



    }

    public void PauseGame(InputAction.CallbackContext action)
    {
        if(action.phase.ToString() == "Started")
            UIScript.Instance.PauseGame();
    }


}
