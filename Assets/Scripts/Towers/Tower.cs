using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : TowerEntity
{
    public enum TowerState { Idle, Attack ,Destroyed };
    [SerializeField] private TowerState currentState;

    [Header("Components")]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody2D rb;

    [Header("Varaibles")]
    [SerializeField] private float enemyDetectRange;
    [SerializeField] private float playerDetectRange;
    [SerializeField] private float nextRangeAttack = 0.0f;
    [SerializeField] private float attackCooldownDuration;
    [SerializeField] private bool isDetectTarget = false;

    protected override void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        base.Awake();
    }

    protected override void Start()
    {
        
    }

    protected override void Update()
    {
        ChooseAnimation();
        base.Update();
    }

    private void RangeAttack()
    {

    }

    private void TowerUpgrade()
    {

    }

    private void Facing()
    {

    }

    public override void TakeDamage(float damageValue)
    {
        StartCoroutine(Tremble());
        base.TakeDamage(damageValue);
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

    private void ChooseAnimation()
    {
        anim.SetBool("IsIdle", false);
        anim.SetBool("IsAttack", false);
        anim.SetBool("IsDestroy", false);

        switch (currentState)
        {
            case TowerState.Idle:
                anim.SetBool("IsIdle", true);
                break;
            case TowerState.Attack:
                anim.SetBool("IsAttack", true);
                break;
            case TowerState.Destroyed:
                anim.SetBool("IsDestroy", true);
                break;
        }
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
