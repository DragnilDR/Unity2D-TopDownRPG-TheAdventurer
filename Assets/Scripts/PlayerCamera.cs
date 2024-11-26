using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera Instance;

    private Camera cachedCamera;

    public Transform playerTransform;
    private Vector3 PlayerPosition => playerTransform.position;

    public bool movingLock = false;

    [SerializeField] private Vector3 offset;
    [SerializeField] private float damping;
    [SerializeField] private float threshold;

    private Vector3 velocity = Vector3.zero;

    public Vector2 maxPos;
    public Vector2 minPos;

    [Header("Find Enemy")]
    [SerializeField] private float searchRadius;
    [SerializeField] private LayerMask enemyLayer;

    private Transform targetEnemy;

    private void Start()
    {
        Instance = this;

        playerTransform = FindAnyObjectByType<Player>().transform;
        cachedCamera = GetComponent<Camera>();

        transform.position = PlayerPosition;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(2)) 
        {
            if (targetEnemy == null)
            {
                GameObject nearestEnemy = FindNearestEnemy.Find(playerTransform.position, searchRadius, enemyLayer);

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

        if (targetEnemy != null && Vector3.Distance(playerTransform.position, targetEnemy.position) > searchRadius)
        {
            targetEnemy = null;
        }
    }

    private void FixedUpdate()
    {
        if (!movingLock)
            Follow();
    }

    private void Follow()
    {
        if (playerTransform != null && Menu.Instance.pauseGame == false)
        {
            Vector3 targetPos;

            if (targetEnemy != null)
            {
                
                targetPos = targetEnemy.position;
            }
            else
            {
                
                Vector3 mousePos = cachedCamera.ScreenToWorldPoint(Input.mousePosition);
                targetPos = (PlayerPosition + mousePos) / 2;

                targetPos.x = Mathf.Clamp(targetPos.x, -threshold + PlayerPosition.x, threshold + PlayerPosition.x);
                targetPos.y = Mathf.Clamp(targetPos.y, -threshold + PlayerPosition.y, threshold + PlayerPosition.y);
            }

            targetPos.z = -2f;

            Vector3 movePosition = targetPos + offset;

            transform.position = Vector3.SmoothDamp(transform.position, movePosition, ref velocity, damping);

            transform.position = new Vector3(Mathf.Clamp(transform.position.x, minPos.x, maxPos.x),
                                         Mathf.Clamp(transform.position.y, minPos.y, maxPos.y),
                                         transform.position.z);
        }
    }

    private void OnDrawGizmos()
    {
        if (playerTransform != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(playerTransform.position, searchRadius);
        }
    }
}
