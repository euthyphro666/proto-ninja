using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SomethingSpecific.ProtoNinja.Events;

namespace SomethingSpecific.ProtoNinja
{
    public class LevelManager : MonoBehaviour
    {
        public GameObject PlayerPrefab;
        public int PlayerCount = 1;
        public int NewGameCountdownTime = 3;
        private bool ActiveGame = false;

        private IList<Rewired.Player> Controllers;
        private Text Status;
        private HudManager Hud;

        void Start()
        {
            Hud = FindObjectOfType<HudManager>();
            Controllers = Rewired.ReInput.players.AllPlayers;
            Status = GameObject.Find("StatusMessage").GetComponent<Text>();
            Status.text = "Press 'Start' to begin.";
        }

        void BeginLevel()
        {
            StartCoroutine(BeginCountdown(NewGameCountdownTime));
        }

        private IEnumerator BeginCountdown(int countdownStartTime)
        {
            for (var i = 0; i < countdownStartTime; i++)
            {
                Status.text = $"{Mathf.RoundToInt(countdownStartTime - i)}!";
                yield return new WaitForSeconds(1);
            }

            ActiveGame = true;
            Status.text = "";
            SpawnPlayers();
            Hud.InitPlayerInfo(PlayerCount);
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
                player.UpdateHealthEvent += OnUpdatePlayerHealth;
            }
        }

        void OnUpdatePlayerHealth(object sender, TypedEventArgs<float> args)
        {
            if (sender is Player player)
            {
                var percent = (int)(args.Value / player.MaxHealth * 100);
                Hud.GetPlayerInfo(player.Id).SetHealthPercent(percent);
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
