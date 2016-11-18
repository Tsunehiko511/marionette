using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using NCMB;
public class OnlineAIView : MonoBehaviour {
	public static int max;
	public GameObject   icon;
	public static GameObject[] icons;
	private int count;
	public static bool GetName{get; set;}

  // private RatingBoard rBoard;                   // 自分のランク, トップ5, 近隣5
  private RatingBoard rBoard;                   // 自分のランク, 近隣5
  private NCMB.Rating myRate;
  public static bool isRateFetched;
  public static bool isRankFetched;                           // ランキングを取得できたか
  public static bool isLeaderBoardFetched;                    // ランキングボードを取得できたか
  private string myname;


	// Use this for initialization
	void Start () {
    // Rateing表
    rBoard = new RatingBoard();

    // 自分のスコアを取得
    myname = FindObjectOfType<UserAuth>().currentPlayer();
    myRate = new NCMB.Rating( 0, myname );
    myRate.fetch();

    // フラグ初期化
    isRateFetched = false;
    isRankFetched = false;
    isLeaderBoardFetched = false;
	}
	
	// Update is called once per frame
	void Update () {
    /*
		if(User.Level <6){
			return;
		}
    */
    if(myRate.isNoData && !isRateFetched){
      rBoard.fetchRank( 1500 );
      isRateFetched = true;
    }
    // 現在のランクの取得が完了したら1度だけ実行
    if( myRate.rate != 0 && !isRateFetched ){
      Debug.Log("isRateFetched:"+myRate.rate);
      rBoard.fetchRank( myRate.rate );
      isRateFetched = true;
    }

    // 現在の順位の取得が完了したら1度だけ実行
    if( rBoard.currentRank != 0 && !isRankFetched ){
      Debug.Log("isRankFetched:"+rBoard.currentRank);
      rBoard.fetchNeighbors();
      isRankFetched = true;
    }    

    // ランキングの取得が完了したら1度だけ実行
    if( rBoard.neighbors != null && !isLeaderBoardFetched){ 
      Debug.Log("isLeaderBoardFetched");
      Debug.Log("count="+rBoard.neighbors.Count);

  // 自分が1位のときと2位のときだけ順位表示を調整
      int offset = 2;
      if(rBoard.currentRank == 1) offset = 0;
      if(rBoard.currentRank == 2) offset = 1;

      // 取得したライバルランキングを表示
      /*
      for( int i = 0; i < rBoard.neighbors.Count; ++i) {
      	nei[i].guiText.text = rBoard.currentRank - offset + i + ". " + rBoard.neighbors[i].print();
      }*/


      // 近い人のどれかを表示
      drawIcon(rBoard.neighbors);

      isLeaderBoardFetched = true;
    }
	}

	public void drawIcon(List<NCMB.Rating> _neighbors){
		count = _neighbors.Count;
		icons = new GameObject[count]; //+1は”外す”
		// int r = UnityEngine.Random.Range(0, count);
		/*while(myname == _neighbors[r].name){
			r = UnityEngine.Random.Range(0, count);
		}
		Debug.Log("r="+r);
    */

		for(int i=0; i<count; i++){
      addIcon(i, _neighbors[i].name,  _neighbors[i].rate, new Vector3 (70f+ (i%3)*100f, -60f - 100*(i/3), 0), "Menu/Panels/ArenaPanel/ScrollView/List");
      /*
			if(i < r){
				addIcon(i, _neighbors[i].name,  _neighbors[i].rate, new Vector3 (70f+ (i%3)*100f, -60f - 100*(i/3), 0), "Menu/Panels/ArenaPanel/ScrollView/List");
			}
			else if(i==r){
				addIcon(r, _neighbors[r].name,  _neighbors[r].rate, new Vector3 (0, 0, 0),"Menu/Panels/Arena2Panel");
			}
			else{
				addIcon(i, _neighbors[i].name,  _neighbors[i].rate, new Vector3 (70f+ ((i-1)%3)*100f, -60f - 100*((i-1)/3), 0), "Menu/Panels/ArenaPanel/ScrollView/List");
			}*/
		}
	}

	public void addIcon(int idx, string _name, int _rate, Vector3 _position, string _typeString){
		icons[idx] = (GameObject)Instantiate(icon, _position, Quaternion.Euler(new Vector3(0, 0, 0)));
		icons[idx].transform.SetParent(GameObject.Find(_typeString).transform, false);
		icons[idx].name = _name;

		GameObject.Find(_typeString + "/" + _name +"/Text").GetComponent<Text>().text = _rate+" : "+_name;
	}

}
