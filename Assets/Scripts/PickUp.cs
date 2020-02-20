using SomethingSpecific.ProtoNinja;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField]
    private GameObject powerupPrefab;
    
    [SerializeField]
    private Sprite powerupIcon;
    
    void Start()
    {
        if(powerupPrefab == null) Debug.LogError("PickUp is missing the GameObject prefab to instantiate on use.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(typeof(Player), out var comp) &&
            comp is Player player)
        {
            Debug.Log($"PowerUp collected by {player.Id}!");
            Destroy(gameObject);

            player.ProcessPickup(powerupPrefab, powerupIcon);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
