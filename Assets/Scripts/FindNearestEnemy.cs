using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindNearestEnemy : MonoBehaviour
{
    public static GameObject Find(Vector3 position, float searchRadius, LayerMask layer)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(position, searchRadius, layer);

        GameObject nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {
            float distance = Vector2.Distance(position, hitCollider.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = hitCollider.gameObject;
            }
        }

        return nearestEnemy;
    }
}
