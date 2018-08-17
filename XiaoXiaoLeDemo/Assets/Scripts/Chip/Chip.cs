using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chip : MonoBehaviour {

    public static List<Chip> busyList = new List<Chip>();
    public static List<Chip> gravityBlockers = new List<Chip>();

    public static readonly int universalColorId = 10;
    public static readonly int uncoloredId = -1;

    public Slot slot; // Slot which include this chip

    // Colors for each chip color ID
    public static readonly Color[] colors = {
        new Color(0.75f, 0.3f, 0.3f),  // 粉红
        new Color(0.3f, 0.75f, 0.3f),   //绿
        new Color(0.3f, 0.5f, 0.75f),   //浅蓝
        new Color(0.75f, 0.75f, 0.3f),  //淡黄
        new Color(0.75f, 0.3f, 0.75f),  //紫色
        new Color(0.75f, 0.5f, 0.3f),   //浅棕色
    };

    public static readonly string[] chipTypes = {
        "Red",
        "Green",
        "Blue",
        "Yellow",
        "Purple",
        "Orange"
    };
}
