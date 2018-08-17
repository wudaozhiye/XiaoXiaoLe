using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ProfileAssistant : MonoBehaviour {

    public static ProfileAssistant main;
    public UserProfile local_profile;
    void Awake()
    {
        main = this;
        UIAssistant.onShowPage += TryToSaveProfile;


    }
    void Start()
    {
        local_profile = UserProfileUtils.ReadProfileFromDevice();
        StartCoroutine(LifeSystemRoutine());

#if UNITY_EDITOR
        if (PlayerPrefs.GetInt("TestLevel") != 0)
        {
            Level.TestLevel(PlayerPrefs.GetInt("TestLevel"));
            return;
        }
#endif
    }

    void TryToSaveProfile(string page)
    {
        UserProfileUtils.WriteProfileOnDevice(local_profile);
    }
    IEnumerator LifeSystemRoutine()
    {
        while (local_profile == null)
            yield return 0;

        TimeSpan refilling_time = new TimeSpan(0, ProjectParameters.main.refilling_time, 0);

        while (true)
        {
            while (local_profile["life"] < ProjectParameters.main.lifes_limit && local_profile.next_life_time <= DateTime.Now)
            {
                local_profile["life"]++;
                local_profile.next_life_time += refilling_time;
                ItemCounter.RefreshAll();
            }
            if (local_profile["life"] >= ProjectParameters.main.lifes_limit)
                local_profile.next_life_time = DateTime.Now + refilling_time;
            yield return new WaitForSeconds(1);
        }
    }
}
public class UserProfile
{
    public System.DateTime lastSave = new DateTime();
    public string name = "";
    public int current_level = 1;
    public System.DateTime next_life_time = new DateTime();
    public System.DateTime daily_raward = new DateTime();

    public Dictionary<string, int> inventory = new Dictionary<string, int>();
    public Dictionary<int, int> score = new Dictionary<int, int>();

    public override string ToString()
    {
        string report = "";
        report += "Name: " + name + ", ";
        report += "Current level: " + current_level + ", ";
        report += "Score count: " + score.Count + ", ";
        report += "Last save: " + lastSave.ToString() + ", ";
        report += "Next life time: " + next_life_time.ToShortDateString() + ", ";
        report += "Daily reward: " + daily_raward.ToShortDateString();
        return report;
    }
    public int GetScore(int level_number)
    {
        if (!score.ContainsKey(level_number))
            return 0;
        return score[level_number];
    }
    public void SetScore(int level_number, int value)
    {
        if (!score.ContainsKey(level_number))
            score.Add(level_number, 0);
        score[level_number] = Mathf.Max(score[level_number], value);
    }

    public int this[string index]
    {
        get
        {
            if (inventory.ContainsKey(index))
                return inventory[index];
            return 0;
        }
        set
        {
            if (!inventory.ContainsKey(index))
                inventory.Add(index, 0);
            inventory[index] = Mathf.Max(0, value);
        }
    }
}
public class UserProfileUtils
{
    public static void WriteProfileOnDevice(UserProfile profile)
    {
        PlayerPrefs.SetString("Profile_name", profile.name);
        PlayerPrefs.SetInt("Profile_current_level", profile.current_level);

        profile.lastSave = System.DateTime.UtcNow;
        PlayerPrefs.SetString("Profile_last_save", profile.lastSave.ToBinary().ToString());
        PlayerPrefs.SetString("Profile_next_life_time", profile.next_life_time.ToBinary().ToString());
        PlayerPrefs.SetString("Profile_daily_raward", profile.daily_raward.ToBinary().ToString());

        string inventory = string.Join(";", profile.inventory.Select(
                p => string.Format(
                    "{0}:{1}",
                    p.Key,
                    p.Value.ToString()
                    )).ToArray<string>());
        PlayerPrefs.SetString("Profile_inventory", inventory);
        string score = string.Join(";", profile.score.Select(
                  p => string.Format(
                      "{0}:{1}",
                      p.Key,
                      p.Value.ToString()
                      )).ToArray<string>());
        PlayerPrefs.SetString("Profile_score", score);
        PlayerPrefs.Save();
    }
    public static UserProfile ReadProfileFromDevice()
    {
        UserProfile profile = new UserProfile();

        profile.name = PlayerPrefs.GetString("Profile_name");
        profile.current_level = PlayerPrefs.GetInt("Profile_current_level");
        if (profile.current_level == 0)
            profile.current_level = 1;

        string lastSave = PlayerPrefs.GetString("Profile_last_save");
        if (lastSave.Length > 0)
            profile.lastSave = System.DateTime.FromBinary(long.Parse(lastSave));

        string next_life_time = PlayerPrefs.GetString("Profile_next_life_time");
        if (next_life_time.Length > 0)
            profile.next_life_time = System.DateTime.FromBinary(long.Parse(next_life_time));

        string daily_raward = PlayerPrefs.GetString("Profile_daily_raward");
        if (daily_raward.Length > 0)
            profile.daily_raward = System.DateTime.FromBinary(long.Parse(daily_raward));
        else
            profile.daily_raward = new DateTime(1971, 1, 1, ProjectParameters.main.dailyreward_hour, 0, 0);

        string inventory = PlayerPrefs.GetString("Profile_inventory");
        if (inventory.Length > 0)
            profile.inventory = inventory
                 .Split(';')
                 .Select(s => s.Split(':'))
                 .ToDictionary(
                    p => p[0],
                    p => int.Parse(p[1])
                );

        string score = PlayerPrefs.GetString("Profile_score");
        if (score.Length > 0)
            profile.score = score
             .Split(';')
             .Select(s => s.Split(':'))
             .ToDictionary(
                p => int.Parse(p[0]),
                p => int.Parse(p[1])
            );
        return profile;
    }
}
