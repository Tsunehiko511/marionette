using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

using NCMB;

public class AISelect : MonoBehaviour {
	AI_Upload ai_up;

	bool isSelected;
	bool isGetData;
	void Start () {
		ai_up = new AI_Upload(this.name);
	}
	

	public void SelectAI(){
		isSelected = true;
	}

	void Update(){
		if(isSelected){
			ai_up.fetch(this.name);
			isSelected = false;
		}
		if(ai_up.isNoData){
			// 選択し直し
			isSelected = true;
			ai_up.isNoData = false;
		}

		if(ai_up.isFetched && !isGetData){
			ai_up.isFetched = false;
			isGetData = true;

			ToBattle(2);
		}
	}


	void ToBattle(int level) {
		/*
			GameObject.Find("Menu/Panels/ArenaPanel/Alert").gameObject.GetComponent<Text>().enabled = true;
			isGetData = false;
			*/
		if(!GameConfiguration.IsSetAI){
			Text tmp = GameObject.Find("Menu/Panels/ArenaPanel/Alert").gameObject.GetComponent<Text>();
			tmp.text = "AI を作成してください";
			tmp.enabled = true;
			Text tmp2 = GameObject.Find("Menu/Panels/Arena2Panel/Alert").gameObject.GetComponent<Text>();
			tmp2.text = "AI を作成してください";
			tmp2.enabled = true;

			isGetData = false;
			return;
		}
		SceneManager.LoadScene("Battle");
	}

}
