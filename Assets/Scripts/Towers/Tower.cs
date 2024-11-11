using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tower : TowerEntity
{
    public enum TowerState { Creating, Idle, Attack ,Destroyed };
    [SerializeField] public TowerState currentState;

    public enum TowerBaseLevel { Level_1, Level_2, Level_3 }
    [SerializeField] public TowerBaseLevel towerBaseLevel;

    public enum TowerWeaponLevel { Level_1, Level_2, Level_3 }
    [SerializeField] public TowerWeaponLevel towerWeaponLevel;

    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRndr_TowerBase;
    [SerializeField] private SpriteRenderer spriteRndr_TowerWeapon;
    [SerializeField] private Animator anim_TowerWeapon;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask enemyLayer;

    [Header("GameObject")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject targetedEnemy;
    [SerializeField] private GameObject bulletPrefab;
    //[SerializeField] private List<GameObject> targets;
    [SerializeField] private GameObject upgradeTowerCanvas;

    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform[] towerWeaponTransforms;

    [SerializeField] private Text text_UpgradeTowerBaseCost;
    [SerializeField] private Text text_UpgradeTowerWeaponCost;

    [Header("Varaibles")]
    [SerializeField] private float enemyDetectRange;
    [SerializeField] private float playerDetectRange;
    [SerializeField] private float nextRangeAttack = 0.0f;
    [SerializeField] private float attackCooldownDuration;
    [SerializeField] private bool isDetectTarget = false;

    [SerializeField] private float mouseDistance = 0f;
    [SerializeField] private GameObject mouseTransform;

    [SerializeField] private int[] towerBaseUpgradeCosts;
    [SerializeField] private int[] towerWeaponUpgradeCosts;

    protected override void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        base.Awake();
    }

    protected override void Start()
    {
        currentState = TowerState.Creating;

        towerWeaponLevel = TowerWeaponLevel.Level_1;
        text_UpgradeTowerWeaponCost.text = $"{towerWeaponUpgradeCosts[0]}";
        TowerWeaponSprite();

        towerBaseLevel = TowerBaseLevel.Level_1;
        text_UpgradeTowerBaseCost.text = $"{towerBaseUpgradeCosts[0]}";
        TowerBaseSprite();
    }

    protected override void Update()
    {
        //UpdateMousePosition();
        player = PlayerController.instance.gameObject;
        DetectPlayer();
        HandleState();
        ChooseAnimation();
        base.Update();
    }

    private void DetectPlayer()
    {
        if(Vector2.Distance(transform.position, player.transform.position) < playerDetectRange && currentState != TowerState.Creating)
        {
            upgradeTowerCanvas.SetActive(true);
        }
        else
        {
            upgradeTowerCanvas.SetActive(false);
        }
    }

    private void HandleState()
    {
        if(currentState != TowerState.Creating)
        {
            spriteRndr_TowerBase.color = new Color32(255, 255, 255, 255);
            spriteRndr_TowerWeapon.color = new Color32(255, 255, 255, 255);
        }

        switch (currentState)
        {
            case TowerState.Creating:

                mouseTransform = GameObject.FindGameObjectWithTag("MouseTransform");
                mouseTransform.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position = mouseTransform.transform.position;
                mouseDistance = Vector3.Distance(player.transform.position, gameObject.transform.position);

                if (Vector3.Distance(player.transform.position, gameObject.transform.position) < playerDetectRange)
                {
                    spriteRndr_TowerBase.color = new Color32(255, 255, 255, 60);
                    spriteRndr_TowerWeapon.color = new Color32(255, 255, 255, 60);

                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        currentState = TowerState.Idle;
                    }
                }
                else
                {
                    spriteRndr_TowerBase.color = new Color32(255, 0, 0, 60);
                    spriteRndr_TowerWeapon.color = new Color32(255, 0, 0, 60);
                }
                

                
                break;

            case TowerState.Idle:

                DetectEnemy();

                /*if (targets.Count > 0)
                {
                    *//*GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                    foreach (var enemy in enemies)
                    {
                        if (Vector3.Distance(enemy.transform.position, gameObject.transform.position) <= enemyDetectRange)
                        {
                            targetedEnemy = enemy;
                            currentState = TowerState.Attack;
                        }
                    }*//*
                }*/
                break;

            case TowerState.Attack:

                if (targetedEnemy != null)
                {
                    float distanceToTarget = Vector2.Distance(transform.position, targetedEnemy.transform.position);
                    if (distanceToTarget > enemyDetectRange)
                    {
                        targetedEnemy = null;
                        currentState = TowerState.Idle;
                        return;
                    }
                    else
                    {
                        RangeAttack();
                    }
                }
                else
                {
                    currentState = TowerState.Idle;
                }
                break;

        }    
    }
    private void UpdateMousePosition()
    {
        mouseTransform = GameObject.FindGameObjectWithTag("MouseTransform");
        mouseTransform.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public void TowerWeaponUpgrade()
    {
        if(towerWeaponLevel != TowerWeaponLevel.Level_3)
        {
            switch(towerWeaponLevel)
            {
                case TowerWeaponLevel.Level_1:
                    if(PlayerController.instance.coinCount >= towerWeaponUpgradeCosts[0])
                    {
                        towerWeaponLevel = TowerWeaponLevel.Level_2;
                        text_UpgradeTowerWeaponCost.text = $"{towerWeaponUpgradeCosts[1]}";
                        TowerWeaponSprite();
                    }
                    break;

                case TowerWeaponLevel.Level_2:
                    if (PlayerController.instance.coinCount >= towerWeaponUpgradeCosts[1])
                    {
                        towerWeaponLevel = TowerWeaponLevel.Level_3;
                        text_UpgradeTowerWeaponCost.text = $"MAX";
                        TowerWeaponSprite();
                    }
                    break;
            }
        }
    }

    public void TowerBaseUpgrade()
    {
        if (towerBaseLevel != TowerBaseLevel.Level_3)
        {
            switch (towerBaseLevel)
            {
                case TowerBaseLevel.Level_1:
                    if(PlayerController.instance.coinCount >= towerBaseUpgradeCosts[0])
                    {
                        towerBaseLevel = TowerBaseLevel.Level_2;
                        text_UpgradeTowerBaseCost.text = $"{towerBaseUpgradeCosts[1]}";
                        TowerBaseSprite();
                    }
                    break;

                case TowerBaseLevel.Level_2:
                    if (PlayerController.instance.coinCount >= towerBaseUpgradeCosts[1])
                    {
                        towerBaseLevel = TowerBaseLevel.Level_3;
                        text_UpgradeTowerBaseCost.text = $"MAX";
                        TowerBaseSprite();
                    }
                    break;
            }
        }
    }

    private void TowerWeaponSprite()
    {
        switch (towerWeaponLevel)
        {
            case TowerWeaponLevel.Level_1:
                spriteRndr_TowerWeapon.sprite = towerWeaponASprite[0];
                break;

            case TowerWeaponLevel.Level_2:
                spriteRndr_TowerWeapon.sprite = towerWeaponASprite[1];
                break;

            case TowerWeaponLevel.Level_3:
                spriteRndr_TowerWeapon.sprite = towerWeaponASprite[2];
                break;
        }
    }

    private void TowerBaseSprite()
    {
        switch (towerBaseLevel)
        {
            case TowerBaseLevel.Level_1:
                firePoint.position = towerWeaponTransforms[0].position;
                spriteRndr_TowerBase.sprite = towerBaseASprite[0];
                break;

            case TowerBaseLevel.Level_2:
                firePoint.position = towerWeaponTransforms[1].position;
                spriteRndr_TowerBase.sprite = towerBaseASprite[1];
                break;

            case TowerBaseLevel.Level_3:
                firePoint.position = towerWeaponTransforms[2].position;
                spriteRndr_TowerBase.sprite = towerBaseASprite[2];
                break;
        }

        spriteRndr_TowerWeapon.gameObject.transform.position = firePoint.position;
    }

    private void DetectEnemy()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, enemyDetectRange, enemyLayer);

        float closestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        foreach (Collider2D enemy in hitEnemies)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < enemyDetectRange)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = enemy.gameObject;
                currentState = TowerState.Attack;
                targetedEnemy = closestEnemy;
            }
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, enemyDetectRange);
    }

    private void RangeAttack()
    {
        if (Time.time > nextRangeAttack && targetedEnemy != null)
        {
            nextRangeAttack = Time.time + attackCooldownDuration;

            GameObject bullet = Instantiate(bulletPrefab);
            bullet.transform.position = firePoint.position;
            SoundManager.instance.PlayTowerASounds(0, transform.position);

            switch(towerType)
            {
                case TowerType.A:
                    bullet.GetComponent<Bullet_TowerA>().target = targetedEnemy;
                    break;

                case TowerType.B:
                    //bullet.GetComponent<Bullet_TowerA>().target = targetedEnemy;
                    break;

                case TowerType.C:
                    //bullet.GetComponent<Bullet_TowerA>().target = targetedEnemy;
                    break;
            }
        }
        else if (targetedEnemy == null)
        {
            currentState = TowerState.Idle;
        }
    }

    private void Facing()
    {

    }

    public override void TakeDamage(float damageValue)
    {
        StartCoroutine(Tremble());
        SoundManager.instance.PlayTowerASounds(1, transform.position);
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            
        }
    }

    private void ChooseAnimation()
    {
        anim_TowerWeapon.SetBool("IsIdle", false);
        anim_TowerWeapon.SetBool("IsAttack", false);
        anim_TowerWeapon.SetBool("IsDestroy", false);

        switch (currentState)
        {
            case TowerState.Idle:
                anim_TowerWeapon.SetBool("IsIdle", true);
                break;
            case TowerState.Attack:
                anim_TowerWeapon.SetBool("IsAttack", true);
                break;
            case TowerState.Destroyed:
                anim_TowerWeapon.SetBool("IsDestroy", true);
                break;
        }
    }

    private IEnumerator Tremble()
    {
        for (int i = 0; i < 10; i++)
        {
            spriteRndr_TowerBase.transform.localPosition += new Vector3(0.08f, 0, 0);
            spriteRndr_TowerWeapon.transform.localPosition += new Vector3(0.08f, 0, 0);
            yield return new WaitForSeconds(0.01f);
            spriteRndr_TowerBase.transform.localPosition -= new Vector3(0.08f, 0, 0);
            spriteRndr_TowerWeapon.transform.localPosition -= new Vector3(0.08f, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
