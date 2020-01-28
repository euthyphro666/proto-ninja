using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public string Owner;
    public Vector3 Delta;

    private float LifeTime = 2f;
    private float LifeTimer;

    void Update()
    {
        LifeTimer += Time.deltaTime;
        if (LifeTimer >= LifeTime)
            Destroy(gameObject);
        transform.Translate(Delta, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(typeof(Player), out var comp) &&
            comp is Player player)
        {
            if (player.Id != Owner)
            {
                Debug.Log("Hit!");
                Destroy(gameObject);
                Destroy(other.gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
