using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentFitter : MonoBehaviour
{
    public float buffer;
    public Camera cam => Camera.main;
    public EventTriggerSO AspectChanged;

    [Button(nameof(FitContent))]
    public bool buttonBool;

    [Button(nameof(CameraFitToView))]
    public bool buttonBool1;

    private void Awake()
    {
        FitContent();
    }

    static Vector3[] arr = new Vector3[]
    {
        new Vector3(0, 0),  //Center
        new Vector3(0, -1), //Center bottom
        new Vector3(1, 0),  //Center right
        new Vector3(0, 1),  //Center up
        new Vector3(-1, 0), //Center left
        new Vector3(-1, -1),//bottom left 
        new Vector3(1, -1), //top left
        new Vector3(1, 1),  //top right
        new Vector3(-1, 1)  //bottom right
    };

    public void FitContent()
    {
        var bounds = new Bounds();
        Vector3 bottomLeft = cam.ViewportToWorldPoint(Vector3.zero);
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(cam.rect.width, cam.rect.height));

        bounds.Encapsulate(bottomLeft);
        bounds.Encapsulate(topRight);

        bounds.Expand(((cam.aspect > 1) ? bounds.size.x : bounds.size.y) * -0.15f);
        Vector2 size = bounds.size * 0.5f;

        transform.GetChild(0).transform.eulerAngles = Vector3.forward * ((cam.aspect > 1) ? 0 : 90);
        transform.GetChild(0).GetChild(0).transform.localPosition = Vector3.right * ((cam.aspect > 1) ? size.x : size.y) * -0.3f;
        transform.GetChild(0).GetChild(1).transform.localPosition = Vector3.right * ((cam.aspect > 1) ? size.x : size.y) * 0.3f + Vector3.forward * 2;

        if (transform.childCount != arr.Length)
            Debug.Log("child count do not match no. of corners");

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).transform.position = size * arr[i];
        }
        AspectChanged.TriggerEvent();
    }

    public void CameraFitToView()
    {
        var (centre, size) = GetCamSize();
        cam.transform.position = centre;
        cam.orthographicSize = size;
    }

    private (Vector3 centre, float size) GetCamSize()
    {
        var bounds = new Bounds();
        foreach (Transform child in gameObject.transform)
            bounds.Encapsulate(child.position);

        bounds.Expand(buffer);

        var vertical = bounds.size.y;
        var horizontal = bounds.size.x * cam.pixelHeight/cam.pixelWidth;

        var size = Mathf.Max(horizontal, vertical) * 0.5f;
        var centre = bounds.center + new Vector3(0, 0, -10);

        return (centre, size);
    }
}
