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
        public float ProjectileSpeed = 0.5f;
        public float SlowDownRate = 0.5f;


        private Vector3 LookVector;
        private Vector3 MoveVector;
        private Rigidbody Body;
        private float ShootTimer;
        private float DodgeTimer;
        private float FreezeTimer;
        private bool Slowed;
        private float LastLX;
        private float LastLY;

        private Rewired.Player Controller;

        private ProjectileType FireMode;

        void Start()
        {
            Controller = ReInput.players.GetPlayer(Id);
            ShootTimer = 0;
            LookVector = new Vector3();
            MoveVector = new Vector3();
            Body = GetComponent<Rigidbody>();
            LastLY = 1f;
            FireMode = ProjectileType.Normal;
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
               transform.position.x +
                (mx * Speed * Time.deltaTime * (Slowed ? SlowDownRate : 1f)),
               transform.position.y,
               transform.position.z +
                (my * Speed * Time.deltaTime) * (Slowed ? SlowDownRate : 1f));
            Body.MovePosition(MoveVector);
            Body.velocity = Vector3.zero;

            // Look vector - simply look at our position translated by the look vector
            var lx = Controller.GetAxis("LookX");
            var ly = Controller.GetAxis("LookY");
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

            ProcessDodge();
            ProcessAttack();
            CheckToggleAttack();
        }

        private void ProcessDodge()
        {
            if (DodgeTimer <= 0)
            {
                if (Controller.GetButton("Dodge"))
                {

                }
            }
        }
        private void CheckToggleAttack()
        {
            // Toggle Attack
            if (Controller.GetButtonDown("ToggleAttack"))
            {
                Debug.Log("Toggled attack");
                FireMode = FireMode == ProjectileType.Normal ?
                        ProjectileType.Fanout :
                        FireMode == ProjectileType.Fanout ?
                        ProjectileType.Rapid :
                        ProjectileType.Normal;
            }
        }
        private void ProcessAttack()
        {
            // Attack
            if (ShootTimer <= 0)
            {
                if (Controller.GetAxis("RangedAttack") > 0)
                {
                    var shootVectors = new List<Vector3>();
                    var cooldown = 0f;
                    switch (FireMode)
                    {
                        case ProjectileType.Normal:
                            var normalVector = new Vector3(LastLX, 0, LastLY);
                            normalVector.Normalize();
                            normalVector *= ProjectileSpeed;
                            shootVectors.Add(normalVector);
                            cooldown = 0.25f;
                            break;
                        case ProjectileType.Fanout:
                            var mid = new Vector3(LastLX, 0, LastLY);
                            mid.Normalize();
                            mid *= ProjectileSpeed;
                            var left = new Vector3(LastLX, 0, LastLY);
                            left.Normalize();
                            left *= ProjectileSpeed;
                            left = Quaternion.Euler(0, -15, 0) * left;
                            var right = new Vector3(LastLX, 0, LastLY);
                            right.Normalize();
                            right *= ProjectileSpeed;
                            right = Quaternion.Euler(0, 15, 0) * right;
                            shootVectors.Add(mid);
                            shootVectors.Add(left);
                            shootVectors.Add(right);
                            cooldown = 0.5f;
                            break;
                        case ProjectileType.Rapid:
                            var rapidVector = new Vector3(LastLX, 0, LastLY);
                            rapidVector.Normalize();
                            rapidVector *= ProjectileSpeed;
                            shootVectors.Add(rapidVector);
                            cooldown = 0.125f;
                            break;
                    }
                    for (var i = 0; i < shootVectors.Count; i++)
                    {
                        var shootVector = shootVectors[i];
                        var instance = GameObject.Instantiate(
                            ProjectilePrefab,
                            transform.position + (shootVector * 1.25f),
                            Quaternion.identity);
                        var projectile = instance.GetComponent<Projectile>();
                        projectile.Owner = Id;
                        projectile.Delta = shootVector;
                        ShootTimer = cooldown;
                    }
                }
            }
            else
            {
                ShootTimer -= Time.deltaTime;
            }
        }

        public void NormalSpeed()
        {
            Slowed = false;
        }

        public void SlowDown()
        {
            Slowed = true;
        }

        public void Freeze(float time)
        {
            FreezeTimer = time;
        }
    }
}