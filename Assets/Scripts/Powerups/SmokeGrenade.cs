using SomethingSpecific.ProtoNinja;
using UnityEngine;

public class SmokeGrenade : MonoBehaviour, IPowerup
{
    
    public Player OwningPlayer { get; set; }

    [SerializeField] private GameObject smokeEffectPrefab;
    [SerializeField] private float smokeDurationInSeconds = 3;

    private float createTime;
    private float currentTime;
    private GameObject smokeEffect;
    
    void Start()
    {
        // start the smoke animation
        smokeEffect = Instantiate(smokeEffectPrefab, gameObject.transform.position + new Vector3(0, 3, 0), Quaternion.identity);
        createTime = currentTime = Time.deltaTime;
    }

    void Update()
    {
        currentTime += Time.deltaTime;
        
        if (currentTime - createTime > smokeDurationInSeconds)
        {
            Debug.Log("destroying smoke");
            Destroy(smokeEffect);
            Destroy(gameObject);
        }
    }
}
