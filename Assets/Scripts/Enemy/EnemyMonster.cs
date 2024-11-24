using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMonster : EnemyEntity
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

    [Header("GameObjects")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject artifact;
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject bloodSplashEffect;
    [SerializeField] public Transform spawnTransform;

    [Header("Variables")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float detectRange;
    [SerializeField] private float attackRange;
    [SerializeField] private float nextCloseAttack;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float attackValue;

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
        agent.speed = 3f;

        base.Start();
    }

    protected override void Update()
    {
        if(PlayerController.instance.gameObject != null)
        {
            player = PlayerController.instance.gameObject;
        }
        artifact = Artifact.instance.gameObject;

        Facing();
        HandleState();
        ChooseAnimation();

        base.Update();
    }

    private void HandleState()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                //DetectTower();
                DetectPlayer();

                if (target == null)
                {
                    currentState = EnemyState.Run;
                }
                break;

            case EnemyState.Run:

                //transform.position = Vector2.MoveTowards(transform.position, artifact.transform.position, moveSpeed);
                //agent.isStopped = false;
                //DetectTower();
                if(target == null)
                {
                    DetectPlayer();
                    agent.SetDestination(artifact.transform.position);

                    float distanceToArtifact = Vector2.Distance(transform.position, artifact.transform.position);
                    if (distanceToArtifact > attackRange)
                    {
                        agent.isStopped = false;
                        currentState = EnemyState.Run;
                        return;
                    }
                    else
                    {
                        currentState = EnemyState.Attack;
                        agent.isStopped = true;
                    }
                }
                else
                {
                    //agent.SetDestination(player.transform.position);
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
                        CloseAttack();
                    }
                }
                else
                {
                    float distanceToArtifact = Vector2.Distance(transform.position, artifact.transform.position);
                    if (distanceToArtifact > attackRange)
                    {
                        agent.isStopped = false;
                        currentState = EnemyState.Run;
                        return;
                    }
                    else
                    {
                        agent.isStopped = true;
                        CloseAttack();
                    }
                }

                break;

            case EnemyState.Died:
                break;
        }
    }

    private void CloseAttack()
    {
        if (Time.time > nextCloseAttack)
        {
            nextCloseAttack = Time.time + attackCooldown;

            if(target != null)
            {
                if (target.gameObject.CompareTag("Player"))
                {
                    target.GetComponent<PlayerController>().TakeDamage(attackValue);
                }
                else if (target.gameObject.CompareTag("Tower"))
                {
                    target.GetComponent<Tower>().TakeDamage(attackValue);
                }
            }
            else
            {
                artifact.GetComponent<Artifact>().TakeDamage(attackValue);

                /*if (artifact.gameObject.CompareTag("Artifact"))
                {
                    target.GetComponent<Artifact>().TakeDamage(attackValue);
                }*/
            }

            SoundManager.instance.PlayEnemyMonsterSounds(0, transform.position);
        }
    }

    private void Facing()
    {
        if (target != null)
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
        else
        {
            Vector2 lookDirection = (Vector2)artifact.transform.position - new Vector2(transform.position.x, transform.position.y);
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
        if (Time.time > nextPlayerChaseCancel)
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
        if (target == player)
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

        PlayerController.instance.spiritOrbCount += 1;
        base.Die(); // Call the base Die method to handle other death-related behaviors
    }

    public override void TakeDamage(float damageValue)
    {
        StartCoroutine(Tremble());
        Instantiate(bloodSplashEffect, transform.position, Quaternion.Euler(0f, 0f, 0f));
        SoundManager.instance.PlayEnemyMonsterSounds(1, transform.position);
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
