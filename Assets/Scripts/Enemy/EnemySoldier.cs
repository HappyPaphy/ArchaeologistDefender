using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySoldier : EnemyEntity
{
    public enum EnemyState { Idle, Run, Attack, Died }
    public EnemyState currentState;

    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRndr;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;
    [SerializeField] private NavMeshAgent agent;

    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask towerLayer;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("GameObjects")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject artifact;
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject bulletPrefab;

    [Header("Variables")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float detectRange;
    [SerializeField] private float attackRange;
    [SerializeField] private float nextRangeAttack;
    [SerializeField] private float attackCooldown;

    [SerializeField] private float nextPlayerChaseCancel;
    [SerializeField] private float nextplayerChaseCooldown;
    [SerializeField] private float chasePlayerDuration;

    [SerializeField] private float avoidDistance = 0.5f;

    [SerializeField] private bool isDetectedTarget;
    [SerializeField] private bool isFollowingTarget;

    protected override void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody2D>();

        base.Awake();
    }

    protected override void Start()
    {
        agent.updateUpAxis = false; // For 2D pathfinding
        agent.updateRotation = false; // For 2D rotation handling
        base.Start();
    }

    protected override void Update()
    {
        player = PlayerController.instance.gameObject;
        artifact = Artifact.instance.gameObject;

        Facing();
        HandleState();
        ChooseAnimation();

        base.Update();
    }

    private void HandleState()
    {
        switch(currentState)
        {
            case EnemyState.Idle:
                DetectTower();

                if(target == null)
                {
                    currentState = EnemyState.Run;
                }
                break;

            case EnemyState.Run:

                if(target != null)
                {
                    float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
                    if (distanceToTarget > detectRange)
                    {
                        target = null;
                        currentState = EnemyState.Idle;
                        return;
                    }
                    else
                    {
                        //transform.position = Vector2.MoveTowards(transform.position, target.transform.position, moveSpeed);
                        agent.SetDestination(target.transform.position);

                        if (distanceToTarget <= attackRange)
                        {
                           currentState = EnemyState.Attack;
                        }
                    }
                }
                else
                {
                    //transform.position = Vector2.MoveTowards(transform.position, artifact.transform.position, moveSpeed);
                    //agent.isStopped = false;
                    DetectTower();
                    agent.SetDestination(artifact.transform.position);
                }

                break;

            case EnemyState.Attack:

                if (target != null)
                {
                    float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
                    if (distanceToTarget > attackRange)
                    {
                        agent.isStopped = false;
                        currentState = EnemyState.Run;
                        return;
                    }
                    else
                    {
                        agent.isStopped = true;
                        RangeAttack();
                    }
                }
                else
                {
                    agent.isStopped = false;
                    currentState = EnemyState.Run;
                }

                break;

            case EnemyState.Died:
                break;
        }
    }

    private void Facing()
    {

    }

    private void RangeAttack()
    {
        if (Time.time > nextRangeAttack && target != null)
        {
            nextRangeAttack = Time.time + attackCooldown;
            SoundManager.instance.PlayEnemySoldierSounds(0, transform.position);
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.transform.position = gameObject.transform.position;
            bullet.GetComponent<Bullet_Soldier>().target = target;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }


    private void DetectTower()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, detectRange, towerLayer);

        float closestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        foreach (Collider2D targetCol in hitEnemies)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, targetCol.transform.position);
            if (distanceToEnemy < detectRange && targetCol.GetComponent<Tower>().currentState != Tower.TowerState.Creating)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = targetCol.gameObject;
                currentState = EnemyState.Run;
                target = closestEnemy;
            }
        }

    }

    private void DetectPlayer()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, detectRange, towerLayer);

        float closestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        foreach (Collider2D targetCol in hitEnemies)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, targetCol.transform.position);
            if (distanceToEnemy < detectRange)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = targetCol.gameObject;
                currentState = EnemyState.Run;
                target = closestEnemy;
            }
        }

    }

    private void ChooseAnimation()
    {
        anim.SetBool("IsIdle", false);
        anim.SetBool("IsRun", false);
        anim.SetBool("IsAttack", false);
        anim.SetBool("IsDied", false);

        switch (currentState)
        {
            case EnemyState.Idle:
                anim.SetBool("IsIdle", true);
                break;
            case EnemyState.Run:
                anim.SetBool("IsRun", true);
                break;
            case EnemyState.Attack:
                anim.SetBool("IsAttack", true);
                break;
            case EnemyState.Died:
                anim.SetBool("IsDied", true);
                break;
        }
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

    public override void TakeDamage(float damageValue)
    {
        StartCoroutine(Tremble());
        SoundManager.instance.PlayEnemySoldierSounds(1, transform.position);
        CharacterHealthComponent.TakeDamage(damageValue);
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
