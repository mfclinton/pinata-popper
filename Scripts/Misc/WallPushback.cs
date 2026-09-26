using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WallPushback : MonoBehaviour
{
    [SerializeField] private Vector2 pushDirection = Vector2.left;
    [SerializeField] private float forceAmount = 1f;

    private List<Rigidbody2D> affectedRigidbodies = new List<Rigidbody2D>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Contains("Pinata"))
        {
            Rigidbody2D rb = other.attachedRigidbody;
            if (rb != null && !affectedRigidbodies.Contains(rb))
            {
                affectedRigidbodies.Add(rb);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag.Contains("Pinata"))
        {
            Rigidbody2D rb = other.attachedRigidbody;
            if (rb != null && affectedRigidbodies.Contains(rb))
            {
                rb.isKinematic = false;
                affectedRigidbodies.Remove(rb);
            }
        }
    }

    private void FixedUpdate()
    {
        foreach (var rb in affectedRigidbodies)
        {
            // Since we set it to kinematic, we have to move it manually
            rb.isKinematic = true;
            rb.MovePosition(rb.position + pushDirection.normalized * forceAmount * Time.fixedDeltaTime);
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 direction = new Vector3(pushDirection.x, pushDirection.y, 0).normalized;
        Vector3 position = transform.position;
        Gizmos.DrawLine(position, position + direction * 2); // Draw line twice the length for better visibility
    }
}