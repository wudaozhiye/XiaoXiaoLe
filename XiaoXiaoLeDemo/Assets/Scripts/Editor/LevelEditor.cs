using Berry.Utils;
using EditorUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Level))]
public class LevelEditor : MetaEditor
{
    LevelProfile profile;
    Level level;
    Rect rect;

    static int cellSize = 40;
    static int legendSize = 20;
    static int slotOffset = 4;

    static Color defaultColor;
    Color[] chipColor;

    static GUIStyle mLabelStyle;

    public static GUIStyle labelStyle
    {
        get
        {
            if (mLabelStyle == null)
            {
                mLabelStyle = new GUIStyle(GUI.skin.button);
                mLabelStyle.wordWrap = true;
                mLabelStyle.normal.background = null;
                mLabelStyle.focused.background = null;
                mLabelStyle.active.background = null;

                mLabelStyle.normal.textColor = Color.black;
                mLabelStyle.focused.textColor = mLabelStyle.normal.textColor;
                mLabelStyle.active.textColor = mLabelStyle.normal.textColor;

                mLabelStyle.fontSize = 8;
                mLabelStyle.margin = new RectOffset();
                mLabelStyle.padding = new RectOffset();
            }
            return mLabelStyle;
        }
    }

    Dictionary<Int2, SlotSettings> slots = new Dictionary<Int2, SlotSettings>();
    List<Int2> teleportTargets = new List<Int2>();
    SlotSettings target_selection;
    bool wait_target = false;

    Dictionary<string, SessionAssistant.ChipInfo> chipInfos = new Dictionary<string, SessionAssistant.ChipInfo>();
    Dictionary<string, SessionAssistant.BlockInfo> blockInfos = new Dictionary<string, SessionAssistant.BlockInfo>();
    public static Dictionary<string, bool> layers = new Dictionary<string, bool>();

    string[] difficults = new string[] {
        "Easy",
        "Normal",
        "Hard"
    };

    List<Int2> selected = new List<Int2>();
    #region Icons
    public static Texture slotIcon;
    public static Texture chipIcon;
    public static Texture jellyIcon;
    public static Texture jamAIcon;
    public static Texture jamBIcon;
    public static Texture blockIcon;
    public static Texture generatorIcon;
    public static Texture teleportIcon;
    public static Texture sugarIcon;
    public static Texture wallhIcon;
    public static Texture wallvIcon;
    public static Dictionary<Side, Texture> gravityIcon = new Dictionary<Side, Texture>();

    static string[] alphabet = { "A", "B", "C", "D", "E", "F" };

    Texture LoadIcon(string resource)
    {
        return EditorGUIUtility.Load(resource) as Texture;
    }
    #endregion
    void OnEnable()
    {
        if (!metaTarget)
            return;
        level = (Level)metaTarget;

        if (SessionAssistant.main == null)
            SessionAssistant.main = FindObjectOfType<SessionAssistant>();

        Level[] levels = FindObjectsOfType<Level>();
        Level.all.Clear();

        foreach (Level l in levels)
        {
            l.profile.level = l.transform.GetSiblingIndex() + 1;
            if (!Level.all.ContainsKey(l.profile.level))
                Level.all.Add(l.profile.level, l.profile);
        }

        if (slotIcon == null) slotIcon = LoadIcon("LevelEditor/SlotIcon.png");
        if (chipIcon == null) chipIcon = LoadIcon("LevelEditor/ChipIcon.png");
        if (jellyIcon == null) jellyIcon = LoadIcon("LevelEditor/JellyIcon.png");
        if (jamAIcon == null) jamAIcon = LoadIcon("LevelEditor/JamAIcon.png");
        if (jamBIcon == null) jamBIcon = LoadIcon("LevelEditor/JamBIcon.png");
        if (blockIcon == null) blockIcon = LoadIcon("LevelEditor/BlockIcon.png");
        if (generatorIcon == null) generatorIcon = LoadIcon("LevelEditor/GeneratorIcon.png");
        if (teleportIcon == null) teleportIcon = LoadIcon("LevelEditor/TeleportIcon.png");
        if (sugarIcon == null) sugarIcon = LoadIcon("LevelEditor/SugarIcon.png");
        if (wallhIcon == null) wallhIcon = LoadIcon("LevelEditor/WallHIcon.png");
        if (wallvIcon == null) wallvIcon = LoadIcon("LevelEditor/WallVIcon.png");

        if (gravityIcon.Count == 0)
        {
            foreach (Side side in Utils.straightSides)
                gravityIcon.Add(side, LoadIcon("LevelEditor/GravityIcon" + side.ToString() + ".png"));
            gravityIcon.Add(Side.Null, LoadIcon("LevelEditor/GravityIcon" + Side.Null.ToString() + ".png"));
        }
        if (layers.Count == 0)
        {
            layers.Add("Chips", true);
            layers.Add("Jellies & Jam", true);
            layers.Add("Blocks", true);
            layers.Add("Generators", true);
            layers.Add("Teleports", true);
            layers.Add("Gravity", true);
            layers.Add("Walls", true);
        }

        slots.Clear();

        chipColor = Chip.colors.Select(x => Color.Lerp(x, Color.white, 0.4f)).ToArray();
    }

