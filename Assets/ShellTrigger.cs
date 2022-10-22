using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShellTrigger : MonoBehaviour
{
    public UnityEvent OnHit = new UnityEvent();

    private Rigidbody2D rigidbody2D;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnHit?.Invoke();

        //Debug.Log($"Collided with {collision.name}");
        DisableObject();
    }

    private void DisableObject()
    {
        rigidbody2D.velocity = Vector2.zero;
        gameObject.SetActive(false);
    }
}
