using SomethingSpecific.ProtoNinja;
using UnityEngine;

public class SmokeGrenade : MonoBehaviour, IPowerup
{
    
    public Player OwningPlayer { get; set; }

    [SerializeField] private GameObject smokeEffectPrefab;
    [SerializeField] private float smokeDurationInSeconds = 3;

    private float createTime;
<<<<<<< HEAD
    private float currentTime;
=======
>>>>>>> master
    private GameObject smokeEffect;
    
    void Start()
    {
        // start the smoke animation
<<<<<<< HEAD
        smokeEffect = Instantiate(smokeEffectPrefab, gameObject.transform.position + new Vector3(0, 3, 0), Quaternion.identity);
        createTime = currentTime = Time.deltaTime;
    }

    void Update()
    {
        currentTime += Time.deltaTime;
        
        if (currentTime - createTime > smokeDurationInSeconds)
        {
            Debug.Log("destroying smoke");
=======
        smokeEffect = Instantiate(smokeEffectPrefab, gameObject.transform.position, Quaternion.identity);
        createTime = Time.deltaTime;
    }

    private void Update()
    {
        var now = Time.deltaTime;
        if (now - createTime > smokeDurationInSeconds)
        {
>>>>>>> master
            Destroy(smokeEffect);
            Destroy(gameObject);
        }
    }
}
