using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudManager : MonoBehaviour
{
    public GameObject PlayerInfoPrefab;

    private PlayerInfo[] PlayerInfos;

    public void InitPlayerInfo(int playerCount)
    {
        if (PlayerInfos != null) Cleanup();

        PlayerInfos = new PlayerInfo[playerCount];
        for (int i = 0; i < playerCount; i++)
        {
            var obj = GameObject.Instantiate(PlayerInfoPrefab, transform);
            var rect = obj.GetComponent<RectTransform>();
            rect.SetX((10 * (i + 1)) + (100 * i));

            var info = obj.GetComponent<PlayerInfo>();
            info.SetTitle("Player " + (i + 1));
            PlayerInfos[i] = info;
        }
    }

    public PlayerInfo GetPlayerInfo(int id)
    {
        return PlayerInfos[id];
    }

    public void Cleanup()
    {
        //TODO
    }

}
