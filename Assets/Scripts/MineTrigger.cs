using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MineTrigger : MonoBehaviour
{
    [SerializeField] private int damage = 30;    
    
    public UnityEvent OnHit = new UnityEvent();
    private Rigidbody2D shellRigidbody2D;

    private void Awake()
    {
        shellRigidbody2D = GetComponent<Rigidbody2D>();    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only tanks, not shells
        if (collision.tag == "BaseCampAsset")
        {
            OnHit?.Invoke();

            var damagable = collision.GetComponent<Damagable>();
            if (damagable != null)
                damagable.Hit(damage);

            //Debug.Log($"Collided with {collision.name}");
            DisableObject();
        }
    }

    private void DisableObject()
    {
        shellRigidbody2D.velocity = Vector2.zero;
        gameObject.SetActive(false);
    }
}
