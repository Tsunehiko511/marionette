using UnityEngine;
using System.Collections;
using NCMB;

public class SendToServer : MonoBehaviour {
	AI_Upload ai_up;

	bool b1;
	bool b2;
	bool b3;


	string Myname;
	void Awake(){
		Myname = FindObjectOfType<UserAuth>().currentPlayer();
		Debug.Log(Myname);
	}
	void Start(){
		ai_up = new AI_Upload(Myname);
	}
	void Update(){
		/*
		if(ai_up.isSearch && !b1){
			b1 = true;
			Debug.Log("検索");
		}
		if(ai_up.isNoConnect && !b2){
			Debug.Log("isNoConnect");
			b2 = true;
		}
		if(ai_up.isNewSaved){
			Debug.Log("サーバー新規登録");
		}
		if(ai_up.isSaved){
			Debug.Log("サーバー登録");
		}
		*/
	}

	public void sendToServer(){
		NodeArrow.Saved();
		ai_up.save();
	}
}
