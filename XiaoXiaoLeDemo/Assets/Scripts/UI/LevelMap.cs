using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMap : MonoBehaviour {

    public static LevelMap main;
    public float mapSize = 1024f;

    Camera mapCamera;
    public float spawnOffset = 0.1f;
    Transform content;
    public float friction = 10;
    public MapLocation[] locations;
    Dictionary<int, MapLocation> locationsList = new Dictionary<int, MapLocation>();
    [HideInInspector]
    public int[] locationLevelNumber;

    Vector2 inertion = new Vector2();
    bool drag = false;
    Vector2[] lastPosition = new Vector2[2];
    Vector3 clickInfo = new Vector3();

    private void Awake()
    {
        main = this;
        if (content == null)
        {
            content = new GameObject("Map").transform;
            content.gameObject.layer = LayerMask.NameToLayer("Map");
        }
        if (mapCamera == null)
        {
            mapCamera = new GameObject("MapCamera").AddComponent<Camera>();
            mapCamera.orthographic = true;
            mapCamera.clearFlags = CameraClearFlags.SolidColor;
            mapCamera.backgroundColor = Color.black;
            mapCamera.cullingMask = 1 << LayerMask.NameToLayer("Map");
            mapCamera.transform.position = new Vector3(0, 0, -10);
        }
        UIAssistant.onScreenResize += UpdateMapParameters;
        UpdateMapParameters();
        mapCamera.orthographicSize = camSizeMax;
        spawnOffset *= Mathf.Max(Screen.width, Screen.height);

        locationLevelNumber = new int[locations.Length];
        locationLevelNumber[0] = 0;

        for (int i = 1; i < locations.Length; i++)
        {
            locationLevelNumber[i] = locationLevelNumber[i - 1] + locations[i - 1].GetLevelCount();
        }

        UIAssistant.onShowPage += (string page) => { if (page == "LevelList") UpdateMap(); };
    }
    void UpdateMap()
    {
        foreach (LevelButton button in FindObjectsOfType<LevelButton>())
            button.Initialize();
    }
    public float camSizeMin = 0;
    public float camSizeMax = 0;
    public void UpdateMapParameters()
    {
        camSizeMax = 0.5f * (mapSize * Screen.height) / (Screen.width * 100f);
        camSizeMin = Mathf.Min(3.5f, camSizeMax);
        mapCamera.orthographicSize = camSizeMax;
    }



    void OnEnable()
    {
        for (int n = 0; n < locations.Length; n++)
            locations[n].number = n;
        main = this;
        content.gameObject.SetActive(true);
        mapCamera.gameObject.SetActive(true);
        UpdateMapParameters();
        if (content.childCount == 0)
        {
            int target_level = 1;
            if (LevelProfile.main != null)
                target_level = LevelProfile.main.level;
            else
                target_level = ProfileAssistant.main.local_profile.current_level;

            int location = 0;
            for (int i = 0; i < locationLevelNumber.Length; i++)
            {
                if (target_level < locationLevelNumber[i])
                    break;
                location = i;
            }

            Transform rect = Transform.Instantiate(locations[location].transform);
            rect.name = locations[0].name;
            rect.parent = content;
            rect.localPosition = Vector3.zero;
            rect.localScale = Vector3.one;

            MapLocation map_location = rect.gameObject.GetComponent<MapLocation>();
            map_location.number = location;
            Vector3 position = mapCamera.transform.position;
            position.y = (map_location.nextLocationConnector.position.y + map_location.previousLocationConnector.position.y) / 2;
            mapCamera.transform.position = position;
        }
    }
    public void AddLocation(MapLocation mapLocation)
    {
        locationsList.Add(mapLocation.number, mapLocation);
    }
    public void RemoveLocation(MapLocation mapLocation)
    {
        locationsList.Remove(mapLocation.number);
    }

    public int IsVisible(Transform o)
    {
        float y = mapCamera.WorldToScreenPoint(o.position).y;
        if (y < -spawnOffset)
            return 1;
        if (y >= Screen.height + spawnOffset)
            return -1;
        return 0;
    }
    public MapLocation ShowNextLocation(MapLocation mapLocation)
    {
        int index = mapLocation.number + 1;

        if (index < 0 || index >= locations.Length)
            return null;

        MapLocation nextLocation;
        if (locationsList.ContainsKey(index))
            nextLocation = locationsList[index];
        else
        {
            nextLocation = locations[index];
            Transform t = Transform.Instantiate(nextLocation.transform);
            t.name = nextLocation.name;

            t.parent = content;
            t.position = mapLocation.nextLocationConnector.position;
            t.localScale = Vector3.one;
            nextLocation = t.GetComponent<MapLocation>();
            nextLocation.number = index;
        }

        return nextLocation;
    }
    public MapLocation ShowPreviuosLocation(MapLocation mapLocation)
    {
        int index = mapLocation.number - 1;

        if (index < 0 || index >= locations.Length)
            return null;

        MapLocation previuosLocation;
        if (locationsList.ContainsKey(index))
            previuosLocation = locationsList[index];
        else
        {
            previuosLocation = locations[index];
            Transform t = Transform.Instantiate(previuosLocation.transform);
            t.name = previuosLocation.name;

            t.parent = content;
            t.position = mapLocation.previousLocationConnector.position;
            previuosLocation = t.GetComponent<MapLocation>();
            previuosLocation.number = index;
            t.localScale = Vector3.one;
            t.position -= previuosLocation.nextLocationConnector.position - t.position;
        }

        return previuosLocation;
    }
}
