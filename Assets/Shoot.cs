using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject ShellPrefab;
    [SerializeField] float reloadDelay = 0.1f;
    [SerializeField] float despawnTime = 5;
    [SerializeField] float shellSpeed = 50.0f;

    private bool canShoot = true;

    public void Update()
    {
        if (Input.GetButtonDown("Fire1") && canShoot)
        {
            canShoot = false;
            ShootShell();
            StartCoroutine(ShootingYield());
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
    }
}
