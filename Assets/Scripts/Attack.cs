using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private Transform target;
    [SerializeField] private float startChargedAttackTime;
    [SerializeField] private float chargedAttackTime;
    [SerializeField] private float detectionDistance;
    [SerializeField] private float moveDistance;  // Distance the enemy should move towards the target

    [SerializeField] private float oblastAttackRange;
    [SerializeField] private LayerMask playerLayer;

    public int type = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float playerDist = Vector3.Distance(transform.position, target.position);

        if (playerDist <= detectionDistance)
        {
            if (chargedAttackTime <= 0)
            {
                chargedAttackTime = startChargedAttackTime;
                StartCoroutine(ChargedAttack());
            }
            else chargedAttackTime -= Time.deltaTime;
        }
    }

    private IEnumerator ChargedAttack()
    {
        yield return new WaitForSeconds(.5f);

        switch (type)
        {
            case 0:
                yield return StartCoroutine(Dash());
                break;
            case 1:
                OblastAttack();
                break;
        }
        

        // Reset velocity after movement
        rb.velocity = Vector2.zero;
    }

    private IEnumerator Dash()
    {
        Vector2 startPos = rb.position;
        Vector2 dir = (target.position - transform.position).normalized;
        float distanceCovered = 0f;

        while (distanceCovered < moveDistance)
        {
            rb.velocity = dir * 2;  // Adjust speed as necessary
            yield return new WaitForFixedUpdate();
            distanceCovered = Vector2.Distance(startPos, rb.position);
        }

        // Stop movement once the distance is covered
        rb.velocity = Vector2.zero;
    }

    private void OblastAttack()
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(transform.position, oblastAttackRange, playerLayer, ~gameObject.layer);

        foreach (Collider2D player in hitPlayer)
        {
            Debug.Log(player.gameObject.name);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, detectionDistance);
        Gizmos.DrawWireSphere(transform.position, oblastAttackRange);
    }
}
