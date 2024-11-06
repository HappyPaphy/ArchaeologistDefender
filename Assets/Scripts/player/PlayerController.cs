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
    [SerializeField] private Rigidbody2D rb;

    [Header("Variable")]
    [SerializeField] private float currentMoveSpeed;
    [SerializeField] private float normalMoveSpeed;
    [SerializeField] private float runMoveSpeed;
    [SerializeField] private float dashSpeed;

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
        base.Update();
    }

    private void Movement()
    {

    }

    private void CreateTower()
    {

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
