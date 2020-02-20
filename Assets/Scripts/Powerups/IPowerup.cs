using System.Collections;
using System.Collections.Generic;
using SomethingSpecific.ProtoNinja;
using UnityEngine;

public interface IPowerup
{
    Player OwningPlayer { get; set; }
}
