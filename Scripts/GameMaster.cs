using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMaster : MonoBehaviour {
	public MapBoard boardPrefab;
	public Unit cubePrefabKING;
	public Unit cubePrefabQUEEN;
	public Unit cubePrefabPOON;
	public Unit cubePrefabROOK;
	public Unit cubePrefabBISHOP;

	public MapBoard board;
	GameObject target;
	Timer Timer;
	public List<Unit> units;
	public List<Unit> move_units;

	public bool IsSelectPhase;
	public bool IsMEventPhase;
	public bool IsMovePhase;
	// public bool IsMDrawPhase;
	public bool IsATKEventPhase;
	public bool IsAttsckPhase;
	public bool IsATKDrawPhase;
	public bool IsCountDownPhase;
	public bool IsEndPhase;
	public int MovedCount;

	private int uCount;
	bool IsEnd;
	string loseTeam;

	void Awake (){

		// ボードの生成　初期化
		board = Instantiate(boardPrefab) as MapBoard;
		board.name = "Board";

		units = new List<Unit>();
		move_units = new List<Unit>();
		Timer = GameObject.Find("Canvas/Timer").GetComponent<Timer>();
	}
// int _x, int _z, int _loc, int _waitTime, string _team, int _hp, int _attack, int _defense, string _type, List<int []> _connectList
	void SpawnUnit(string _name , int _position_x, int _position_z, int _loc, string _team, int _hp, int _attack, int _defense, List<int []> _connectList){
		Unit cube = Instantiate(GetPrefabUnit(_name), new Vector3 (_position_x*Constants.BLOCK_SIZE, 0, _position_z*Constants.BLOCK_SIZE), Quaternion.Euler(new Vector3(0, 180, 0))) as Unit;
		cube.Initialize(_position_x,_position_z,_loc, 50+5*_connectList.Count, _team, _hp, _attack, _defense, _name, _connectList); // 座標, 移動力
		cube.name = _team+_name;
		units.Add(cube);
	}
	/*
	void SpawnUnit(string _name , int _position_x, int _position_z, int _loc, int _waitTime, string _team, int _hp, int _attack, int _defense, List<int []> _connectList){
		Unit cube = Instantiate(GetPrefabUnit(_name), new Vector3 (_position_x*Constants.BLOCK_SIZE, 0, _position_z*Constants.BLOCK_SIZE), Quaternion.Euler(new Vector3(0, 180, 0))) as Unit;
		cube.Initialize(_position_x,_position_z,_loc, _waitTime, _team, _hp, _attack, _defense, _name, _connectList); // 座標, 移動力
		cube.name = _team+_name;
		units.Add(cube);
	}
	*/
	Unit GetPrefabUnit(string _type){
		switch(_type){
			case "KING":
			return cubePrefabKING;
			case "QUEEN":
			return cubePrefabQUEEN;
			case "POON":
			return cubePrefabPOON;
			case "ROOK":
			return cubePrefabROOK;
			case "BISHOP":
			return cubePrefabBISHOP;
			default:
			return cubePrefabPOON;
		}
	}

	// Use this for initialization
	void Start () {
		loseTeam = "";
		// ユニットの生成 初期化
		
		SpawnUnit("KING", 	1, 	6, 2, 	"My", 		100, 30, 20, MyConnects.connectLists[0]);
		SpawnUnit("ROOK", 	2, 	3, 3, 	"My", 		100, 30, 20, MyConnects.connectLists[1]);
		SpawnUnit("POON", 	4, 	6, 3, 	"My", 		100, 30, 20, MyConnects.connectLists[2]);
		SpawnUnit("BISHOP", 2, 	9, 4, 	"My", 		100, 30, 20, MyConnects.connectLists[3]);
		SpawnUnit("KING", 	20, 6, 2, 	"Enemy", 	100, 30, 20, Enemys.connectLists[0]);
		SpawnUnit("ROOK", 	19, 9, 3, 	"Enemy", 	100, 30, 20, Enemys.connectLists[1]);
		SpawnUnit("POON", 	17, 6, 3, 	"Enemy", 	100, 30, 20, Enemys.connectLists[2]);
		SpawnUnit("BISHOP", 19, 3, 4, 	"Enemy", 	100, 30, 20, Enemys.connectLists[3]);
		/*
		SpawnUnit("KING", 	1, 	6, 2, 94, 	"My", 		100, 30, 20, MyConnects.connectLists[0]);
		SpawnUnit("ROOK", 	2, 	3, 3, 96, 	"My", 		100, 30, 20, MyConnects.connectLists[1]);
		SpawnUnit("POON", 	4, 	6, 3, 102, 	"My", 		100, 30, 20, MyConnects.connectLists[2]);
		SpawnUnit("BISHOP", 2, 	9, 4, 108, 	"My", 		100, 30, 20, MyConnects.connectLists[3]);
		SpawnUnit("KING", 	20, 6, 2, 93, 	"Enemy", 	100, 30, 20, Enemys.connectLists[0]);
		SpawnUnit("ROOK", 	19, 9, 3, 114, 	"Enemy", 	100, 30, 20, Enemys.connectLists[1]);
		SpawnUnit("POON", 	17, 6, 3, 95, 	"Enemy", 	100, 30, 20, Enemys.connectLists[2]);
		SpawnUnit("BISHOP", 19, 3, 4, 98, 	"Enemy", 	100, 30, 20, Enemys.connectLists[3]);
		*/
		uCount = 0;
		IsSelectPhase = true;
		IsMEventPhase = true;
		IsMovePhase = true;
		IsATKEventPhase = true;
		IsAttsckPhase = true;
		IsATKDrawPhase = true;
		IsCountDownPhase = true;
		IsEndPhase = true;
		Invoke("SetSelectBool",2f);
	}

	/*


	*/
	
	// Update is called once per frame
	void Update () {
		if(IsEnd){
			return;
		}

		// 動かすユニットを決める(waitTimeが最も小さいUnitを選択：今回は一つ)
		if(!IsSelectPhase){
			Debug.Log("選択フェイズ");
			IsSelectPhase = true;

			int can_move_count = 10000;
			foreach(Unit unit in units){
				// waitTimeを取得
				int tmp_waitTime = unit.waitTime;
				// Debug.Log(tmp_waitTime);
				// より早いunitがあればmove_unitsを新規作成
				if(can_move_count > tmp_waitTime){
					can_move_count = tmp_waitTime;
					move_units.Clear();
					move_units.Add(unit);					
				}
				else if(can_move_count == tmp_waitTime){
					// 同じなら追加
					move_units.Add(unit);					
				}
			}

			// 動かすunitのwaitTimeを取得
			// 全体からwaitTimeを引く
			IsMEventPhase = false;
		}
		// イベントの取得
		if(!IsMEventPhase){
			Debug.Log("移動イベントフェイズ");
			IsMEventPhase = true;
			// 移動関数 = 移動範囲を求める，ターゲットを決める，移動場所を決める
			foreach(Unit unit in move_units){
				unit.SetMEvent(board);
			}
			// アニメーションがないからそのまま次へ
			IsMovePhase = false;
		}

		// 移動フェイズ = 移動計算＋アニメーション
		if(!IsMovePhase){
			Debug.Log("移動フェイズ");
			IsMovePhase = true;
			// 移動関数 = 移動範囲を求める，ターゲットを決める，移動場所を決める
			foreach(Unit unit in move_units){
				unit.Move(board);
			}
			// アニメーションがあるからここではトリガーを引かない
		}

		// アニメーションが終わったら
		if(!IsATKEventPhase && MovedCount == move_units.Count){
			MovedCount = 0;
			Debug.Log("攻撃イベントフェイズ");
			IsATKEventPhase = true;
			// 移動関数 = 移動範囲を求める，ターゲットを決める，移動場所を決める
			foreach(Unit unit in move_units){
				unit.SetATKEvent(board);
			}
			// アニメーションがないからそのまま次へ
			IsAttsckPhase = false;
		}

		// 攻撃フェイズ = 攻撃計算
		if(!IsAttsckPhase){
			Debug.Log("攻撃フェイズ");
			IsAttsckPhase = true;

			// 攻撃関数 = ターゲットを決めてダメージ計算する
			foreach(Unit unit in move_units){
				unit.Attack(board);
			}
			// アニメーションがないからそのまま次へ
			IsATKDrawPhase = false;
		}

		// 攻撃描画フェイズ = アニメーション＋数値表示
		if(!IsATKDrawPhase){
			Debug.Log("攻撃描画フェイズ");
			IsATKDrawPhase = true;

			// 攻撃描画関数
			uCount = 0;
			foreach(Unit unit in units){
				if(unit.IsAttack || unit.IsDamage){
					uCount++;
					unit.AttackDraw(board);
				}
			}
			if(uCount == 0){
				IsCountDownPhase = false;
			}
			/*
			foreach(Unit unit in move_units){
				unit.AttackDraw(board);
			}*/
			// アニメーションがあるからここではトリガーを引かない			
		}

		// カウントダウンフェイズ　& 勝利判定
		if(!IsCountDownPhase && MovedCount == uCount){
			IsCountDownPhase = true;
			MovedCount = 0;
			if(loseTeam != ""){
				IsEnd = true;
				GameObject.Find("GameOver").GetComponent<gameOverScript>().Lose(loseTeam);
			}
			// カウントダウン
			if(Timer.CountDown() && !IsEnd){
				IsEnd = true;
			}
			IsEndPhase = false;
		}



		// 終了フェイズ
		if(!IsEndPhase && !IsEnd){
			Debug.Log("終了フェイズ");
			IsEndPhase = true;
			int moved_waitTime = move_units[0].waitTime;
			// 他のunitのwaitTimeを引く
			foreach(Unit unit in units){
				unit.waitTime -= moved_waitTime;
				// 動かしたunitのwaitTimeを元に戻す
				if(unit.waitTime==0){
					unit.waitTime = unit.waitTime_max;
				}
			}
			IsSelectPhase = false;
		}
	}
	public void SetSelectBool(){
		IsSelectPhase = false;
	}


	public void SetLOSE(string _name_team){
		if(loseTeam == ""){
			loseTeam = _name_team;
		}
		else{
			loseTeam = "DRAW";
		}
	}


	public void SetATKEventBool(){
		IsATKEventPhase = false;
		MovedCount++;
	}

	public void SetCountDownBool(){
		IsCountDownPhase = false;
		MovedCount++;
	}
}
