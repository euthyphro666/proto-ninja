using UnityEngine;
namespace SomethingSpecific.ProtoNinja
{
    public enum ProjectileType
    {
        Normal,
        Fanout,
        Rapid
    }

    public class Projectile : MonoBehaviour
    {
        public int Owner;
        public Vector3 Delta;
        public float RotationSpeed = 5f;
        public int Damage = 1;

        private float LifeTime = 2f;
        private float LifeTimer;

        private void Update()
        {
            LifeTimer += Time.deltaTime;
            if (LifeTimer >= LifeTime)
                Destroy(gameObject);
            transform.Translate(Delta, Space.World);
            transform.Rotate(0, RotationSpeed * Time.deltaTime, 0);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(typeof(Player), out var comp) &&
                comp is Player player)
            {
                if (player.Id == Owner) return;

                Debug.Log("Hit!");
                Destroy(gameObject);

                // Could add a damage value here later if we want
                player.ProcessHit(other.gameObject, Damage);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
