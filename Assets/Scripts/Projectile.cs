using System.Collections;
using System.Collections.Generic;
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

        private float LifeTime = 2f;
        private float LifeTimer;

        void Update()
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
}
