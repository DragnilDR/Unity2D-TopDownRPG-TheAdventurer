using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    [SerializeField] private float lifeTime = .5f;

    private void Start()
    {
        Invoke(nameof(DestroyBullet), lifeTime);
    }

    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, .5f);

        if (hit.collider != null)
        {
            HandleCollision(hit.collider);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        HandleCollision(collider);
    }

    private void HandleCollision(Collider2D collider)
    {
        switch (collider.tag)
        {
            case "Enemy":
                collider.GetComponent<Enemy>().TakeDamage(damage);
                DestroyBullet();
                break;
            default:
                if (!collider.isTrigger)
                    DestroyBullet();
                break;
        }
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
