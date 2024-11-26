using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

    private Transform playerTransform;

    private Vector3 PlayerPosition => playerTransform.position;
    private Vector3 Position => transform.position;
    private float DistanceFromPlayer => Vector2.Distance(Position, PlayerPosition);

    [Space(10)]
    [SerializeField] private TextMeshPro enemyNameUI;
    [SerializeField] private GameObject popUpDamagePrefab;
    [SerializeField] private Transform hands;
    [SerializeField] private Transform attackPosition;

    [Header("Stats")]
    public Stats stats = new();

    [Header("Movement")]
    [SerializeField] private float minDetectionRange;
    [SerializeField] private float maxDetectionRange;
    [SerializeField] private Vector3 moveDirection;
    [SerializeField] private Vector3 lastMoveDirection;
    private float detectionRange;
    private Vector3 startPosition;

    [Header("Attack")]
    [SerializeField] private float attackRange;
    [SerializeField] private float meleeAttackRange;
    [Space(5)]
    [SerializeField] private float startTimeBtwAttack;
    [SerializeField] private float timeBtwAttack;

    [Header("Layer")]
    [SerializeField] private LayerMask playerLayer;

    [Header("MatBlink")]
    private Material matBlink;
    private Material matDefault;
    private SpriteRenderer spriteRenderer;


    private bool IsMoving = true;
    private bool IsFollowing = false;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        matBlink = Resources.Load("Blink", typeof(Material)) as Material;
        matDefault = spriteRenderer.material;

        playerTransform = FindObjectOfType<Player>().transform;

        enemyNameUI.text = gameObject.name;

        startPosition = Position;

        timeBtwAttack = startTimeBtwAttack;

        detectionRange = minDetectionRange;
    }

    private void Update()
    {
        float playerDistance = Vector2.Distance(PlayerPosition, transform.position);

        if (IsFollowing)
            detectionRange = maxDetectionRange;
        else 
            detectionRange = minDetectionRange;

        if (timeBtwAttack <= 0)
        {
            if (playerDistance < attackRange)
            {
                timeBtwAttack = startTimeBtwAttack;

                Vector2 playerDirection = PlayerPosition - Position;

                animator.SetFloat("HorizontalAttack", playerDirection.x);
                animator.SetFloat("VerticalAttack", playerDirection.y);

                animator.SetTrigger("Attack");
            }
        }
        else
            timeBtwAttack -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (playerTransform != null)
        {
            HandMove();

            if (IsMoving)
            {
                if (detectionRange >= DistanceFromPlayer)
                {
                    IsFollowing = true;
                    Movement((PlayerPosition - Position).normalized);

                    float distanceToPlayer = Vector3.Distance(Position, PlayerPosition);
                    float stoppingDistance = 1f;

                    if (distanceToPlayer <= stoppingDistance)
                        Movement(Vector3.zero);
                }
                else
                {
                    IsFollowing = false;
                    Movement((startPosition - Position).normalized);

                    float distanceToStart = Vector3.Distance(Position, startPosition);
                    float stoppingDistance = 0.1f;

                    if (distanceToStart <= stoppingDistance)
                        Movement(Vector3.zero);
                }
            }
            else Movement(Vector3.zero);
        }
    }

    private void LateUpdate()
    {
        animator.SetFloat("Horizontal", moveDirection.x);
        animator.SetFloat("Vertical", moveDirection.y);

        animator.SetBool("IsRunning", moveDirection.sqrMagnitude > 0);

        if (moveDirection.x != 0 || moveDirection.y != 0)
        {
            animator.SetFloat("LastHorizontal", moveDirection.x);
            animator.SetFloat("LastVertical", moveDirection.y);
        }
    }

    public void LockMove()
    {
        IsMoving = false;
    }

    public void UnLockMove()
    {
        IsMoving = true;
    }

    private void Movement(Vector3 direction)
    {
        moveDirection = direction;
        rb.velocity = moveDirection * stats.Speed;
    }

    private void HandMove()
    {
        Vector2 difference = PlayerPosition - hands.position;
        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        hands.rotation = Quaternion.Euler(0f, 0f, rotationZ);
    }

    private void MeleeAttack()
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPosition.position, meleeAttackRange, playerLayer, ~gameObject.layer);

        foreach (Collider2D player in hitPlayer)
        {
            player.GetComponent<Player>().TakeDamage(stats.Damage);
        }
    }

    public void TakeDamage(int damage)
    {
        int totalDamage = damage - stats.Armor;

        if (totalDamage <= 0)
        {
            stats.Health -= 0;
            ShowPopupDamage(0);
        }
        else
        {
            stats.Health -= totalDamage;
            ShowPopupDamage(totalDamage);
        }

        spriteRenderer.material = matBlink;

        if (stats.Health <= 0)
        {
            Death();
        }
        else Invoke(nameof(ResetMaterial), .1f);
    }

    private void ResetMaterial()
    {
        spriteRenderer.material = matDefault;
    }

    private void ShowPopupDamage(int damage)
    {
        var text = Instantiate(popUpDamagePrefab, transform.position + new Vector3(Random.Range(-1f, 1f), 1, 0), Quaternion.identity);

        text.GetComponent<TextMeshPro>().text = damage.ToString();

        Destroy(text, 1f);
    }

    private void Death()
    {
        GetComponent<ItemBag>().InstantiateItem(Position);

        foreach (var quest in QuestList.Instance.questList)
        {
            int goalNumber = quest.questGoalNumber;

            if (quest.questInfo.questGoal[goalNumber].goalType == QuestGoal.GoalType.Kill)
            {
                if (quest.questInfo.questGoal[goalNumber].needdedEnemyName == gameObject.name)
                {
                    quest.count++;
                }
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPosition == null)
            return;

        Gizmos.DrawWireSphere(attackPosition.position, meleeAttackRange);
        Gizmos.DrawWireSphere(Position, detectionRange);
        Gizmos.DrawWireSphere(Position, minDetectionRange);
        Gizmos.DrawWireSphere(Position, maxDetectionRange);
        Gizmos.DrawWireSphere(Position, attackRange);
    }
}
