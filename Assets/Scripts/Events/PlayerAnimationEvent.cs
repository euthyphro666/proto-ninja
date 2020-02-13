using System.Collections;
using System.Collections.Generic;
using SomethingSpecific.ProtoNinja;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    private Player playerController;
    
    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponentInParent<Player>();
    }

    /// <summary>
    /// Called through Animation Events.
    /// </summary>
    /// <remarks>See Unity docs: https://docs.unity3d.com/Manual/AnimationEventsOnImportedClips.html</remarks>
    /// <param name="animationName"></param>
    public void FinishedAnimation(string animationName)
    {
        Debug.Log($"Finished animation: {animationName}");
        if(animationName == "IsDead")
            playerController.DestroyPlayer();
    }
}
