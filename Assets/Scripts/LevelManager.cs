using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;
using System.Linq;

namespace SomethingSpecific.ProtoNinja
{
    public class LevelManager : MonoBehaviour
    {

        public GameObject PlayerPrefab;
        public int PlayerCount = 1;
        private bool ActiveGame = false;
        private IList<Rewired.Player> Controllers;
        private Text Status;


        void Start()
        {
            Controllers = Rewired.ReInput.players.AllPlayers;
            Status = GameObject.Find("StatusMessage").GetComponent<Text>();
        }


        void BeginLevel()
        {
            ActiveGame = true;
            Status.text = "";
            SpawnPlayers();
        }

        void SpawnPlayers()
        {
            var playersParent = GameObject.Find("Players");
            var spawns = GameObject.FindGameObjectsWithTag("Spawn");
            var spawnIndices = spawns.GetUniqueRandomIndexArray(PlayerCount);
            for (int i = 0; i < PlayerCount; i++)
            {
                var pos = spawns[spawnIndices[i]].transform.position;
                var playerObject = GameObject.Instantiate(PlayerPrefab, pos, Quaternion.identity, playersParent.transform);
                var player = playerObject.GetComponent<Player>();
                player.Id = i;
            }
        }

        void EndLevel(Player winner)
        {
            ActiveGame = false;
            Status.text = $"Player {winner.Id} Won!\nPress 'Start' to Begin Again!";
            Destroy(winner.gameObject);
        }

        void Update()
        {
            if (!ActiveGame)
            {
                if (Controllers.Any(c => c.GetButtonSinglePressDown("Start")))
                {
                    Debug.Log("Got start.");
                    BeginLevel();
                }
            }
            else
            {
                var players = GameObject.FindGameObjectsWithTag("Player");
                if (players.Length <= 1)
                {
                    EndLevel(players[0].GetComponent<Player>());
                }
            }
        }
    }
}
