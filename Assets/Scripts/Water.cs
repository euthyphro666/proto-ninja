using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SomethingSpecific.ProtoNinja
{
    public class Water : MonoBehaviour
    {

        void OnTriggerEnter(Collider other)
        {
            Debug.Log("Entered");
            if (other.TryGetComponent(typeof(Player), out var comp) &&
                comp is Player player)
            {
                player.SlowDown();
            }
        }
        void OnTriggerExit(Collider other)
        {
            Debug.Log("exited");
            if (other.TryGetComponent(typeof(Player), out var comp) &&
                comp is Player player)
            {
                player.NormalSpeed();
            }
        }
    }
}
