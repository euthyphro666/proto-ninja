using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    private Text Title;
    private RawImage Health;

    private void Awake()
    {
        Title = GetComponentInChildren<Text>();
        Health = GetComponentInChildren<RawImage>();
    }

    public void SetTitle(string title)
    {
        Title.text = title;
    }

    public void SetHealthPercent(int percent)
    {
        Health.rectTransform.SetRight(100 - percent);
    }
}
