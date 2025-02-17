using System;
using StarterAssets;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Rendering;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class Player : MonoBehaviour
{
    public GameObject currentWeapon;
    public int health = 100;

    public Vector3 initLerp;
    public Vector3 finalLerp;
    private Vector3 initPosition;

    public Text Message;
    public Text gameMessage;
    public Text healthStatus;
    public Text enemyHealth;
    public Text gameStats;

    private bool isLerping = false;
    private float timeForLerpA = 0.25f;
    private float elaspedTimeA = 0f;
    private float timeForLerpB = 0.25f;
    private float elaspedTimeB = 0f;
    private int goblinDeaths = 0;
    private int wave = 0;
    private int experience = 0;
    private int level = 0;
    public GameObject levelUpPanel;
    public Button upgradeAttackButton;
    public Button upgradeMovementButton; 
    public int attackDamage = 20; 
    public float movementSpeed = 5.0f; 
    private bool isLevelingUp = false; 

    void Start()
    {

        Message = GameObject.Find("Message").GetComponent<Text>();
        gameMessage = GameObject.Find("CombatMessage").GetComponent<Text>();
        healthStatus = GameObject.Find("Health").GetComponent<Text>();
        enemyHealth = GameObject.Find("EnemyHealth").GetComponent<Text>();
        gameStats = GameObject.Find("GameStats").GetComponent<Text>();
        levelUpPanel = GameObject.Find("LevelUpPanel");
        upgradeAttackButton = GameObject.Find("upgradeAttackButton").GetComponent<Button> ();
        upgradeMovementButton = GameObject.Find("upgradeMovementButton").GetComponent<Button>();
        attackDamage = 20;

        initPosition = currentWeapon.transform.position;
        initLerp = currentWeapon.transform.rotation.eulerAngles;
        finalLerp = currentWeapon.transform.rotation.eulerAngles + new Vector3(0f, 0f, 90f);
        Debug.Log(initLerp);
        Debug.Log(finalLerp);
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Level Up Panel is not assigned in the Inspector.");
        }
    }

    void Update()
    {
        gameStats.text = "exp: " + experience.ToString();
        if (isLevelingUp) return;
        switch (experience)
        {
            case 50: LevelUp(1); break;
            case 255: LevelUp(2); break;
            case 610: LevelUp(3); break;
            case 1015: LevelUp(4); break;
        }
        RaycastHit HitInfo;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out HitInfo, 100.0f))
        {
            Message.text = HitInfo.transform.name;
        }

        if (isLerping == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isLerping = true;
                elaspedTimeA = 0f;
                elaspedTimeB = 0f;
            }
        }
        else
        {
            if (elaspedTimeA < timeForLerpA)
            {
                currentWeapon.transform.localRotation = Quaternion.Euler(Vector3.Lerp(initLerp, finalLerp, elaspedTimeA / timeForLerpA));
                elaspedTimeA += Time.deltaTime;
            }
            else if (elaspedTimeB < timeForLerpB)
            {
                currentWeapon.transform.localRotation = Quaternion.Euler(Vector3.Lerp(finalLerp, initLerp, elaspedTimeB / timeForLerpB));
                elaspedTimeB += Time.deltaTime;
            }
            else
            {
                isLerping = false;
                Debug.Log(initLerp);
                Debug.Log(finalLerp);

                RaycastHit HitInfoAttack;
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out HitInfoAttack, 5.0f))
                {
                    if (HitInfoAttack.transform.gameObject.name.StartsWith("GOBLIN"))
                    {
                        GoblinAI goblin = HitInfoAttack.transform.GetComponent<GoblinAI>();
                        int atkRNG = UnityEngine.Random.Range(0, 10);
                        if (atkRNG > 5)
                        {
                            goblin.TakeDamage(attackDamage);
                            EnemyHealth(goblin.health);
                            GameLog("You hit Goblin for" + attackDamage +" hp");
                            GameLog(goblin.health.ToString());
   
                            if (goblin.health <= 0)
                            {
                                experience += 50;
                                GameLog("Goblin is dead");
                                goblinDeaths += 1;
                                GameLog(goblinDeaths.ToString() + " Goblin deaths");
                            }
                        }
                        else
                        {
                            GameLog("You failed to hit Goblin");
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.transform.name);

        if (other.transform.name == "CharacterDetect") {
            GoblinAI goblin = other.transform.parent.gameObject.GetComponent<GoblinAI>();
            goblin.Attack();
            goblin.steeringBasics.maxVelocity = 0.0f;
        }
        else if (other.transform.name == "PotentialPickup")
        {
            //maybe increase gold here?

        }

    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log(other.transform.name);

        if (other.transform.name == "CharacterDetect")
        {
            GoblinAI goblin = other.transform.parent.gameObject.GetComponent<GoblinAI>();
            if (goblin.anims.isPlaying == false)
            {
                int atkRNG = UnityEngine.Random.Range(0, 10);
                if (atkRNG > 5)
                {
                    this.health -= 10;
                    GameLog("Goblin hit you for 10 hp");
                    HealthLog((this.health).ToString());


                }
                else
                {
                    GameLog("Goblin failed to hit");
                }
            }
            goblin.Attack();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log(other.transform.name);

        if (other.transform.name == "CharacterDetect")
        {
            GoblinAI goblin = other.transform.parent.gameObject.GetComponent<GoblinAI>();
            goblin.Run();
            goblin.steeringBasics.maxVelocity = 3.5f;
        }

    }

    int lnTotal = 0;


    private void LevelUp(int val)
    {
        isLevelingUp = true;
        Time.timeScale = 0; 
        levelUpPanel.SetActive(true); 
        level = val;
        experience += 5;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
        // Add listeners to the buttons
        upgradeAttackButton.onClick.RemoveAllListeners();
        upgradeMovementButton.onClick.RemoveAllListeners();

        upgradeAttackButton.onClick.AddListener(() => UpgradeAttack(val));
        upgradeMovementButton.onClick.AddListener(() => UpgradeMovement(val));
    }

    private void UpgradeAttack(int val)
    {
        attackDamage += 5; 
        GameLog("Attack Increased" + attackDamage.ToString());

        ResumeGame(val);

    }

    private void UpgradeMovement(int val)
    {
        FirstPersonController player = GetComponent<FirstPersonController>();
        if (player != null)
        {
            player.IncreaseSpeed(); 
        }
        GameLog("Speed Increased" + player.MoveSpeed.ToString());
        ResumeGame(val);
    }

    private void ResumeGame(int val)
    {
        isLevelingUp = false;
        levelUpPanel.SetActive(false);
        Time.timeScale = 1; 
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        //Wave(val);
    }
    private void GameLog(string ln)
    {
        if (lnTotal > 10)
        {
            gameMessage.text = "";
            lnTotal = 0;
        }

        gameMessage.text += ln + Environment.NewLine;
        ++lnTotal;
    }

    private void HealthLog(string ln)
    {
        healthStatus.text = ln + "%";
    }

    private void EnemyHealth(int health)
    {
        enemyHealth.text = health + "%";
    }

    //private void Wave(int level)
    //{
    //    GoblinWave spawner = FindAnyObjectByType<GoblinWave>();
    //    if (spawner != null)
    //    {
    //        spawner.goblinsPerWave += 2; // Increase goblins per wave
    //    }
    //}
}