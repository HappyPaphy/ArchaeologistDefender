using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artifact : CharacterEntity
{
    public enum ArtifactState { Idle, Destroyed};
    [SerializeField] private ArtifactState currentState;

    [Header("Components")]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody2D rb;

    [Header("Variables")]
    [SerializeField] private bool isBeingMove;
    [SerializeField] private bool isOutOfBound;

    [SerializeField] private float playerRange;
    [SerializeField] private Vector3 spawnPosition; 

    public static Artifact instance;

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

        base.Awake();
    }

    protected override void Start()
    {
        spawnPosition = gameObject.transform.position;
    }

    protected override void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, PlayerController.instance.transform.position);
        float distanceToSpawn = Vector2.Distance(transform.position, PlayerController.instance.transform.position);

        if (distanceToSpawn < 0.03f && !GameManager.instance.IsArtifactBeingStole())
        {
            transform.position = spawnPosition;
        }
        else if(distanceToSpawn >= 0.03f && !GameManager.instance.IsArtifactBeingStole())
        {
            if (distanceToPlayer < playerRange)
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    transform.position = PlayerController.instance.transform.position;
                }
            }
        }

        ChooseAnimation();
        Checkhealth();
        base.Update();
    }

    private void Checkhealth()
    {
        if (CharacterHealthComponent.CurrentHP <= 0)
        {
            currentState = ArtifactState.Destroyed;
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

    private void ChooseAnimation()
    {
        anim.SetBool("IsIdle", false);
        anim.SetBool("IsDestroy", false);

        switch (currentState)
        {
            case ArtifactState.Idle:
                anim.SetBool("IsIdle", true);
                break;
            case ArtifactState.Destroyed:
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
