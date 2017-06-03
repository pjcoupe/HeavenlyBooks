using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltCameraMove : MonoBehaviour {

    Vector3 initialCameraPosition;
	// Use this for initialization
    float xRange = 10f;
    float yRange = 10f;
	void Start () {
        initialCameraPosition = transform.localPosition;
        print("initial Cam:"+initialCameraPosition);
	}
	
	// Update is called once per frame
	void Update () {


        float mouseRatioX = -0.5f + Input.mousePosition.x / Screen.width;
        float mouseRatioY = -0.5f + Input.mousePosition.y / Screen.height;


        Vector3 newPos = new Vector3(mouseRatioX * xRange, mouseRatioY * yRange, 0);
        transform.localPosition = initialCameraPosition + newPos;
        /*
        float eulerY = mouseRatioX * 20f;
        eulerY = eulerY < 0 ? 360f + eulerY: eulerY;
        float eulerX = mouseRatioY * 10f;
        eulerX = eulerX < 0 ? 360f + eulerX: eulerX;
        
        transform.eulerAngles = new Vector3(eulerX, eulerY, 0);
        print("PJC REMOVE y="+transform.eulerAngles.y+" x="+transform.eulerAngles.x);
        */
	}
}
