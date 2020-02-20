using SomethingSpecific.ProtoNinja;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField]
    private GameObject powerUpPrefab;
    
    [SerializeField]
    private Sprite powerUpIcon;
    
    void Start()
    {
        if(powerUpPrefab == null) Debug.LogError("PickUp is missing the GameObject prefab to instantiate on use.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(typeof(Player), out var comp) &&
            comp is Player player)
        {
            Debug.Log($"PowerUp collected by {player.Id}!");
            Destroy(gameObject);

            // Could add a damage value here later if we want
            player.ProcessPickUp(other.gameObject, powerUpPrefab, powerUpIcon);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
