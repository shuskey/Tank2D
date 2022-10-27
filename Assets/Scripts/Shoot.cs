using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


public class Shoot : MonoBehaviour
{
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject ShellPrefab;
    [SerializeField] float reloadDelay = 1.0f;
    [SerializeField] float despawnTime = 5;
    [SerializeField] float shellSpeed = 50.0f;

    public UnityEvent OnShoot, OnCantShoot;
    public UnityEvent<float> OnReloading; 

    private bool canShoot = true;
    private bool shootInputDetected = false;
    private float currentDelayCountdown = 0;

    public void Start()
    {
        OnReloading?.Invoke(reloadDelay);
    }

    //public void OnShootButtonPressed(InputAction.CallbackContext context)
    //{
    //    shootInputDetected = context.action.triggered;
    //}

    public void OnShootThisGun(bool shootIfTrue)
    {
        shootInputDetected = shootIfTrue;
    }

    public void Update()
    {
        if (canShoot)
        {
            if (shootInputDetected)  // Input.GetButtonDown("Fire1"))
            {
                canShoot = false;
                currentDelayCountdown = reloadDelay;
                ShootShell();
               // StartCoroutine(ShootingYield());
            }
        }
        else
        {
            OnCantShoot?.Invoke();
            currentDelayCountdown -= Time.deltaTime;
            if (currentDelayCountdown <= 0)
                canShoot = true;
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
