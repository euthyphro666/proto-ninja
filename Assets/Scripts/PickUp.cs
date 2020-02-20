using System.Collections;
using System.Collections.Generic;
using SomethingSpecific.ProtoNinja;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField]
    private GameObject powerUpPrefab;
    
    void Start()
    {
        if(powerUpPrefab == null) Debug.LogError("PickUp script is missing its prefab!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(typeof(Player), out var comp) &&
            comp is Player player)
        {
            Debug.Log($"PowerUp collected by {player.Id}!");
            Destroy(gameObject);

            // Could add a damage value here later if we want
            player.ProcessPickUp(other.gameObject, powerUpPrefab);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
