﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SomethingSpecific.ProtoNinja
{
    public class BearTrap : MonoBehaviour, IPowerup
    {
        public Player OwningPlayer { get; set; }
        public float FreezeTime = 0.5f;
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(typeof(Player), out var comp) &&
                comp is Player player)
            {
                player.Freeze(FreezeTime);
                Destroy(gameObject);
            }
        }

    }
}
