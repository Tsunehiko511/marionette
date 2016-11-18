using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Timer : MonoBehaviour {
	private int viewTime;
	Text timer;
	// Use this for initialization
	void Start () {
		timer = GetComponent<Text>();
		viewTime = 120;
	}


	public bool CountDown(){
		viewTime--;
		timer.text = "" + viewTime;
		if(viewTime <= 0){
			GameObject.Find("GameOver").GetComponent<gameOverScript>().Lose("TimeUp");
			return true;
		}
		return false;
	}	
}
