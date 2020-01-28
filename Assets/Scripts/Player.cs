using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

namespace SomethingSpecific.ProtoNinja
{
    public class Player : MonoBehaviour
    {

        public int Id = 0;
        public float Speed = 5f;
        public GameObject ProjectilePrefab;
        public float FreezeTime = 1f;
        public float ShootTime = 0.25f;
        public float ProjectileSpeed = 0.5f;

        private Vector3 LookVector;
        private Vector3 MoveVector;
        private Rigidbody Body;
        private float ShootTimer;
        private float FreezeTimer;
        private float LastLX;
        private float LastLY;

        private Rewired.Player Controller;

        void Start()
        {
            Controller = ReInput.players.GetPlayer(Id);
            ShootTimer = 0;
            LookVector = new Vector3();
            MoveVector = new Vector3();
            Body = GetComponent<Rigidbody>();
            LastLX = 1f;
        }

        void Update()
        {
            if (FreezeTimer > 0)
            {
                FreezeTimer -= Time.deltaTime;
                return;
            }
            // Move vector - simply translate by this
            var mx = Controller.GetAxis("MoveX");
            var my = Controller.GetAxis("MoveY");
            MoveVector.Set(
               transform.position.x + (mx * Speed * Time.deltaTime),
               transform.position.y,
               transform.position.z + (my * Speed * Time.deltaTime));
            Body.MovePosition(MoveVector);
            Body.velocity = Vector3.zero;

            // Look vector - simply look at our position translated by the look vector
            var lx = -Controller.GetAxis("LookY");
            var ly = Controller.GetAxis("LookX");
            LookVector.Set(
               transform.position.x + lx,
               transform.position.y,
               transform.position.z + ly);
            transform.LookAt(LookVector);

            if (lx != 0 || ly != 0)
            {
                LastLX = lx;
                LastLY = ly;
            }

            // Attack
            if (ShootTimer >= ShootTime)
            {
                var attack = Controller.GetAxis("Attack");
                if (attack > 0)
                {
                    var shootVector = new Vector3(LastLY, 0, -LastLX);
                    shootVector.Normalize();
                    shootVector *= ProjectileSpeed;
                    var instance = GameObject.Instantiate(
                        ProjectilePrefab,
                        transform.position + (shootVector * 1.25f),
                        Quaternion.identity);
                    var projectile = instance.GetComponent<Projectile>();
                    projectile.Owner = Id;
                    projectile.Delta = shootVector;
                    ShootTimer = 0;
                }
            }
            else
            {
                ShootTimer += Time.deltaTime;
            }
        }

        public void Freeze()
        {
            FreezeTimer = FreezeTime;
        }
    }
}