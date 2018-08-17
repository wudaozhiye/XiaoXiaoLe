using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionAssistant : MonoBehaviour {

    public static SessionAssistant main;
    public int blockCountTotal;
    public int jellyCountTotal;
    public int[] jamCountTotal = new int[2];

    public bool squareCombination = true;
    public List<Combinations> combinations = new List<Combinations>();
    public List<ChipInfo> chipInfos = new List<ChipInfo>();

    public int lastMovementId;
    //移动步数
    public int movesCount; // Number of remaining moves
    public int swapEvent; // After each successed swap this parameter grows by 1 
    public int[] countOfEachTargetCount = { 0, 0, 0, 0, 0, 0 };// Array of counts of each color matches. Color ID is index.
    //剩余步数
    public float timeLeft; // Number of remaining time
    public int eventCount; // Event counter
    //分数
    public int score = 0; // Current score
    public int[] colorMask = new int[6]; // Mask of random colors: color number - colorID

    public int creatingSugarDropsCount;
    public bool isPlaying = false;
    public bool outOfLimit = false;
    public int creatingSugarTask = 0;
    public bool reachedTheTarget = false;
    public bool firstChipGeneration = false;
    public int matchCount = 0;

    public int stars;

    bool targetRoutineIsOver = false;
    bool limitationRoutineIsOver = false;

    void Awake()
    {
        main = this;
        combinations.Sort((Combinations a, Combinations b) => {
            if (a.priority < b.priority)
                return -1;
            if (a.priority > b.priority)
                return 1;
            return 0;
        });
    }
    public static void Reset()
    {
        main.stars = 0;

        main.eventCount = 0;
        main.matchCount = 0;
        main.lastMovementId = 0;
        main.swapEvent = 0;
        main.score = 0;
        main.firstChipGeneration = true;

        main.isPlaying = false;
        main.movesCount = LevelProfile.main.limit;
        main.timeLeft = LevelProfile.main.limit;
        main.countOfEachTargetCount = new int[] { 0, 0, 0, 0, 0, 0 };
        main.creatingSugarTask = 0;

        main.reachedTheTarget = false;
        main.outOfLimit = false;

        main.targetRoutineIsOver = false;
        main.limitationRoutineIsOver = false;

        main.iteraction = true;
    }
    #region Swapping
    // Temporary Variables
    bool swaping = false; // �TRUE when the animation plays swapping 2 chips
    public bool iteraction = false;

    #endregion

    [System.Serializable]
    public class ChipInfo
    {
        public string name = "";
        public string contentName = "";
        public bool color = true;
        public string shirtName = "";
    }

    [System.Serializable]
    public class Combinations
    {
        public int priority = 0;
        public string chip;
        public bool horizontal = true;
        public bool vertical = true;
        public bool square = false;
        public int minCount = 4;

    }
}
