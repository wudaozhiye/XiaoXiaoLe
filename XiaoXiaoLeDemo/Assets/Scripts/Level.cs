using Berry.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {

    public static Dictionary<int, LevelProfile> all = new Dictionary<int, LevelProfile>();
    public LevelProfile profile;

    void Awake()
    {
        profile.level = transform.GetSiblingIndex() + 1;

        if (!all.ContainsKey(profile.level))
            all.Add(profile.level, profile);

        if (!Application.isEditor)
            Destroy(gameObject);
    }
    public static void LoadLevel(int key)
    {
        if (CPanel.uiAnimation > 0)
            return;

        if (!all.ContainsKey(key))
            return;

        LevelProfile.main = all[key];

        if (ProfileAssistant.main.local_profile["life"] > 0)
            UIAssistant.main.ShowPage("LevelSelectedPopup");
        else
            UIAssistant.main.ShowPage("NotEnoughLifes");
    }
#if UNITY_EDITOR
    public static void TestLevel(int l)
    {
        LevelProfile.main = all[l];
        FieldAssistant.main.StartLevel();
        PlayerPrefs.DeleteKey("TestLevel");
    }
#endif
}
[System.Serializable]
public class LevelProfile
{
    public static LevelProfile main; // current level

    public const int maxSize = 12; // maximal playing field size

    public int levelID = 0; // Level ID
    public int level = 0; // Level number

    // field size
    public int width = 9;
    public int height = 9;
    public int colorCount = 6; // count of chip colors
    public int targetColorCount = 30; // Count of target color in Color mode
    public int targetSugarDropsCount = 0; // Count of sugar chips in SugaDrop mode
    public int firstStarScore = 100; // number of score points needed to get a first stars
    public int secondStarScore = 200; // number of score points needed to get a second stars
    public int thirdStarScore = 300; // number of score points needed to get a third stars
    public float stonePortion = 0f;

    public FieldTarget target = FieldTarget.None; // Playing rules

    public Limitation limitation = Limitation.Moves;
    //步数
    public int limit = 30; // Count of moves in TargetScore and JellyCrush

    public float ai_difficult = 0.7f;

    public int[] countOfEachTargetCount = new int[6]; // Array of counts of each color matches. Color ID is index.

    public List<SlotSettings> slots = new List<SlotSettings>();
    public List<Int2> wall_vertical = new List<Int2>();
    public List<Int2> wall_horizontal = new List<Int2>();

    public void SetTargetCount(int index, int target)
    {
        countOfEachTargetCount[index] = target;
    }
    public int GetTargetCount(int index)
    {
        return countOfEachTargetCount[index];
    }
    public LevelProfile GetClone()
    {
        LevelProfile clone = new LevelProfile();
        clone = (LevelProfile)MemberwiseClone();
        clone.levelID = -1;
        return clone;
    }
}
[System.Serializable]
public class SlotSettings
{
    public Int2 position = new Int2();
    public Side gravity = Side.Bottom;

    public bool generator = false;
    public Int2 teleport = Int2.Null;
    public string chip = "";
    public int color_id = 0;
    public int jelly_level = 0;
    public string jam = "";
    public string block_type = "";
    public int block_level = 0;

    public List<string> tags = new List<string>();

    public SlotSettings(int _x, int _y)
    {
        position = new Int2(_x, _y);
    }
    public SlotSettings(Int2 _position)
    {
        position = _position;
    }
    public SlotSettings GetClone()
    {
        return MemberwiseClone() as SlotSettings;
    }
}