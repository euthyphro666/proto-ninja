using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caterpillar : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(typeof(Player), out var comp) &&
            comp is Player player)
        {
            player.Freeze();
            Destroy(gameObject);
        }
    }
}
