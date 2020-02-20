using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using SomethingSpecific.ProtoNinja.Events;

namespace SomethingSpecific.ProtoNinja
{
    public class Player : MonoBehaviour
    {

        #region Debug
        [SerializeField] private bool DebugToggleShooting;
        #endregion

        #region Public Fields
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
        public int MaxHealth = 5;
        #endregion

        #region Private Fields
        private GameObject activePowerup;
        private Rewired.Player controller;
        private ProjectileType fireMode;
        private Vector3 lookVector;
        private Vector3 moveVector;
        private Rigidbody body;
        private Animator anim;

        private float shootTimer;
        private float freezeTimer;
        private bool blocking;
        private bool canBlock;
        private bool dodging;
        private bool canDodge;
        private bool slowed;
        private float lastLX;
        private float lastLY;
        private float lastMX;
        private float lastMY;
        #endregion

        #region Props
        private int _Health;
        public int Health
        {
            get => _Health;
            private set
            {
                if (_Health != value && _Health > 0)
                {
                    _Health = value;
                    UpdateHealthEvent?.Invoke(this, new TypedEventArgs<int>(_Health));
                    if (_Health <= 0)
                    {
                        anim.SetBool("IsDead", true);
                    }
                }
            }
        }

        private Sprite PowerupIcon
        {
            set => UpdatePowerupEvent?.Invoke(this, new TypedEventArgs<Sprite>(value));
        }
        #endregion

        #region Events
        public event EventHandler<TypedEventArgs<int>> UpdateHealthEvent;
        public event EventHandler<TypedEventArgs<int>> UpdateDashEvent;
        public event EventHandler<TypedEventArgs<int>> UpdateBlockEvent;
        public event EventHandler<TypedEventArgs<Sprite>> UpdatePowerupEvent;
        #endregion

        #region Public Functions
        /// <summary>
        /// Handles the player being hit
        /// </summary>
        public void ProcessHit(int damage)
        {
            if (!blocking)
            {
                Health -= damage;
            }
        }

        public void ProcessPickup(GameObject powerupPrefab, Sprite powerupIcon)
        {
            PowerupIcon = powerupIcon;
            activePowerup = powerupPrefab;
        }

        // Destroy the player GameObject
        public void DestroyPlayer()
        {
            Destroy(gameObject);
        }

        public void NormalSpeed()
        {
            slowed = false;
        }

        public void SlowDown()
        {
            slowed = true;
        }

        public void Freeze(float time)
        {
            freezeTimer = time;
        }
        #endregion

        #region Private Functions
        private void Start()
        {
            controller = ReInput.players.GetPlayer(Id);
            shootTimer = 0;
            _Health = MaxHealth;
            lookVector = new Vector3();
            moveVector = new Vector3();
            body = GetComponent<Rigidbody>();
            lastLY = 1f;
            fireMode = ProjectileType.Normal;
            blocking = dodging = false;
            canBlock = canDodge = true;
            anim = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            if (freezeTimer > 0)
            {
                freezeTimer -= Time.deltaTime;
                return;
            }

            // Move vector - simply translate by this
            var mx = controller.GetAxis("MoveX");
            var my = controller.GetAxis("MoveY");
            if ((mx != 0 || my != 0) && !dodging)
            {
                anim.SetBool("IsRunning", true);
                lastMX = mx;
                lastMY = my;
            }
            else if (mx == 0 && my == 0)
            {
                anim.SetBool("IsRunning", false);
            }

            var speedModifier = Speed * Time.deltaTime *
                                (slowed ? SlowDownRate : 1f) *
                                (dodging ? DodgeRate : 1f);
            moveVector.Set(
                transform.position.x + ((dodging ? lastMX : mx) * speedModifier),
                transform.position.y,
                transform.position.z + ((dodging ? lastMY : my) * speedModifier));

            body.MovePosition(moveVector);
            body.velocity = Vector3.zero;

            // Look vector - simply look at our position translated by the look vector
            var lx = controller.GetAxis("LookX");
            var ly = controller.GetAxis("LookY");
            if (lx != 0 || ly != 0)
            {
                lastLX = lx;
                lastLY = ly;
                // If we're looking somewhere look there
                lookVector.Set(
                    transform.position.x + lx,
                    transform.position.y,
                    transform.position.z + ly);
                transform.LookAt(lookVector);
            }
            else
            {
                // If we're not looking somewhere and we're moving look where we're moving.
                if (mx != 0 || my != 0)
                {
                    lastLX = mx;
                    lastLY = my;
                    lookVector.Set(
                        transform.position.x + mx,
                        transform.position.y,
                        transform.position.z + my);
                    transform.LookAt(lookVector);
                }
            }


            ProcessDodge();
            // Only allow attacking if we're not blocking
            ProcessBlock();
            if (!blocking)
                ProcessAttack();
            CheckToggleAttack();
            ProcessPowerup();
        }

