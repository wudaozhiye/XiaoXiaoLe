using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceAssistant : MonoBehaviour {

    public static ServiceAssistant main;

    bool rate_it_showed = false;
    bool daily_reward_showed = false;

    void Awake()
    {
        if (Application.isEditor)
            Application.runInBackground = true;

        main = this;
        UIAssistant.onShowPage += LevelMapPopup;

        rate_it_showed = PlayerPrefs.GetInt("Rated") == 1;
    }
    void LevelMapPopup(string page)
    {
        StartCoroutine(LevelMapPopupRoutine(page));

    }
    IEnumerator LevelMapPopupRoutine(string page)
    {
        if (page != "LevelList")
            yield break;

        yield return 0;

        while (CPanel.uiAnimation > 0)
            yield return 0;
        if (UIAssistant.main.GetCurrentPage() != page)
            yield break;

        yield return 0;

        // Daily Reward
        if (!daily_reward_showed && ProfileAssistant.main.local_profile.daily_raward < System.DateTime.Now)
        {
            daily_reward_showed = true;
            UIAssistant.main.ShowPage("SpinWheel");
            yield break;
        }

    }
}
