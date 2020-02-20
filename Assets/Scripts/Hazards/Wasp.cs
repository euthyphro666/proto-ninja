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
            Debug.DrawRay(transform.position, transform.forward * Movespeed, Color.magenta, Time.deltaTime);
            transform.Translate(transform.forward * Movespeed * Time.deltaTime, Space.World);


            if (Vector3.Distance(transform.position, transform.position) <= MaxDist)
            {

            }
        }


    }
}
