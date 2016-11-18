using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;


public class FromeMenuSceneScript : MonoBehaviour {

	public void ToBattle(int level) {
		/*
			GameObject.Find("Menu/Panels/ArenaPanel/Alert").gameObject.GetComponent<Text>().enabled = true;
			Debug.Log("ここだよーーー!");
			return;
		*/

		if(!GameConfiguration.IsSetAI){
			Text tmp = GameObject.Find("Menu/Panels/StoryPanel/Alert").gameObject.GetComponent<Text>();
			tmp.text = "AI構築を完了さしてください";
			tmp.enabled = true;
			return;
		}
		PlayerPrefs.Save();
		SceneManager.LoadScene("Battle");
	}

	public void ToFSM() {
		GameConfiguration.IsSetAI = true;
		SceneManager.LoadScene("FSM_Edit");
	}

	public void checkSet(){
		GameObject.Find("Menu/Panels/ArenaPanel/Alert").gameObject.GetComponent<Text>().enabled = false;
	}
}