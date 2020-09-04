using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    // scroll controls
    public float minFov = 5f;
    public float maxFov = 10f;
    public float sensitivity = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnGUI()
 {
     if(Event.current.type == EventType.ScrollWheel) {
         // it's weird, maybe only on linux, but both mouse and trackpad report two opposite signed values at once, e.g.: -0.5, 3 or 0.5, -3
         // TODO: this still behaves a big weird on trackpad - so need to look more into these values
        if(Math.Abs(Event.current.delta.y) < 1.0f) {
            Camera gameCamera = GetComponent<Camera>();
            float size = gameCamera.orthographicSize;
            size += Event.current.delta.y * sensitivity;
            size = Mathf.Clamp(size, minFov, maxFov);
            gameCamera.orthographicSize = size;
        }
     }

 }

    // Update is called once per frame
    void Update()
    {
    }
}
