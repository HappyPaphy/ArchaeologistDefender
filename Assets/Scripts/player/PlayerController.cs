using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : PlayerEntity
{
    public static PlayerController instance;

    public enum PlayerState { Idle, Run, Crafting, Hurt, Died };
    public PlayerState currentState;

    [Header("Components")]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private CapsuleCollider2D col;
    [SerializeField] private Rigidbody2D rb;

    [Header("Variable")]
    [SerializeField] private float currentMoveSpeed;
    [SerializeField] private float normalMoveSpeed;
    [SerializeField] private float runMoveSpeed;
    [SerializeField] private float dashSpeed;

    public int coinCount = 0;
    public int spiritOrbCount = 0;

    private Vector2 movement;
    private bool isSprintToggleOn = false;
    private bool isCrafting = false;
    private Vector2 mousePos;

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

        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        CreateTower();
        Movement();
        Running();
        base.Update();
    }

    private void Movement()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        rb.MovePosition(rb.position + movement * currentMoveSpeed * Time.deltaTime);
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

        if (isSprintToggleOn && (movement != Vector2.zero))
        {
            currentState = PlayerState.Run;
            currentMoveSpeed = runMoveSpeed;
        }
        else if (!isSprintToggleOn && (movement != Vector2.zero) && !isCrafting)
        {
            currentState = PlayerState.Idle;
            currentMoveSpeed = normalMoveSpeed;
        }
    }

    private void CreateTower()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TowerPlacer.instance.CreateTower(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TowerPlacer.instance.CreateTower(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TowerPlacer.instance.CreateTower(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            TowerPlacer.instance.CreateTower(3);
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

    public virtual void TakeDamage(float damageValue)
    {
        StartCoroutine(Tremble());
        CharacterHealthComponent.TakeDamage(damageValue);
    }

    protected override void Die()
    {

        /*if (GameData.Instance != null)
        {
            for (int i = 0; i < orbsToSpawn; i++)
            {
                GameData.Instance.CreateOrbAtTransform(transform);
            }
        }
        QuestManager.Instance.TrackKill(enemyType);*/

        base.Die(); // Call the base Die method to handle other death-related behaviors
    }

    private IEnumerator Tremble()
    {
        for (int i = 0; i < 10; i++)
        {
            sprite.transform.localPosition += new Vector3(0.08f, 0, 0);
            yield return new WaitForSeconds(0.01f);
            sprite.transform.localPosition -= new Vector3(0.08f, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
