using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    private Text Title;
    private RectTransform Health;
    private RectTransform Dodge;
    private RectTransform Block;

    private void Awake()
    {
        Title = GetComponentInChildren<Text>();
        var bars = GetComponentsInChildren<RawImage>();
        foreach (var bar in bars)
        {
            if (bar.name == "HealthBar")
            {
                Health = bar.GetComponent<RectTransform>();
            }
            else if (bar.name == "DodgeCooldown")
            {
                Dodge = bar.GetComponent<RectTransform>();
            }
            else if (bar.name == "BlockCooldown")
            {
                Block = bar.GetComponent<RectTransform>();
            }
        }
    }

    public void SetTitle(string title)
    {
        Title.text = title;
    }

    public void SetHealthPercent(int percent)
    {
        Health.SetRight(100 - percent);
    }

    public void SetDodgePercent(int percent)
    {
        Dodge.SetRight(100 - percent);
    }

    public void SetBlockPercent(int percent)
    {
        Block.SetRight(100 - percent);
    }
}
