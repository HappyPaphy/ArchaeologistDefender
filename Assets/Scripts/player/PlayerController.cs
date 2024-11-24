using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : PlayerEntity
{
    public static PlayerController instance;

    public enum PlayerState { Idle, Run, Crafting, Hurt, Died };
    public PlayerState currentState;


    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRndr;
    [SerializeField] private CapsuleCollider2D col;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Slider sliderCoin;
    [SerializeField] private Slider sliderSpiritOrb;

    [Header("UI")]
    [SerializeField] private Text coinText;
    [SerializeField] private Text spiritOrbText;
    [SerializeField] private Text[] towerBuildCostsText;

    [Header("GameObject")]
    [SerializeField] private GameObject bloodSplashEffect;

    [Header("Variable")]
    [SerializeField] private float currentMoveSpeed;
    [SerializeField] private float normalMoveSpeed;
    [SerializeField] private float runMoveSpeed;
    [SerializeField] private float dashSpeed;

    [SerializeField] private float nextSpiritOrbIncome;
    [SerializeField] private float spiritIncomeCooldownDuration;
    [SerializeField] private int coinIncomeAmount;
    [SerializeField] private float nextCoinIncome;
    [SerializeField] private float coinIncomeCooldownDuration;
    [SerializeField] private int spiritOrbIncomeAmount;

    public int coinCount = 100;
    public int spiritOrbCount = 10;
    public int spiritOrbCountCancel = 0;
    private int currentCraftingTowerIndex = 0;
    public bool isCrafting = false;

    [SerializeField] private int[] towerBuildCosts;

    private Vector2 movement;
    private Vector2 mousePos;
    private bool isSprintToggleOn = false;

    protected override void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        rb = GetComponent<Rigidbody2D>();

        base.Awake();
    }

    protected override void Start()
    {
        if(towerBuildCostsText != null && towerBuildCosts != null)
        {
            for (int i = 0; i < towerBuildCostsText.Length; i++)
            {
                towerBuildCostsText[i].text = $"{towerBuildCosts[i]}";
            }
        }
        
        if(sliderCoin != null)
        {
            sliderCoin.maxValue = coinIncomeCooldownDuration;
        }
        
        if(sliderSpiritOrb != null)
        {
            sliderSpiritOrb.maxValue = spiritIncomeCooldownDuration;
        }
        
        base.Start();
    }

    protected override void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if(coinText != null)
            coinText.text = $"{coinCount}";

        if (spiritOrbText != null)
            spiritOrbText.text = $"{spiritOrbCount}";


        if(isCrafting)
        {
            if(Input.GetKeyDown(KeyCode.F))
            {
                isCrafting = false;
                currentState = PlayerState.Idle;
            }
            else if(Input.GetKeyDown(KeyCode.Escape))
            {
                isCrafting = false;
                spiritOrbCount += spiritOrbCountCancel;
                towerBuildCosts[currentCraftingTowerIndex] -= 2;
                towerBuildCostsText[currentCraftingTowerIndex].text = $"{towerBuildCosts[currentCraftingTowerIndex]}";
                currentState = PlayerState.Idle;
            }
        }

        if (!isCrafting)
        {
            CreateTower();
            Movement();
            Running();
        }

        if (sliderCoin != null)
            sliderCoin.value = nextCoinIncome - Time.time;

        if (sliderSpiritOrb != null)
            sliderSpiritOrb.value = nextSpiritOrbIncome - Time.time;

        if (Time.time > nextCoinIncome)
        {
            nextCoinIncome = Time.time + coinIncomeCooldownDuration;
            coinCount += coinIncomeAmount;
        }

        if (Time.time > nextSpiritOrbIncome)
        {
            nextSpiritOrbIncome = Time.time + spiritIncomeCooldownDuration;
            spiritOrbCount += spiritOrbIncomeAmount;
        }

        base.Update();
    }

    private void Movement()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        rb.MovePosition(rb.position + movement * currentMoveSpeed * Time.deltaTime);

        if(movement.x < 0)
        {
            spriteRndr.flipX = true;
        }
        else if (movement.x > 0)
        {
            spriteRndr.flipX = false;
        }
    }

    private void Running()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isSprintToggleOn = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isSprintToggleOn = false;
        }

        if (/*isSprintToggleOn &&*/ (movement != Vector2.zero))
        {
            currentState = PlayerState.Run;
            currentMoveSpeed = runMoveSpeed;
        }
        else if (/*!isSprintToggleOn &&*/ (movement == Vector2.zero) && !isCrafting)
        {
            currentState = PlayerState.Idle;
            currentMoveSpeed = normalMoveSpeed;
        }
    }

    private void CreateTower()
    {
        if (!isCrafting)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && spiritOrbCount >= towerBuildCosts[0])
            {
                currentCraftingTowerIndex = 0;
                spiritOrbCountCancel = towerBuildCosts[0];
                towerBuildCosts[0] += 2;
                towerBuildCostsText[0].text = $"{towerBuildCosts[0]}";
                spiritOrbCount -= spiritOrbCountCancel;

                currentState = PlayerState.Crafting;
                isCrafting = true;
                TowerPlacer.instance.CreateTower(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && spiritOrbCount >= towerBuildCosts[1])
            {
                currentCraftingTowerIndex = 1;
                spiritOrbCountCancel = towerBuildCosts[1];
                towerBuildCosts[1] += 5;
                towerBuildCostsText[1].text = $"{towerBuildCosts[1]}";
                spiritOrbCount -= spiritOrbCountCancel;

                currentState = PlayerState.Crafting;
                isCrafting = true;
                TowerPlacer.instance.CreateTower(1);
            }
            /*else if (Input.GetKeyDown(KeyCode.Alpha3) && spiritOrbCount >= towerBuildCosts[2])
            {
                spiritOrbCountCancel = towerBuildCosts[2];
                towerBuildCosts[2] += 2;
                towerBuildCostsText[2].text = $"{towerBuildCosts[2]}";
                spiritOrbCount -= spiritOrbCountCancel;

                currentState = PlayerState.Crafting;
                isCrafting = true;
                TowerPlacer.instance.CreateTower(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4) && spiritOrbCount >= towerBuildCosts[3])
            {
                spiritOrbCountCancel = towerBuildCosts[3];
                towerBuildCosts[3] += 2;
                towerBuildCostsText[3].text = $"{towerBuildCosts[3]}";
                spiritOrbCount -= spiritOrbCountCancel;

                currentState = PlayerState.Crafting;
                isCrafting = true;
                TowerPlacer.instance.CreateTower(3);
            }*/
        }
    }

    private void CheckHealth()
    {
        if (CharacterHealthComponent.CurrentHP <= 0)
        {
            currentState = PlayerState.Died;
            // Game Over;
        }

        if (CharacterHealthComponent.CurrentHP > CharacterHealthComponent.MaxHP)
        {
            CharacterHealthComponent.SetHP(CharacterHealthComponent.MaxHP);
        }
    }

    public override void TakeDamage(float damageValue)
    {
        StartCoroutine(Tremble());
        Instantiate(bloodSplashEffect, transform.position, Quaternion.Euler(0f, 0f, 0f));
        //SoundManager.instance.
        base.TakeDamage(damageValue);
    }

    protected override void Die()
    {
        currentState = PlayerState.Died;

        //base.Die();
    }


    private IEnumerator Tremble()
    {
        for (int i = 0; i < 10; i++)
        {
            spriteRndr.transform.localPosition += new Vector3(0.08f, 0, 0);
            yield return new WaitForSeconds(0.01f);
            spriteRndr.transform.localPosition -= new Vector3(0.08f, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
