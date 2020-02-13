using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Wasp : MonoBehaviour

{

    public Transform Player;
    public int Movespeed;
    public int MaxDist;
    public int MinDist;




    void Start()
    {
        
    }

    void Update()
    {
        transform.LookAt(Player);

        if (Vector3.Distance(transform.position, Player.position) >= MinDist)
        {
            transform.Translate(transform.forward * Movespeed * Time.deltaTime);


            if(Vector3.Distance(transform.position, transform.position) <= MaxDist)
            {

            }
        }

        
    }
}
