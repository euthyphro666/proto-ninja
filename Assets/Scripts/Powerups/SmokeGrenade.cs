using SomethingSpecific.ProtoNinja;
using UnityEngine;

public class SmokeGrenade : MonoBehaviour, IPowerup
{
    
    public Player OwningPlayer { get; set; }

    [SerializeField] private GameObject smokeEffectPrefab;
    [SerializeField] private float smokeDurationInSeconds = 3;

    private float createTime;
    private GameObject smokeEffect;
    
    void Start()
    {
        // start the smoke animation
        smokeEffect = Instantiate(smokeEffectPrefab, gameObject.transform.position, Quaternion.identity);
        createTime = Time.deltaTime;
    }

    private void Update()
    {
        var now = Time.deltaTime;
        if (now - createTime > smokeDurationInSeconds)
        {
            Destroy(smokeEffect);
            Destroy(gameObject);
        }
    }
}