    public override void OnInspectorGUI()
    {
        if (!metaTarget)
            return;
        level = (Level)metaTarget;

        if (!level)
        {
            EditorGUILayout.HelpBox("No level selected", MessageType.Info);
            return;
        }
        if (level.profile == null)
            level.profile = new LevelProfile();

        profile = level.profile;

        #region Temporary arrays
        slots = profile.slots.ToDictionary(x => x.position, x => x);
        chipInfos = SessionAssistant.main.chipInfos.ToDictionary(x => x.name, x => x);
        blockInfos = SessionAssistant.main.blockInfos.ToDictionary(x => x.name, x => x);
        teleportTargets.Clear();

        foreach (Int2 coord in selected)
            if (slots.ContainsKey(coord) && !teleportTargets.Contains(slots[coord].teleport))
                teleportTargets.Add(slots[coord].teleport);

        #endregion

        if (profile.levelID == 0)
        {
            profile = new LevelProfile();
            profile.levelID = level.gameObject.GetInstanceID();
            ResetField();
        }
        if (profile.levelID != level.gameObject.GetInstanceID())
        {
            if (profile.levelID != level.gameObject.GetInstanceID())
                profile = profile.GetClone();
            profile.levelID = level.gameObject.GetInstanceID();
        }

        Undo.RecordObject(level, "Level design changed");

        #region Level parameters
        GUILayout.Label("Level Parameters", EditorStyles.centeredGreyMiniLabel, GUILayout.ExpandWidth(true));
        profile.level = level.transform.GetSiblingIndex() + 1;

        EditorGUILayout.BeginVertical(EditorStyles.textArea);

        #region Navigation Panel

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("<<", EditorStyles.miniButtonLeft, GUILayout.Width(30)))
        {
            SelectLevel(1);
            return;
        }
        if (GUILayout.Button("<", EditorStyles.miniButtonMid, GUILayout.Width(30)))
        {
            SelectLevel(profile.level - 1);
            return;
        }
        GUILayout.Label("Level #" + profile.level, EditorStyles.miniButtonMid, GUILayout.Width(70));

        if (GUILayout.Button(">", EditorStyles.miniButtonMid, GUILayout.Width(30)))
        {
            SelectLevel(profile.level + 1);
            return;
        }
        if (GUILayout.Button(">>", EditorStyles.miniButtonRight, GUILayout.Width(30)))
        {
            SelectLevel(Level.all.Count);
            return;
        }
        EditorGUILayout.EndHorizontal();
        #endregion
        profile.width = Mathf.RoundToInt(EditorGUILayout.Slider("Width", 1f * profile.width, 5f, LevelProfile.maxSize));
        profile.height = Mathf.RoundToInt(EditorGUILayout.Slider("Height", 1f * profile.height, 5f, LevelProfile.maxSize));
        profile.colorCount = Mathf.RoundToInt(EditorGUILayout.Slider("Count of Possible Colors", 1f * profile.colorCount, 3f, chipColor.Length));
        profile.stonePortion = Mathf.Round(EditorGUILayout.Slider("Stone Portion", profile.stonePortion, 0f, 0.7f) * 100) / 100;

