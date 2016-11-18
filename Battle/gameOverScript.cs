using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using NCMB;

public class gameOverScript : MonoBehaviour {
  public Slider castleHP1;
  public Slider castleHP2;
  int score1;
  int score2;

	NCMB.Rating my_Data;
	NCMB.Rating opp_Data;
	private NCMB.Rating rating;

	private int rate;
	private bool isGetName;
	private bool isPushMenu;
	private bool isFetched;
	private bool isSend;
	GameObject giveUpButton;
	// GameObject goConfigButton;
	GameObject goReturnButton;
	// GameObject goMenuButton;
	Text result;

  float win_point = 0;

	bool gameOverflg = false;
	void Awake(){
		giveUpButton 		= GameObject.Find("Canvas/MenuButton");
		// goConfigButton 	= this.gameObject.transform.FindChild("ConfigButton").gameObject;
		goReturnButton	= GameObject.Find("Canvas/ReturnButton");
		result = GameObject.Find("Canvas/Text").GetComponent<Text>();
	}
	void Start () {
		result.enabled = false;
		// giveUpButton 		= this.gameObject.transform.FindChild("Button").gameObject;
		// goConfigButton 	= this.gameObject.transform.FindChild("ConfigButton").gameObject;
		// goReturnButton	= this.gameObject.transform.FindChild("ReturnButton").gameObject;
		// goMenuButton 		= this.gameObject.transform.FindChild("GoMenu").gameObject;
		giveUpButton.SetActive(false);
		// goConfigButton.SetActive(false);
		goReturnButton.SetActive(false);
		// goMenuButton.SetActive(false);
	}



	void Update (){
		// メニューボタンが押されたら
		if(isPushMenu){

			if(!isGetName){
				string my_name = FindObjectOfType<UserAuth>().currentPlayer();
				string opp_name = Enemys.name;

		    my_Data = new NCMB.Rating( 1500, my_name );
		    opp_Data = new NCMB.Rating( 1500, opp_name );
				isGetName = true;
				Debug.Log(my_name);
				Debug.Log(opp_name);
			}

			if(my_Data.name == opp_Data.name){
				Time.timeScale = 1;
				SceneManager.LoadScene("Menu");
				return;
			}


			// 名前の取得ができたら
			if(my_Data.name != null && opp_Data.name != null && !isFetched){

				// rateの取得
		    my_Data.fetch();
		    opp_Data.fetch();
		    isFetched = true;
			}

			if(my_Data.isNoData){
				Time.timeScale = 1;
				SceneManager.LoadScene("Menu");
				return;
			}


			// Rateの取得ができたら
			if(my_Data.isFetched && opp_Data.isFetched && !isSend){
				int my_rate = my_Data.rate;
				int opp_rate = opp_Data.rate;
				Debug.Log(my_rate);
				Debug.Log(opp_rate);
				// 計算してサーバに送信
				my_Data.rate = GetNewRate(win_point, my_rate, opp_rate);
				opp_Data.rate = GetNewRate(1-win_point, opp_rate, my_rate);
				my_Data.save();
				opp_Data.save();
				Debug.Log("my_name="+my_rate);
				Debug.Log("opp_name="+opp_rate);
				isSend = true;
			}

			// 勝って，名前の取得ができたら = データがある！　 
			if(my_Data.isSaved && opp_Data.isSaved){
				Time.timeScale = 1;
				SceneManager.LoadScene("Menu");
			}
		}
	}

	int GetNewRate(float _win_point , int _rate1, int _rate2){
		float E1 	= 1f/(1f+Mathf.Pow(10,((_rate2 - _rate1)*0.0025f)));
		int new_rate1 = _rate1 + Mathf.RoundToInt(32f*(_win_point - E1));
		return new_rate1;
	}

	
	public void Lose (string loseTeam){
		if(loseTeam != "" && loseTeam != "ポーズ"){
			// goMenuButton.SetActive(true);
		}
		else{
		}
		Debug.Log(loseTeam);
		giveUpButton.SetActive(true);
		if(loseTeam=="Team1"){
			result.text = "LOSE : 全滅";
			win_point = 0;
		}
		else if(loseTeam=="Team2"){
			result.text = "WIN : 撃破";
			win_point = 1;
		}
		else if(loseTeam=="MyKING"){
			result.text = "LOSE : 大将 撤退";
			win_point = 0;
		}
		else if(loseTeam=="EnemyKING"){
			result.text = "WIN : 大将 撃破";
			win_point = 1;
		}
		else if(loseTeam=="DRAW"){
			result.text = "DRAW";
			win_point = 0;
		}
		else if(loseTeam=="TimeUp"){
			Unit myKing = GameObject.Find("MyKING").GetComponent<Unit>();
			Unit enemyKing = GameObject.Find("EnemyKING").GetComponent<Unit>();
			if(myKing.hp > enemyKing.hp){
				result.text = "WIN : 大将のHP";
				win_point = 1;
			}
			else if(myKing.hp < enemyKing.hp){
				result.text = "LOSE : 大将のHP";
				win_point = 0;
			}
			else{
				result.text = "DRAW : TIME UP";
				win_point = 0.5f;
			}
		}
		else{
			result.text = "";
			goReturnButton.SetActive(true);
			// goConfigButton.SetActive(true);
		}
		result.enabled = true;
		/*
		GameObject[] clones = GameObject.FindGameObjectsWithTag ("Finish"); // TODO
		foreach (GameObject obj in clones) {
			Destroy(obj);
		}*/

		Time.timeScale = 0;
		gameOverflg = true;
	}

	public void MenuPush (){
		if (gameOverflg == true) {
			// GameObject.Find("Audio Source2").GetComponent<MusicPlayer>().musicStop();


			/*---- レーティングセット----*/

			// レーティングの計算
			// string my_name = SaverData.name;
			// string opp_name = Enemy.name;
			isPushMenu = true;
		}
	}

	public void ReturnPush (){
			Time.timeScale = 1;		

			result.enabled = false;
			giveUpButton.SetActive(false);
			goReturnButton.SetActive(false);
			// goConfigButton.SetActive(false);
			// goMenuButton.SetActive(false);
	}

	public void ConfigPush (){
			result.enabled = false;
			giveUpButton.SetActive(false);
			goReturnButton.SetActive(false);
			// goConfigButton.SetActive(false);
			// goMenuButton.SetActive(false);
	}
}