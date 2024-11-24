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
    [SerializeField] private CapsuleCollider2D col;

    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask towerLayer;
    [SerializeField] private LayerMask obstacleLayer;

    [SerializeField] private LayerMask ignoreLayer_NotSteal;
    [SerializeField] private LayerMask ignoreLayer_Stealing;

    [Header("GameObjects")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject artifact;
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject bloodSplashEffect;
    [SerializeField] public Transform spawnTransform;

    [Header("Variables")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float detectRange;
    [SerializeField] private float attackRange;
    [SerializeField] private float nextRangeAttack;
    [SerializeField] private float attackCooldown;

    [SerializeField] private float nextPlayerChaseCancel;
    [SerializeField] private float nextplayerChaseCooldown;
    [SerializeField] private float chasePlayerDuration;

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
        agent.speed = 2f;

        col.excludeLayers = ignoreLayer_NotSteal;
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
                DetectPlayer();

                if (target == null)
                {
                    currentState = EnemyState.Run;
                }
                break;

            case EnemyState.Run:

                float distanceToArtifact = Vector2.Distance(transform.position, artifact.transform.position);

                if (!GameManager.instance.IsArtifactBeingStole())
                {
                    if (distanceToArtifact < 0.07f)
                    {
                        GameManager.instance.SetArtifactStoleStatus(true);
                        artifact.transform.position = transform.position;
                        agent.SetDestination(spawnTransform.position);
                        agent.speed = 0.5f;
                    }

                    if (target != null)
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
                        DetectPlayer();
                        agent.SetDestination(artifact.transform.position);
                    }
                }
                else
                {
                    if (distanceToArtifact < 0.07f)
                    {
                        GameManager.instance.SetArtifactStoleStatus(true);
                        artifact.transform.position = transform.position;
                        agent.SetDestination(spawnTransform.position);
                        agent.speed = 0.5f;
                        
                        col.excludeLayers = ignoreLayer_Stealing;
                    }
                    else
                    {
                        DetectTower();
                        DetectPlayer();
                        agent.SetDestination(player.transform.position);
                    }
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
        if(target != null)
        {
            Vector2 lookDirection = (Vector2)target.transform.position - new Vector2(transform.position.x, transform.position.y);
            float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;

            if ((angle > -90) && (angle < 90))
            {
                gameObject.transform.localScale = new Vector3(1, 1, 1);
                slider.transform.localScale = new Vector3(1f, 1f, 1f);
            }
            else
            {
                gameObject.transform.localScale = new Vector3(-1, 1, 1);
                slider.transform.localScale = new Vector3(-1f, 1f, 1f);
            }
        }
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
        if(Time.time > nextPlayerChaseCancel)
        {
            nextPlayerChaseCancel = Time.time + nextplayerChaseCooldown;
            Invoke("ResetPlayerChase", chasePlayerDuration);

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, detectRange, playerLayer);

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
        

    }

    private void ResetPlayerChase()
    {
        if(target == player)
        target = null;
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
        float distanceToArtifact = Vector2.Distance(transform.position, artifact.transform.position);

        if (distanceToArtifact < 0.03f)
        {
            GameManager.instance.SetArtifactStoleStatus(false);
        }
        /*if (GameData.Instance != null)
        {
            for (int i = 0; i < orbsToSpawn; i++)
            {
                GameData.Instance.CreateOrbAtTransform(transform);
            }
        }
        QuestManager.Instance.TrackKill(enemyType);*/

        PlayerController.instance.coinCount += 10;
        base.Die(); // Call the base Die method to handle other death-related behaviors
    }

    public override void TakeDamage(float damageValue)
    {
        StartCoroutine(Tremble());
        Instantiate(bloodSplashEffect, transform.position, Quaternion.Euler(0f, 0f, 0f));
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
