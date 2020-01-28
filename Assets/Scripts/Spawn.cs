using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SomethingSpecific.ProtoNinja
{
    public class Spawn : MonoBehaviour
    {
        void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "SpawnIcon.png", true);
        }
    }
}