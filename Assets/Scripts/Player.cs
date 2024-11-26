using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;
    private Camera cachedCamera;
    
    private enum MoveActionType
    {
        Waiting,
        Running,
        Dashing
    }

    [SerializeField] private MoveActionType moveActionType;

    public enum AttackType
    {
        Melle,
        Range
    }

    public AttackType attackType;

    [Space(10)]
    [SerializeField] private Transform hands;
    [SerializeField] private Transform attackPosition;

    [SerializeField] private GameObject loadObject;
    private Image loadImage;

    public GameObject projectile;

    [SerializeField] private float staminaRecoveryDelay = 1f; // Время задержки для восстановления стамины
    private float lastActionTime = 0f; // Время последнего выполнения действия

    [SerializeField] private float searchRadius;

    [Header("Sound")]
    [SerializeField] private AudioClip heartBeat;

    [Header("Stats")]
    public Stats stats = new();
    [SerializeField] private float maxStamina;
    
    [Header("Movement")]
    private Vector3 moveDirection;
    private Vector3 lookDirection;
    private Vector3 dashDirection;

    [Header("Attack")]
    [SerializeField] private float meleeAttackRange;
    public float comboTimeout = 0.5f; // Time window for combo input
    private int comboCount = 0;
    private float lastAttackTime = 0f;

    [Header("Layers")]
    [SerializeField] private LayerMask enemyLayer;

    [Header("Keybinds")]
    [SerializeField] private KeyCode dashKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode attackKey = KeyCode.Mouse0;

    [Header("MatBlink")]
    private Material matBlink;
    private Material matDefault;
    private SpriteRenderer spriteRenderer;

    private Transform targetEnemy;

    public static event System.Action OnDeleteSave;

    public bool IsAlive => stats.Health >= 0;
    public bool IsLowHP => stats.Health <= 10;

    private bool readyToAttack = true; 

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        cachedCamera = Camera.main;

        maxStamina = stats.Stamina;

        spriteRenderer = GetComponent<SpriteRenderer>();
        matBlink = Resources.Load("Blink", typeof(Material)) as Material;
        matDefault = spriteRenderer.material;

        loadImage = loadObject.GetComponent<Image>();
        loadImage.fillAmount = 0;
        loadObject.SetActive(false);
    }

    private void Update()
    {
        if (IsLowHP)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.loop = true;
                audioSource.clip = heartBeat;
                audioSource.Play();
            }
        }
        else audioSource.Stop();

        // Определите время, прошедшее с момента последнего действия
        float timeSinceLastAction = Time.time - lastActionTime;

        // Если прошло достаточно времени после последнего действия, начните восстанавливать стамину
        if (timeSinceLastAction >= staminaRecoveryDelay)
        {
            stats.Stamina = Mathf.Clamp(stats.Stamina + 5, 0, 100);
        }

        HandleInput();

        if (moveActionType == MoveActionType.Dashing)
            StartCoroutine(Dash());
        else StopAllCoroutines();

        if (rb.velocity == Vector2.zero)
            moveActionType = MoveActionType.Waiting;

        if (Input.GetMouseButtonDown(2))
        {
            if (targetEnemy == null)
            {
                GameObject nearestEnemy = FindNearestEnemy.Find(transform.position, searchRadius, enemyLayer);

                if (nearestEnemy != null)
                {
                    targetEnemy = nearestEnemy.transform;
                }
            }
            else
            {
                targetEnemy = null;
            }
        }

        if (targetEnemy != null && Vector3.Distance(transform.position, targetEnemy.position) > searchRadius)
        {
            targetEnemy = null;
        }
    }

    private void FixedUpdate()
    {
        HandMove();

        if (moveActionType != MoveActionType.Dashing)
            Movement();
    }

    private void LateUpdate()
    {
        animator.SetFloat("Horizontal", moveDirection.x);
        animator.SetFloat("Vertical", moveDirection.y);

        animator.SetFloat("HorizontalAttack", lookDirection.x);
        //animator.SetFloat("VerticalAttack", lookDirection.y);

        animator.SetBool("IsRunning", moveDirection.sqrMagnitude > 0);

        if (moveDirection.x != 0 || moveDirection.y != 0)
        {
            animator.SetFloat("LastHorizontal", moveDirection.x);
            animator.SetFloat("LastVertical", moveDirection.y);
        }
    }

    private void HandleInput()
    {
        if (Menu.Instance.pauseGame == false)
        {
            float stamina = Mathf.Lerp(maxStamina, 0f, .7f);

            if (PlayerCamera.Instance.movingLock == false)
            {
                moveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

                if (Input.GetKeyDown(dashKey) && stats.Stamina >= stamina)
                {
                    dashDirection = moveDirection;
                    moveActionType = MoveActionType.Dashing;
                    stats.Stamina -= 33.33f;
                    lastActionTime = Time.time;
                }
                
                if (Input.GetKeyDown(attackKey) && stats.Stamina >= stamina && readyToAttack)
                {
                    if (Time.time - lastAttackTime > comboTimeout)
                    {
                        comboCount = 0; // Reset combo if time since last attack is greater than timeout
                    }

                    switch (attackType)
                    {
                        case AttackType.Melle:
                            ComboAttack();
                            break;
                    }
                }

                if (attackType == AttackType.Range)
                {
                    if (Input.GetKey(attackKey) && stats.Stamina >= stamina && readyToAttack)
                    {
                        loadObject.SetActive(true);

                        ChargingAttack();
                    }

                    if (Input.GetKeyUp(attackKey))
                    {
                        loadObject.SetActive(false);

                        loadImage.fillAmount = 0;
                    }
                }
            }
        }
    }

    private void Movement()
    {
        moveActionType = MoveActionType.Running;
        rb.velocity = moveDirection * stats.Speed;
    }

    private void HandMove()
    {
        if (targetEnemy != null)
        {
            lookDirection = targetEnemy.position - hands.position;
        }
        else
        {
            lookDirection = cachedCamera.ScreenToWorldPoint(Input.mousePosition) - hands.position;
        }
        
        float rotationZ = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        hands.rotation = Quaternion.Euler(0f, 0f, rotationZ);
    }

    private IEnumerator Dash()
    {
        animator.SetTrigger("Dash");

        while (true)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dashDirection, 0.5f, LayerMask.GetMask("Wall"));

            if (hit.collider != null)
            {
                rb.velocity = Vector2.zero;
                break;
            }

            rb.velocity = dashDirection * 10;
            yield return new WaitForFixedUpdate();

            yield return new WaitForSeconds(.3f);
            moveActionType = MoveActionType.Waiting;
        }
    }

    private void ResetAttack() => readyToAttack = true;

    private void MeleeAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition.position, meleeAttackRange, enemyLayer, ~gameObject.layer);

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(stats.Damage);
        }

        animator.SetFloat("LastHorizontal", lookDirection.x);
        animator.SetFloat("LastVertical", lookDirection.y);
    }

    private void ComboAttack()
    {
        readyToAttack = false;
        stats.Stamina -= 33.33f;
        lastActionTime = Time.time;
        
        switch (comboCount)
        {
            case 0:
                SoundSystem.Instance.PlaySound("MelleAttack1");
                animator.SetTrigger("AttackSword1");
                comboCount++;
                break;
            case 1:
                SoundSystem.Instance.PlaySound("MelleAttack2");
                animator.SetTrigger("AttackSword2");
                comboCount++;
                break;
            case 2:
                SoundSystem.Instance.PlaySound("MelleAttack3");
                animator.SetTrigger("AttackSword1");
                comboCount++;
                break;
        }
        if (comboCount == 3)
        {
            // Reset combo count if max combo reached
            comboCount = 0;
            // Perform first attack again or add additional logic for extended combos
        }

        lastAttackTime = Time.time;
    }

    private void RangeAttack()
    {
        GameObject pt = Instantiate(projectile, attackPosition.position, attackPosition.rotation);

        if (pt.name == "Arrow(Clone)")
        {
            SoundSystem.Instance.PlaySound("Arrow");
            pt.transform.rotation *= Quaternion.Euler(0f, 0f, -45f);
        }
        else
        {
            SoundSystem.Instance.PlaySound("Fireball");
        }

        pt.GetComponent<Projectile>().damage = stats.Damage;

        Rigidbody2D ptRb = pt.GetComponent<Rigidbody2D>();
        ptRb.velocity = attackPosition.right * 30;
    }

    private void ChargingAttack()
    {
        loadImage.fillAmount += 2.5f * Time.deltaTime;

        if (loadImage.fillAmount >= 1)
        {
            readyToAttack = false;
            stats.Stamina -= 33.33f;
            lastActionTime = Time.time;
            loadImage.fillAmount = 0;

            switch (projectile.name)
            {
                case "Arrow":
                    animator.SetTrigger("AttackBow");
                    break;
                case "Fireball":
                    animator.SetTrigger("AttackStaff");
                    break;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        int totalDamage = damage - stats.Armor;

        if (totalDamage <= 0) stats.Health -= 0;
        else stats.Health -= totalDamage;

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

    private void Death()
    {
        FindAnyObjectByType<DeathScreenController>().SetActiveDeathScreen();
        OnDeleteSave?.Invoke();
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPosition == null)
            return;

        Gizmos.DrawWireSphere(attackPosition.position, meleeAttackRange);
    }
}
