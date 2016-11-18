using UnityEngine;
using System.Collections;

public class PannelPosition : MonoBehaviour {
	float top;
	public float bottom;

	RectTransform textRect;
	// Use this for initialization
	void Start () {
		top = - Screen.currentResolution.height;
		bottom = - 225f;
		textRect = GetComponent<RectTransform> ();
		textRect.offsetMin = new Vector2(textRect.offsetMin.x, bottom);
		textRect.offsetMax = new Vector2(textRect.offsetMax.x, top);
	}
	
}
