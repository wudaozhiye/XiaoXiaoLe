using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour {

    public static GameCamera main;

    public static Camera cam;
    public bool playing = false;
    void Awake()
    {
        main = this;
        cam = GetComponent<Camera>();
        UIAssistant.onScreenResize += OnScreenResize;
        OnScreenResize();
    }
    public void OnScreenResize()
    {
        if (!FieldArea.main)
            return;
        FieldArea.main.UpdateParameters();
        StopAllCoroutines();
        StartCoroutine(ResizingRoutine());
    }
    IEnumerator ResizingRoutine()
    {
        FieldArea.main.UpdateParameters();
        float targetSize = GetTargetSize();

        Vector3 targetPosition = new Vector3(-2f * FieldArea.position.x / FieldArea.screen_size.x, -2f * FieldArea.position.y / FieldArea.screen_size.y, -10);
        targetPosition.x *= targetSize * Screen.width / Screen.height;
        targetPosition.y *= targetSize;

        float speed = Vector3.Distance(targetPosition, transform.position) * 3;
        speed = Mathf.Max(speed, 3);

        while (playing && (targetPosition != transform.position || cam.orthographicSize != targetSize))
        {
            cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, targetSize, Time.unscaledDeltaTime * speed);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.unscaledDeltaTime * speed);
            yield return 0;
        }
    }
    float GetTargetSize()
    {
        float width = FieldAssistant.main.field.width * ProjectParameters.Instance.slot_offset * (FieldArea.screen_size.x / FieldArea.size.x) * 0.5f * Screen.height / Screen.width;
        float height = FieldAssistant.main.field.height * ProjectParameters.Instance.slot_offset * (FieldArea.screen_size.y / FieldArea.size.y) * 0.5f;

        return width > height ? width : height;
    }
}
