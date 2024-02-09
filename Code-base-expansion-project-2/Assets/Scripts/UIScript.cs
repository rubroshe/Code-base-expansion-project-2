using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIScript : MonoBehaviour
{
    [Header("Needed stuff")] 
    public GameObject menu;
    public Text ammoText;
    public PlayerMovement player;
    public PlayerGun gun;
    public Text endScreenText;
    public GameObject nextLevelButton;
    
    [Header("Images")]
    public Image hearts;
    public Image[] heartGlows;
    public Image chainsawChargeImage;
    public Image chainsawGlow;
    public Image[] weaponLights;
    public Image endScreen;
    public GameObject heathbarHolder;
    public Image heathbar;
    
    
    [Header("Numbers")] 
    public int weaponCount = 1;
    public int weaponIndex = 0;
    public int playerHealth = 8;
    public float chainsawCharge = 0;
    public float chainsawChargeTime = 12.5f;
    public int smgClip = 0;
    public int smgAmmo = 0;
    public int laserAmmo = 0;
    public float bossHealth = 100;
    public bool bossFight = false;

    [Header("Info")]
    public bool paused = false;

    public bool updateHP = true;
    public static UIScript Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
       
    }
    
    // Start is called before the first frame update
    void Start()
    {
       Cursor.visible = false;
       Cursor.lockState = CursorLockMode.Confined;
       Time.timeScale = 1;
    }

    private void FixedUpdate()
    {
        chainsawCharge += 1 / (100 * chainsawChargeTime);
        if (playerHealth <= 0)
        {
            gameOver(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        THENUMBERSMASON();
        if(Input.GetKeyDown(KeyCode.Tab)){PauseGame();} // the button above tab
        UiUpdate();
        
    }

    public void PauseGame()
    {
        paused = !paused; // switches the pause state
        menu.SetActive(paused);
        if (paused) // pauses the game
        {
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else // unpauses the game
        {
            Time.timeScale = 1;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    void UiUpdate()
    {
        //boss
        if (bossFight)
        {
            heathbarHolder.gameObject.SetActive(true);
            heathbar.fillAmount = bossHealth / 100f;
        }
        else
        {
            heathbarHolder.gameObject.SetActive(false);
        }


        //health
        hearts.fillAmount = playerHealth / 8f;
        
        for (int i = 0; i < 4; i++)
        {
            heartGlows[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < playerHealth/2; i++)
        {
            heartGlows[i].gameObject.SetActive(true);
        }
        
        //weapon
        for (int i = 0; i < 3; i++)
        {
            if (weaponIndex == i)
            {
                weaponLights[i].gameObject.SetActive(true);
            }
            else
            {
                weaponLights[i].gameObject.SetActive(false);
            }
        }
        
        //ammo ∞
        chainsawChargeImage.fillAmount = chainsawCharge;
        if(chainsawCharge >= 1f)
            chainsawGlow.gameObject.SetActive(true);
        else
            chainsawGlow.gameObject.SetActive(false);
        
        switch (weaponIndex)
        {
            case 0:

                ammoText.text = "∞";
                ammoText.gameObject.transform.localScale = new Vector3(1.5f, 1.5f,1);
                break;
            case 1:

                ammoText.text = smgClip + "/" + smgAmmo;
                ammoText.gameObject.transform.localScale = new Vector3(0.5f, 0.5f,1);
                break;
            case 2:

                ammoText.text = laserAmmo.ToString();
                ammoText.gameObject.transform.localScale = new Vector3(0.5f, 0.5f,1);
                break;
        }

    }

    void THENUMBERSMASON()
    {
        weaponIndex = gun.gunIndex;
        weaponCount = gun.gunCount;
        if(updateHP)
        playerHealth = player.healthCurrent;
        smgClip = gun.smgAmmoCurrent;
        smgAmmo = gun.smgInventoryAmmo;
        laserAmmo = gun.laserInventoryAmmo;
    }

    public void gameOver(bool win)
    {
        Cursor.visible = true;
        Time.timeScale = 0;
        endScreen.gameObject.SetActive(true);
        if (win)
        {
            nextLevelButton.gameObject.SetActive(true);
            endScreenText.text =
                "Good Job engineer, you have cleaned up a little more of this mess. report to HR for payment";
        }
        else
        {
            endScreenText.text =
                "Disappointing, corporate expects much more from all of it's employees. Report to HR for immediate contract termination and payment of "
                + Random.Range(100, 1200)
            +" in company damages in addition to the operation invoice.";
        }
    }
}