        private void ProcessPowerup()
        {
            if (activePowerup)
            {
                var powerup = Instantiate(activePowerup, transform);
                powerup.GetComponent<IPowerup>().OwningPlayer = this;
                activePowerup = null;
            }
            else
                Debug.LogWarning($"Unable to instantiate and use powerup for Player {Id}");
        }
        
        private void ProcessDodge()
        {
            if (canDodge && controller.GetButton("Dodge"))
            {
                StartCoroutine(PerformDodge());
            }
        }

        private IEnumerator PerformDodge()
        {
            canDodge = false;
            dodging = true;
            anim.SetTrigger("HasDashed");

            Debug.Log($"Player {Id} Dodging");
            yield return new WaitForSeconds(DodgeDuration);

            Debug.Log($"Player {Id} Stopped Dodging");
            dodging = false;

            yield return new WaitForSeconds(DodgeCooldown);
            canDodge = true;
            Debug.Log($"Player {Id} Can Dodge Again");
        }

        private void CheckToggleAttack()
        {
            // Toggle Attack
            if (controller.GetButtonDown("ToggleAttack"))
            {
                fireMode = fireMode == ProjectileType.Normal ? ProjectileType.Fanout :
                    fireMode == ProjectileType.Fanout ? ProjectileType.Rapid :
                    ProjectileType.Normal;
            }
        }

        /// <summary>
        /// Enables the blocking state and starts a cooldown
        /// </summary>
        private void ProcessBlock()
        {
            if (canBlock && controller.GetButtonDown("Block"))
            {
                StartCoroutine(PerformBlock());
            }
        }

        private IEnumerator PerformBlock()
        {
            // Enable the blocking state
            Debug.Log($"Player {Id} Blocking");
            blocking = true;
            canBlock = false;

            // Wait the blocking duration and then disable it
            yield return new WaitForSeconds(BlockDuration);
            Debug.Log($"Player {Id} Stopped Blocking");
            blocking = false;

            // Wait the cooldown before block can be used again
            yield return new WaitForSeconds(BlockCooldown);
            canBlock = true;

            Debug.Log($"Player {Id} Can Block Again");
        }

        private void ProcessAttack()
        {
            // Attack
            if (shootTimer <= 0)
            {
                if (controller.GetAxis("RangedAttack") > 0 || DebugToggleShooting)
                {
                    anim.SetTrigger("HasThrown");
                    var shootVectors = new List<Vector3>();
                    var cooldown = 0f;
                    switch (fireMode)
                    {
                        case ProjectileType.Normal:
                            var normalVector = new Vector3(lastLX, 0, lastLY);
                            normalVector.Normalize();
                            normalVector *= ProjectileSpeed;
                            shootVectors.Add(normalVector);
                            cooldown = 0.25f;
                            break;
                        case ProjectileType.Fanout:
                            var mid = new Vector3(lastLX, 0, lastLY);
                            mid.Normalize();
                            mid *= ProjectileSpeed;
                            var left = new Vector3(lastLX, 0, lastLY);
                            left.Normalize();
                            left *= ProjectileSpeed;
                            left = Quaternion.Euler(0, -15, 0) * left;
                            var right = new Vector3(lastLX, 0, lastLY);
                            right.Normalize();
                            right *= ProjectileSpeed;
                            right = Quaternion.Euler(0, 15, 0) * right;
                            shootVectors.Add(mid);
                            shootVectors.Add(left);
                            shootVectors.Add(right);
                            cooldown = 0.5f;
                            break;
                        case ProjectileType.Rapid:
                            var rapidVector = new Vector3(lastLX, 0, lastLY);
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
                        shootTimer = cooldown;
                    }
                }
            }
            else
            {
                shootTimer -= Time.deltaTime;
            }
        }
        #endregion

    }
}