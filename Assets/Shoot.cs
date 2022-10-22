using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Shoot : MonoBehaviour
{
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject ShellPrefab;
    [SerializeField] float reloadDelay = 0.1f;
    [SerializeField] float despawnTime = 5;
    [SerializeField] float shellSpeed = 50.0f;

    public UnityEvent OnShoot, OnCantShoot;
    public UnityEvent<float> OnReloading; 

    private bool canShoot = true;
    public void Start()
    {
        OnReloading?.Invoke(reloadDelay);
    }

    public void Update()
    {
        if (canShoot)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                canShoot = false;
                ShootShell();
                StartCoroutine(ShootingYield());
            }
        }
        else
        {
            OnCantShoot?.Invoke();
        }

    }

    IEnumerator ShootingYield()
    {
        yield return new WaitForSeconds(reloadDelay);
        canShoot = true;
    }

    public void ShootShell()
    {
        var newShell = Instantiate(ShellPrefab, spawnPoint.position, spawnPoint.rotation);        
        newShell.GetComponent<Rigidbody2D>().velocity = newShell.transform.up * shellSpeed;

        Destroy(newShell, despawnTime);

        OnShoot?.Invoke();
        OnReloading.Invoke(reloadDelay);
    }
}
