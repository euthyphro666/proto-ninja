using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudManager : MonoBehaviour
{
    public GameObject PlayerInfoPrefab;

    private UIPlayerInfo[] PlayerInfos;

    public void InitPlayerInfo(GameObject[] targets)
    {
        if (PlayerInfos != null) Cleanup();

        PlayerInfos = new UIPlayerInfo[targets.Length];
        for (int i = 0; i < targets.Length; i++)
        {
            var obj = GameObject.Instantiate(PlayerInfoPrefab, transform);
            var rect = obj.GetComponent<RectTransform>();
            rect.SetX((10 * (i + 1)) + (100 * i));

            var info = obj.GetComponent<UIPlayerInfo>();
            info.Init(i + 1, targets[i]);
            PlayerInfos[i] = info;
        }
    }

    public UIPlayerInfo GetPlayerInfo(int id)
    {
        return PlayerInfos[id];
    }

    public void Cleanup()
    {
        //TODO
    }

}
