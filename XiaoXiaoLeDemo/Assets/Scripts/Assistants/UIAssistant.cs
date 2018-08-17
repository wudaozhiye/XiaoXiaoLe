using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class UIAssistant : MonoBehaviour {

    public static UIAssistant main;
    public List<Transform> UImodules = new List<Transform>();

    public delegate void Action();
    public static Action onScreenResize = delegate { };
    public static Action<string> onShowPage = delegate { };
    Vector2 screenSize;

    public List<CPanel> panels = new List<CPanel>(); // Dictionary panels. It is formed automatically from the child objects
    public List<Page> pages = new List<Page>(); // Dictionary pages. It is based on an array of "pages"

    List<string> history = new List<string>();

    void Start()
    {
        ArraysConvertation(); // filling dictionaries
        Page defaultPage = GetDefaultPage();
        if (defaultPage != null)
            ShowPage(defaultPage, true); // Showing of starting page
    }
    void Awake()
    {
        main = this;
        screenSize = new Vector2(Screen.width, Screen.height);
    }
    void Update()
    {
        if (screenSize != new Vector2(Screen.width, Screen.height))
        {
            screenSize = new Vector2(Screen.width, Screen.height);
            onScreenResize.Invoke();
        }
    }
    // filling dictionaries
    public void ArraysConvertation()
    {
        // filling panels dictionary of the child objects of type "CPanel"
        panels = new List<CPanel>();
        panels.AddRange(GetComponentsInChildren<CPanel>(true));
        foreach (Transform module in UImodules)
            panels.AddRange(module.GetComponentsInChildren<CPanel>(true));
        if (Application.isEditor)
            panels.Sort((CPanel a, CPanel b) => {
                return string.Compare(a.name, b.name);
            });
    }
    public void ShowPage(Page page, bool immediate = false)
    {
        if (CPanel.uiAnimation > 0)
            return;

        if (history.Count > 0 && history.Last() == page.name)
            return;

        if (pages == null)
            return;

        history.Add(page.name);
        if (history.Count > 100)
            history.RemoveAt(0);

        foreach (CPanel panel in panels)
        {
            if (page.panels.Contains(panel))
                panel.SetVisible(true, immediate);
            else
                if (!page.ignoring_panels.Contains(panel) && !panel.freez)
                panel.SetVisible(false, immediate);
        }

        if (page.soundtrack != "-")
        {
            if (page.soundtrack != AudioAssistant.main.currentTrack)
                AudioAssistant.main.PlayMusic(page.soundtrack);
        }

        if (page.setTimeScale)
            Time.timeScale = page.timeScale;

        onShowPage(page.name);
    }
    public void ShowPage(string page_name)
    {
        ShowPage(page_name, false);
    }
    public void ShowPage(string page_name, bool immediate)
    {
        Page page = pages.Find(x => x.name == page_name);
        if (page != null)
            ShowPage(page, immediate);
    }
    public void FreezPanel(string panel_name, bool value = true)
    {
        CPanel panel = panels.Find(x => x.name == panel_name);
        if (panel != null)
            panel.freez = value;
    }
    public void SetPanelVisible(string panel_name, bool visible, bool immediate = false)
    {
        CPanel panel = panels.Find(x => x.name == panel_name);
        if (panel)
        {
            if (immediate)
                panel.SetVisible(visible, true);
            else
                panel.SetVisible(visible);
        }
    }
    // hide all panels
    public void HideAll()
    {
        foreach (CPanel panel in panels)
            panel.SetVisible(false);
    }
    public void ShowPreviousPage()
    {
        if (history.Count < 2)
            return;
        ShowPage(history[history.Count - 2]);
        history.RemoveAt(history.Count - 1);
        //之前已经加入到history了   删除再次ShowPage 加入的
        history.RemoveAt(history.Count - 1);
    }
    public string GetCurrentPage()
    {
        return history.Last();
    }
    public Page GetDefaultPage()
    {
        return pages.Find(x => x.default_page);
    }
    [System.Serializable]
    public class Page
    {
        public string name; // page name
        public List<CPanel> panels = new List<CPanel>(); // a list of names of panels in this page
        public List<CPanel> ignoring_panels = new List<CPanel>(); // a list of names of panels in this page
        public string soundtrack = "-";
        public bool default_page = false;
        public bool setTimeScale = true;
        public float timeScale = 1;
    }
}

