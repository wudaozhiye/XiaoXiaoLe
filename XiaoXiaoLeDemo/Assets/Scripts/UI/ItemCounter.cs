using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ItemCounter : MonoBehaviour {

    Text label;
    public string itemID; // Item ID
    public static System.Action refresh = delegate { };

    void Awake()
    {
        label = GetComponent<Text>();
        refresh += Refresh;
    }
    void OnEnable()
    {
        Refresh(); // Updating when counter is activated
    }
    public void Refresh()
    {
        if (!label)
            return;
        if (ProfileAssistant.main.local_profile != null)
            label.text = ProfileAssistant.main.local_profile[itemID].ToString();
        else
            label.text = "0";
    }
    // Refreshing all counters function
    public static void RefreshAll()
    {
        refresh.Invoke();
    }
}
