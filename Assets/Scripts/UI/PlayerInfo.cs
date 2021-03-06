﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SomethingSpecific.ProtoNinja;

public class PlayerInfo : MonoBehaviour
{
    private Text Title;
    private GameObject Target;
    private RectTransform Fixed;
    private RectTransform Fluid;
    private Image[] Health;
    private Image Powerup;
    private RawImage Block;
    private RawImage Dash;

    private void Awake()
    {
        var fixedObj = transform.GetChild(0);
        if (fixedObj.name == "Fixed")
            Fixed = fixedObj.GetComponent<RectTransform>();
        var fluidObj = transform.GetChild(1);
        if (fluidObj.name == "Fluid")
            Fluid = fluidObj.GetComponent<RectTransform>();

        Title = GetComponentInChildren<Text>();
        var images = GetComponentsInChildren<RawImage>();
        foreach (var image in images)
        {
            if (image.name == "BlockBar") Block = image;
            if (image.name == "DashBar") Dash = image;
        }
        Health = GetComponentsInChildren<Image>()
            .Where(i => i.name.StartsWith("HP"))
            .OrderBy(i => int.Parse(i.name.Substring(2)))
            .ToArray();
        Powerup = GetComponentsInChildren<Image>(true)
            .FirstOrDefault(i => i.name == "Powerup");
    }

    public void Init(int id, GameObject target)
    {
        Target = target;
        Title.text = $"Player {id}";
        Fixed.SetX(50 + ((id - 1) * 225));
        Fixed.SetY(-25);
    }

    public void SetPowerup(Sprite icon)
    {
        Powerup.sprite = icon;
        Powerup.enabled = icon != null;
    }

    /// <summary>
    /// Must be between 0(inclusive) and 5(exlusive)
    /// </summary>
    /// <param name="health"></param>
    public void SetHealth(int health)
    {
        if (health >= 0 && health <= 5)
        {
            for (int i = 0; i < 5; i++)
            {
                Health[i].gameObject.SetActive(i < health);
            }
        }
    }

    public void SetBlock(int percent)
    {
        Block.rectTransform.SetRight(100 - percent);
    }

    public void SetDash(int percent)
    {
        Dash.rectTransform.SetRight(100 - percent);
    }

    private Vector2 LastPos;
    public void Update()
    {
        // var pos = WorldToCanvasPosition(Block.canvas, Fluid.parent.transform as RectTransform, Camera.main, Target.transform.position);
        // // var pos = Camera.main.WorldToScreenPoint(Target.transform.position);
        // Debug.Log($"({pos.x}, {pos.y})");
        // if (LastPos != pos)
        // {
        //     Fluid.SetX(pos.x);
        //     Fluid.SetY(pos.y);
        //     LastPos = pos;
        // }
    }

    private Vector2 WorldToCanvasPosition(Canvas canvas, RectTransform canvasRect, Camera camera, Vector3 position)
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(camera, position);
        Vector2 result;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : camera, out result);
        return canvas.transform.TransformPoint(result);
    }
}
