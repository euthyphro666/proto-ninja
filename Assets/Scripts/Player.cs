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
        public float DodgeRate = 2f;
        public float DodgeCooldown = 1f;
        public float DodgeDuration = 0.125f;
        public float BlockCooldown = 1f;
        public float BlockDuration = 1f;

        [SerializeField] private bool DebugToggleShooting;

        private Vector3 LookVector;
        private Vector3 MoveVector;
        private Rigidbody Body;
        private Animator Anim;
        private float ShootTimer;
        private float FreezeTimer;
        private bool Blocking;
        private bool CanBlock;
        private bool Dodging;
        private bool CanDodge;
        private bool Slowed;
        private float LastLX;
        private float LastLY;
        private float LastMX;
        private float LastMY;

        private Rewired.Player Controller;

        private ProjectileType FireMode;


        private void Start()
        {
            Controller = ReInput.players.GetPlayer(Id);
            ShootTimer = 0;
            LookVector = new Vector3();
            MoveVector = new Vector3();
            Body = GetComponent<Rigidbody>();
            LastLY = 1f;
            FireMode = ProjectileType.Normal;
            Blocking = Dodging = false;
            CanBlock = CanDodge = true;
            Anim = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            if (FreezeTimer > 0)
            {
                FreezeTimer -= Time.deltaTime;
                return;
            }

            // Move vector - simply translate by this
            var mx = Controller.GetAxis("MoveX");
            var my = Controller.GetAxis("MoveY");
            if ((mx != 0 || my != 0) && !Dodging)
            {
                Anim.SetBool("IsRunning", true);
                LastMX = mx;
                LastMY = my;
            }
            else if (mx == 0 && my == 0)
            {
                Anim.SetBool("IsRunning", false);
            }

            var speedModifier = Speed * Time.deltaTime *
                                (Slowed ? SlowDownRate : 1f) *
                                (Dodging ? DodgeRate : 1f);
            MoveVector.Set(
                transform.position.x + ((Dodging ? LastMX : mx) * speedModifier),
                transform.position.y,
                transform.position.z + ((Dodging ? LastMY : my) * speedModifier));

            Body.MovePosition(MoveVector);
            Body.velocity = Vector3.zero;

            // Look vector - simply look at our position translated by the look vector
            var lx = Controller.GetAxis("LookX");
            var ly = Controller.GetAxis("LookY");
            if (lx != 0 || ly != 0)
            {
                LastLX = lx;
                LastLY = ly;
                // If we're looking somewhere look there
                LookVector.Set(
                    transform.position.x + lx,
                    transform.position.y,
                    transform.position.z + ly);
                transform.LookAt(LookVector);
            }
            else
            {
                // If we're not looking somewhere and we're moving look where we're moving.
                if (mx != 0 || my != 0)
                {
                    LastLX = mx;
                    LastLY = my;
                    LookVector.Set(
                        transform.position.x + mx,
                        transform.position.y,
                        transform.position.z + my);
                    transform.LookAt(LookVector);
                }
            }


            ProcessDodge();
            // Only allow attacking if we're not blocking
            ProcessBlock();
            if (!Blocking)
                ProcessAttack();
            CheckToggleAttack();
        }

        private void ProcessDodge()
        {
            if (CanDodge && Controller.GetButton("Dodge"))
            {
                StartCoroutine(PerformDodge());
            }
        }

        private IEnumerator PerformDodge()
        {
            CanDodge = false;
            Dodging = true;
            Anim.SetTrigger("HasDashed");

            Debug.Log($"Player {Id} Dodging");
            yield return new WaitForSeconds(DodgeDuration);

            Debug.Log($"Player {Id} Stopped Dodging");
            Dodging = false;

            yield return new WaitForSeconds(DodgeCooldown);
            CanDodge = true;
            Debug.Log($"Player {Id} Can Dodge Again");
        }

        private void CheckToggleAttack()
        {
            // Toggle Attack
            if (Controller.GetButtonDown("ToggleAttack"))
            {
                FireMode = FireMode == ProjectileType.Normal ? ProjectileType.Fanout :
                    FireMode == ProjectileType.Fanout ? ProjectileType.Rapid :
                    ProjectileType.Normal;
            }
        }

        /// <summary>
        /// Enables the blocking state and starts a cooldown
        /// </summary>
        private void ProcessBlock()
        {
            if (CanBlock && Controller.GetButtonDown("Block"))
            {
                StartCoroutine(PerformBlock());
            }
        }

        private IEnumerator PerformBlock()
        {
            // Enable the blocking state
            Debug.Log($"Player {Id} Blocking");
            Blocking = true;
            CanBlock = false;

            // Wait the blocking duration and then disable it
            yield return new WaitForSeconds(BlockDuration);
            Debug.Log($"Player {Id} Stopped Blocking");
            Blocking = false;

            // Wait the cooldown before block can be used again
            yield return new WaitForSeconds(BlockCooldown);
            CanBlock = true;

            Debug.Log($"Player {Id} Can Block Again");
        }

        private void ProcessAttack()
        {
            // Attack
            if (ShootTimer <= 0)
            {
                if (Controller.GetAxis("RangedAttack") > 0 || DebugToggleShooting)
                {
                    Anim.SetTrigger("HasThrown");
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

        /// <summary>
        /// Handles the player being hit
        /// </summary>
        public void ProcessHit(GameObject parent)
        {
            if (!Blocking)
                Destroy(parent);
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