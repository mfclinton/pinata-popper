using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public abstract class AShootable : APoolable
{
    public event Action OnShootableLoadedEvent;
    
    public Collider2D ShootableCollider { get; private set; }
    public Rigidbody2D ShootableRigidbody { get; private set; }

    private void Awake()
    {
        ShootableCollider = GetComponent<Collider2D>();
        ShootableRigidbody = GetComponent<Rigidbody2D>();
    }
    
    public void OnShootableLoaded()
    {
        gameObject.SetActive(true);
        ShootableCollider.enabled = false;
        ShootableRigidbody.bodyType = RigidbodyType2D.Static;
        OnShootableLoadedEvent?.Invoke();
    }
    public void OnShootableUnloaded()
    {
        gameObject.SetActive(false);
    }

    public void Fire(Vector3 force)
    {
        OnShootableFired();
        ShootableRigidbody.AddForce(force, ForceMode2D.Impulse);
    }
    
    private void OnShootableFired()
    {
        gameObject.SetActive(true);
        ShootableCollider.enabled = true;
        ShootableRigidbody.bodyType = RigidbodyType2D.Dynamic;
    }
}