        #region Stars
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Score Stars", GUILayout.ExpandWidth(true));
        profile.firstStarScore = Mathf.Max(EditorGUILayout.IntField(profile.firstStarScore, GUILayout.ExpandWidth(true)), 1);
        profile.secondStarScore = Mathf.Max(EditorGUILayout.IntField(profile.secondStarScore, GUILayout.ExpandWidth(true)), profile.firstStarScore + 1);
        profile.thirdStarScore = Mathf.Max(EditorGUILayout.IntField(profile.thirdStarScore, GUILayout.ExpandWidth(true)), profile.secondStarScore + 1);
        EditorGUILayout.EndHorizontal();
        #endregion

        #region Limitation

        Enum limitation = EditorGUILayout.EnumPopup("Limitation", profile.limitation);
        if (profile.limitation != (Limitation)limitation)
            profile.limit = 30;
        profile.limitation = (Limitation)limitation;
        switch (profile.limitation)
        {
            case Limitation.Moves:
                profile.limit = Mathf.RoundToInt(EditorGUILayout.Slider("Move Count", profile.limit, 5, 100));
                break;
            case Limitation.Time:
                profile.limit = Mathf.RoundToInt(EditorGUILayout.Slider("Session duration (" + Utils.ToTimerFormat(profile.limit) + ")", Mathf.Ceil(profile.limit / 5) * 5, 5, 300));
                break;
        }
        #endregion

        #region Target
        profile.target = (FieldTarget)EditorGUILayout.EnumPopup("Target", profile.target);
        if (profile.target == FieldTarget.Color)
        {
            defaultColor = GUI.color;
            profile.targetColorCount = Mathf.RoundToInt(EditorGUILayout.Slider("Targets Count", profile.targetColorCount, 1, profile.colorCount));
            for (int i = 0; i < chipColor.Length; i++)
            {
                GUI.color = chipColor[i];
                if (i < profile.targetColorCount)
                    profile.SetTargetCount(i, Mathf.Clamp(EditorGUILayout.IntField("Color " + alphabet[i].ToString(), profile.GetTargetCount(i)), 1, 999));
                else
                    profile.SetTargetCount(i, 0);
            }
            GUI.color = defaultColor;
        }
        if (profile.target == FieldTarget.SugarDrop)
        {
            profile.targetSugarDropsCount = Mathf.RoundToInt(EditorGUILayout.Slider("Sugar Count", profile.targetSugarDropsCount, 1, 20));
        }
        if (profile.target == FieldTarget.Duel)
        {
            profile.ai_difficult = EditorGUILayout.Slider("AI Difficult ("
                + difficults[Mathf.RoundToInt(profile.ai_difficult * (difficults.Length - 1))]
                + ")", Mathf.Round(profile.ai_difficult * 10) / 10, 0f, 1f);
        }

        #endregion
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
        #endregion

        UpdateName(level);



    }
    public void SelectLevel(int v)
    {
        Level _level = FindObjectsOfType<Level>().ToList().Find(x => x.profile.level == v);
        if (_level)
        {
            level = _level;
            BerryPanel.lastSelectedLevel = level.transform.GetSiblingIndex();
            BerryPanel.currentLevel = level;
        }
    }
    void ResetField()
    {
        slots.Clear();
        profile.wall_horizontal.Clear();
        profile.wall_vertical.Clear();
        for (int x = 0; x < profile.width; x++)
            for (int y = 0; y < profile.height; y++)
                NewSlotSettings(new Int2(x, y));
    }
    SlotSettings NewSlotSettings(Int2 coord)
    {
        if (!slots.ContainsKey(coord))
        {
            slots.Add(coord, new SlotSettings(coord));
            if (coord.y == profile.height - 1)
                slots[coord].generator = true;
            if (coord.y == 0)
                slots[coord].tags.Add("SugarDrop");
            return slots[coord];
        }
        return null;
    }
    public override UnityEngine.Object FindTarget()
    {
        return BerryPanel.currentLevel;
    }

    public static void UpdateName(Level level)
    {
        level.name = "Level:" + level.profile.level.ToString() + "," + level.profile.target + "," + level.profile.limitation;
    }
}
