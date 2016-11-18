using UnityEngine;
using System.Collections;

public class InitScreenSize : MonoBehaviour {
	int width  = Screen.width;
	int height = Screen.height;
	// Use this for initialization
	void Start () {
		RectTransform textRect = GetComponent<RectTransform> ();
		textRect.sizeDelta = new Vector2 (width, height);	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
