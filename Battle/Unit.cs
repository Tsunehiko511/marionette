using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Unit : MonoBehaviour {
	public int[] xz_position;	// 現在地
	public int locomotion;		// 移動力
	public int waitTime_max;			// 待ち時間
	public int waitTime;			// 待ち時間
	public string team;
	public string enemy_team;
	public string type;

	public int state; 				// 現在の状態
	public string s_state;
	public int hp;
	public int attack; 				// 現在の状態
	public int defense; 				// 現在の状態

	public List<int[]> connectList;

	public int sum_damage;
	public bool IsAttack;
	public bool IsDamage;

	Slider _slider;


	public Canvas _canvas;
	Camera rotateCamera;

	public int[,] attackRange;					// 攻撃範囲

	List<Unit> sub_enemy_targets;
	List<Unit> sub_friend_targets;
	int currentFlag;
	int eventFlag;
	public int connectStartId;
	bool getStartId;


	int D_APPRECIABLE;

	Animator animator;
	AnimatorStateInfo stateInfo;



	// Use this for initialization
	GameObject gameMaster;
	GameObject board;

	MapBoard mcBoard;
	int[,] old_marea;					// 一つ前の移動範囲
	public int[] target_position;
	void Awake(){
		xz_position = new int[2];

	}
	void Start () {
		gameMaster = GameObject.Find("GameMaster");
		board = GameObject.Find("Board");
		old_marea = new int[Constants.MAP_SIZE_X+2,Constants.MAP_SIZE_Z+2];
		target_position = new int[2];

		SetColor(this.team);
		board.GetComponent<MapBoard>().SetMapBoard(this.xz_position[0],this.xz_position[1], this);
		attackRange = new int[4,2]{{0,1}, {1,0}, {0,-1}, {-1,0}};
		_slider = this.gameObject.transform.FindChild("UnitCanvas/Slider").gameObject.GetComponent<Slider>();
		_slider.value = this.hp;

		D_APPRECIABLE = this.locomotion;
		sub_enemy_targets = new List<Unit>();
		sub_friend_targets = new List<Unit>();
		sum_damage = 0;
		IsDamage = false;

		animator = this.GetComponent<Animator>();
		foreach (Transform child in _canvas.transform){
			if(child.name == "Text"){
				Text tmp_text = child.gameObject.GetComponent<Text>();
				tmp_text.gameObject.SetActive (false);
			}
		}
		rotateCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
	}
	public void Initialize(int _x, int _z, int _loc, int _waitTime, string _team, int _hp, int _attack, int _defense, string _type, List<int []> _connectList){
		this.xz_position[0] = _x;
		this.xz_position[1] = _z;
		this.locomotion = _loc;
		this.waitTime_max = _waitTime;
		this.waitTime = _waitTime;
		this.state = Constants.S_START;
		SetFlag(Constants.F_START);
		this.team = _team;
		if(_team == "My"){
			this.enemy_team = "Enemy";
		}
		else{
			this.enemy_team = "My";			
		}
		this.hp = _hp;
		this.attack = _attack;
		this.defense = _defense;
		this.type = _type;
		this.connectList = GetConnect(_connectList);
	}

	void Update(){
		_canvas.transform.rotation = rotateCamera.transform.rotation;
	}

	/*---------
	* 状態遷移 *
	---------*/

	// MDAxMTAwMDAxMDEwMDExMDExMDAwMDAwMTAxMDAwMDAxMDAwMDEwMDAwMTAwMDEwMTAwMDAxMTExMTAwMDEwMDAxMDEwMDAxMDAwMTEwMDEwMTAwMDExMDExMTAwMTExMDAxMDAwMTAwMTEwMDAxMTAxMDAwMDEwMTAwMDEwMDAwMDEwMDAwMTAwMDEwMDAwMDAwMDAxMDAwMDAxMDAwMTEwMDAwMDAwMDExMDAwMDEwMDAxMTAwMDAwMDAwMTEwMDAwMTAwMDExMDAwMDAwMDAxMDAwMDAxMDAwMTEwMDAwMDAwMDAxMDAwMDEwMDAxMDAwMDAwMDAwMTEwMDAxMDAwMDEwMDAwMDA=
	bool IsTransition(int now_state, int now_event, int next_state){
		if(now_state != this.state){
			return false;
		}
		if(IsFlagMatch(eventFlag, now_event)){
			currentFlag = now_event;
			this.s_state = Constants.GetStatusString(next_state);
			if(this.state == next_state){
				return true;	//TODO 無限ループするかも
			}
			this.state = next_state;
			// Debug.Log(this.s_state);
			// return true;
		}
		return false;
	}

  bool NextFlagIsAnd(List<int[]> _connectList, int next_i){
  	if(next_i < _connectList.Count){
  		int next_flag = _connectList[next_i][3];
  		if(next_flag == Constants.AND){
  			return true;
  		}
  	}
  	return false;
  }

  // 遷移
  void DoTransition(List<int[]> _connectLists){
  	SetFlag(Constants.F_UNCONDITIONAL);

  	int idx = GetStartConnect(connectStartId, _connectLists);
  	if(idx == -1){
  		//Debug.Log("ID取得エラー");
  		//return;
  		idx = 0;
  		this.state = Constants.S_START;
  		SetFlag(Constants.F_START);
  	}
  	for(int i=0; i<_connectLists.Count; i++){
  		int num = (i+idx)%_connectLists.Count;
	  	int[] _connectList = _connectLists[num];
      if(IsTransition(_connectList[1], _connectList[2], _connectList[4])){// state, position, id
      	connectStartId = _connectList[3];																	// flag
      	getStartId = true;
      	break;
      }
  	}
    eventFlag = 0; // TODO ここで0にしてもいいのか？
  }

  // 接続場所の検索
  int GetStartConnect(int _startId, List<int[]> _connectLists){
  	if(!getStartId){
  		return 0; // 初期値
  	}
  	for(int i=0; i<_connectLists.Count; i++){
			int[] _connectList = _connectLists[i];
	  	if(_connectList[0] == connectStartId){
	  		return i;
	  	}  	
  	}
  	return -1;
  }

  List<int[]> GetConnect(List<int[]> _connectList){
	  List<int[]> tmp_connects = new List<int[]>();
  	int flag = 0;
  	// 次の条件がANDなら今のフラグと結合して次に行く
    for (int i=0; i<_connectList.Count; i++){
    	flag = flag | _connectList[i][3]; // 今のフラグ(こいつがANDになることはない！)

    	// 次がANDなら2つ次のペアに行く　[1,1,1,2,0][1,1,AND,2,0][1,1,2,2,0]
    	if(NextFlagIsAnd(_connectList, i+1)){
    		// 今をANDの状態にして次に行く
    		i = i+1;
    		continue;
    	}

    	if(NextFlagIsAnd(_connectList, i)){
    		Debug.Log("ANDだよ！！");
    	}
    	int s_id 				= _connectList[i][0];
	    int now_state 	= _connectList[i][1];
	    int e_id 				= _connectList[i][4];
	    int next_state 	= _connectList[i][5];
	    tmp_connects.Add(new int[]{s_id, now_state, flag, e_id, next_state});
      flag = 0; 								// 使い終わったらリセット(ANDならここまでこない)
    }
    return tmp_connects;
  }

  void SetConnectList(){
  	if(connectList.Count == 0){
			if(this.team == "My"){
	  		if(this.type == "KING"){
		  		connectList = GetConnect(MyConnects.connectLists[0]);
	  		}
	  		else if(this.type == "ROOK"){
		  		connectList = GetConnect(MyConnects.connectLists[1]);
	  		}
	  		else if(this.type == "BISHOP"){
		  		connectList = GetConnect(MyConnects.connectLists[1]);
	  		}
	  		else{
		  		connectList = GetConnect(MyConnects.connectLists[1]);
	  		}				
			}
			else if(this.team == "Enemy"){
	  		if(this.type == "KING"){
		  		connectList = GetConnect(Enemys.connectLists[0]);
	  		}
	  		else if(this.type == "ROOK"){
		  		connectList = GetConnect(Enemys.connectLists[1]);
	  		}
	  		else if(this.type == "BISHOP"){
		  		connectList = GetConnect(Enemys.connectLists[1]);
	  		}
	  		else{
		  		connectList = GetConnect(Enemys.connectLists[1]);
	  		}
			}
  		this.state = Constants.S_START;
  		SetFlag(Constants.F_START);
  		getStartId = false;
  	}
  }

	public void SetMEvent(MapBoard _mcBoard){
		SetConnectList();
		// イベントを取得
		SetDistanceEvent(_mcBoard);					// 周辺の敵，味方を感知
		SetCloseToKingEvent(_mcBoard);			// 自軍大将周辺の敵，味方を感知

		// 状態遷移
		DoTransition(connectList);
	}

	public void SetATKEvent(MapBoard _mcBoard){
		SetConnectList();		
		// イベント取得
		SetDistanceEvent(_mcBoard);

		// 状態遷移
		DoTransition(connectList);
	}
	public void SetFlag(int eventType){
		eventFlag = eventFlag | eventType;
	}


	void SetDistanceEvent(MapBoard _mcBoard){
		if(sub_enemy_targets == null){
			sub_enemy_targets = new List<Unit>();
		}
		else{
			sub_enemy_targets.Clear();
		}
		if(sub_friend_targets == null){
			sub_friend_targets = new List<Unit>();
		}
		else{
			sub_friend_targets.Clear();	
		}

		int[,] marea = _mcBoard.GetMoveArea(xz_position[0],xz_position[1], this.locomotion, this.team);
		for(int i=0; i<marea.GetLength(0); i++){
			for(int j=0; j<marea.GetLength(1); j++){
				if(marea[i,j] < 0 || marea[i,j] == this.locomotion){
					continue;
				}

				// 移動範囲+攻撃範囲に敵 or 味方がいるか
				for(int k=0; k<attackRange.GetLength(0); k++){
					int tmp_i = i + attackRange[k,0];
					int tmp_j = j + attackRange[k,1];
					// 敵との距離でフラグ設定
					Unit tmp_enemy = _mcBoard.GetMapBoardUnit(tmp_i,tmp_j);
					if(IsTeamMate(tmp_enemy) == this.enemy_team){
						// Debug.Log(marea[i,j]+":"+tmp_i+","+tmp_j);
						if(this.state == Constants.S_ATK_KING && tmp_enemy.type == "KING"){

						}
						else{
							SetFlag(Constants.F_E_APPRECIABLE);
							// 攻撃候補を取得
							if(!sub_enemy_targets.Contains(tmp_enemy)){
								sub_enemy_targets.Add(tmp_enemy);
							}							
						}
					}
					else if(IsTeamMate(tmp_enemy) == this.team ){
						SetFlag(Constants.F_F_APPRECIABLE);
						// 攻撃候補を取得
						if(!sub_friend_targets.Contains(tmp_enemy)){
							sub_friend_targets.Add(tmp_enemy);
						}
					}
				}
			}
		}
	}
	// MDAxMTAwMDAxMDEwMDExMDExMDAwMDAwMTAxMDAwMDAxMDAwMDEwMDAwMTAwMDEwMTAwMDAxMTExMTAwMDEwMDAxMDEwMDAxMDAwMTEwMDEwMTAwMDExMDExMTAwMTExMDAxMDAwMTAwMTEwMDAxMTAxMDAwMDEwMTAwMDEwMDAwMDEwMDAwMTAwMDEwMDAwMDAwMDAxMDAwMDAxMDAwMTEwMDAwMDAwMDExMDAwMDEwMDAxMTAwMDAwMDAwMTEwMDAwMTAwMDExMDAwMDAwMDAxMDAwMDAxMDAwMTEwMDAwMDAwMDAxMDAwMDEwMDAxMDAwMDAwMDAwMTEwMDAxMDAwMDEwMDAwMDA=
	void SetCloseToKingEvent(MapBoard _mcBoard){
		Unit myKing = GetKing(this.team, _mcBoard, "KING");
		// Unit myKing = GetKing(this.team);
		// Debug.Log(myKing.xz_position[0]+","+myKing.xz_position[1]+":"+myKing.team);
		int[,] marea = _mcBoard.GetMoveArea(myKing.xz_position[0],myKing.xz_position[1], 4, myKing.team);
		for(int i=0; i<marea.GetLength(0); i++){
			for(int j=0; j<marea.GetLength(1); j++){
				if(marea[i,j]<0 || marea[i,j] == 4){
					continue;
				}

				// 移動範囲+攻撃範囲に敵 or 味方がいるか
				for(int k=0; k<myKing.attackRange.GetLength(0); k++){
					int tmp_i = i + myKing.attackRange[k,0];
					int tmp_j = j + myKing.attackRange[k,1];
					// 敵との距離でフラグ設定
					Unit tmp_enemy = _mcBoard.GetMapBoardUnit(tmp_i,tmp_j);		
					if(IsTeamMate(tmp_enemy) == myKing.enemy_team){
						SetFlag(Constants.F_CLOSE_TO_F_KING_ENEMY);
					}
					else if(IsTeamMate(tmp_enemy) == myKing.team){
						SetFlag(Constants.F_CLOSE_TO_F_KING_FRIEND);
					}
				}
			}
		}		
	}

	bool IsFlagMatch(int _eventFlag, int _flag){
		if((_eventFlag & _flag) == _flag){
			return true;
		}
		return false;
	}





	public void AttackDraw(MapBoard _mcBoard){
		if(this.IsAttack){
			this.IsAttack = false;
			animator.SetTrigger("AttackTrigger");
			// animator.SetInteger("animation",11);
			animator.Update(0);
			stateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
			StartCoroutine(DelayDamageDraw(stateInfo.length, _mcBoard));
			if(this.IsDamage){
				Invoke("ViewDamage", stateInfo.length/2f);
			}
		}
		else if(this.IsDamage){
			StartCoroutine(DelayDamage(0.25f, _mcBoard));
			Invoke("ViewDamage", 0.3f);
		}
		else{
			gameMaster.GetComponent<GameMaster>().SetCountDownBool();
		}

		// 終了
	}
	private IEnumerator DelayDamage(float waitTime, MapBoard _mcBoard){
	    yield return new WaitForSeconds(waitTime);
			animator.SetTrigger("OnDamageTrigger");
			animator.Update(0);
			stateInfo = this.animator.GetCurrentAnimatorStateInfo(0);			
			// ダメージ表示！とダメージエフェクト hp==0なら死亡
			StartCoroutine(DelayDamageDraw(stateInfo.length, _mcBoard));
	}

	void ViewDamage(){
		_canvas.transform.rotation = rotateCamera.transform.rotation;
		foreach (Transform child in _canvas.transform){
			if(child.name == "Text"){
				Text tmp_text = child.gameObject.GetComponent<Text>();
				tmp_text.text ="-"+ this.sum_damage;
				tmp_text.gameObject.SetActive (true);
				_slider.value = this.hp;
				Invoke("HideDamage", 0.3f);
			}
		}		
	}
	void HideDamage(){
		foreach (Transform child in _canvas.transform){
			if(child.name == "Text"){
				Text tmp_text = child.gameObject.GetComponent<Text>();
				tmp_text.text ="";
				tmp_text.gameObject.SetActive (false);
				this.sum_damage = 0;				
			}
		}		
	}

	private IEnumerator DelayDamageDraw(float waitTime, MapBoard _mcBoard){
	    yield return new WaitForSeconds(waitTime);
	    DamageDraw(_mcBoard);
	}

	void DamageDraw(MapBoard _mcBoard){
		if(!this.IsDamage){
			gameMaster.GetComponent<GameMaster>().SetCountDownBool();
			return;
		}
		this.IsDamage = false;
		if(this.hp <= 0){
			// _mcBoard.SetMapBoard(xz_position[0],xz_position[1],"Empty", null);
			gameMaster.GetComponent<GameMaster>().units.Remove(this);
			// gameMaster.GetComponent<GameMaster>().move_units.Remove(this);
			// ボードから取り除く
			_mcBoard.SetMapBoard(xz_position[0], xz_position[1], null);
			if(this.type == "KING"){
				gameMaster.GetComponent<GameMaster>().SetLOSE(this.team+"KING");
			}
			Destroy(this.gameObject);
		}
		gameMaster.GetComponent<GameMaster>().SetCountDownBool();
	}
	public void Attack(MapBoard _mcBoard){
		List<Unit> taeget_enemys = GetTargetEnemys(_mcBoard);
		if(taeget_enemys.Count == 0 ){
			return;
		}
		// ダメージ計算をして終了
		Unit target_enemy = GetTargetEnemy(taeget_enemys);
		target_enemy.OnDamage(this.attack);
		this.IsAttack = true;
		transform.LookAt(target_enemy.gameObject.transform);
	}

	Unit GetTargetEnemy(List<Unit> _enemys){
		Unit tmp_taeget_enemy = null;
		// 評価 Evaluation
		int max_value = -1;
		foreach(Unit _enemy in _enemys){
			int value = GetEvaluation(_enemy);
			if(max_value < value){
				max_value = value;
				tmp_taeget_enemy = _enemy;
			}
		}

		return tmp_taeget_enemy;
	}

	int GetEvaluation(Unit _mCube){
		int value = 0;
		// ダメージがもっとも多い敵 得意

		return value;
	}

	List<Unit> GetTargetEnemys(MapBoard _mcBoard){
		List<Unit> tmp_taeget_enemy = new List<Unit>();
		// attackRange;
		for(int i=0; i<attackRange.GetLength(0); i++){
			// 攻撃範囲に敵がいるか調べる
			int tmp_x = xz_position[0]+attackRange[i,0];
			int tmp_z = xz_position[1]+attackRange[i,1];
			Unit tmp_mCube = _mcBoard.GetMapBoardUnit(tmp_x, tmp_z);
			// ターゲットならターゲットを攻撃!!
			if(target_position[0] == tmp_x && target_position[1] == tmp_z && IsTeamMate(tmp_mCube) == this.enemy_team){
				tmp_taeget_enemy.Clear();
				tmp_taeget_enemy.Add(tmp_mCube);
				break;
			}
			if(IsTeamMate(tmp_mCube) == this.enemy_team){
				tmp_taeget_enemy.Add(tmp_mCube);
			}
		}
		return tmp_taeget_enemy;
	}

	// 呼ばれるたびにHpが減る(防御力が優っていれば１) TODO ダメージ計算で表示の時に引く方がいいかな？
	public void OnDamage(int _atk){
		int tmp_damage = Mathf.Max(1, _atk - this.defense);
		this.sum_damage += tmp_damage;
		this.hp -= tmp_damage;
		this.IsDamage = true;
	}

	// TODO いらない
	void SetColor(string _team){
		// Color _color = this.gameObject.transform.FindChild("Canvas/Slider/Fill Area/Fill").gameObject.GetComponent<Image>().color;

		if(_team == "My"){
			this.gameObject.transform.FindChild("UnitCanvas/Slider/Fill Area/Fill").gameObject.GetComponent<Image>().color = new Color32(255, 0, 0, 255);
			// _color =  new Color32(255, 0, 0, 255);//Color.red;
		}
		else{
			this.gameObject.transform.FindChild("UnitCanvas/Slider/Fill Area/Fill").gameObject.GetComponent<Image>().color = new Color32(0, 0, 255, 255);
			// _color =  new Color32(0, 0, 255, 255);//Color.red;
		}
	}

	public void Move(MapBoard _mcBoard){
		// 状態からターゲットの決定
		if(!SetTarget(this.state, _mcBoard)){
			Animated(_mcBoard);
			return;
		}
		// target_position = new int[2]{18,9};
		// 移動範囲を表示
		// 移動場所を表示
		// 目的地まで移動
		ViewArea(_mcBoard);
		// 床を元に戻す。
	}

	// 状態に応じたターゲットを決める
	public bool SetTarget(int _state, MapBoard _mcBoard){
		Unit _target;
		switch(_state){
			case Constants.S_INIT:
			case Constants.S_START:
			case Constants.S_WAIT:
			_target = null;
			break;
			case Constants.S_ATK_ENEMY:
			_target = GetNearUnit(this.enemy_team);
			break;
			case Constants.S_ATK_KING:
			_target = GetKing(this.enemy_team, _mcBoard, "TARGET");
			break;
			case Constants.S_DEF_KING:
			_target = GetKing(this.team, _mcBoard, "TARGET");
			break;
			case Constants.S_DEF_FRIEND:
			_target = GetNearUnit(this.team);
			break;
			case Constants.S_GETWAY:
			_target = GetNearUnit(this.enemy_team);
			break;
			default: // S_START，S_WAIT
			_target = null;
			break;
		}
		if(_target == null){
			return false;
		}
		// Debug.Log(this.name+"ターゲット="+_target.name);
		target_position[0] = _target.xz_position[0];
		target_position[1] = _target.xz_position[1];
		return true;
	}

	Unit GetKing(string _team, MapBoard _mcBoard, string _text){
		Unit tmp_king = null;
		Unit[,] unit_board = _mcBoard.map_Board_Unit;
		for(int i=0; i<unit_board.GetLength(0); i++){
			for(int j=0; j<unit_board.GetLength(1); j++){
				if(unit_board[i,j] == null){
					continue;
				}
				if(unit_board[i,j].enemy_team == _team){
					continue;
				}
				if(unit_board[i,j].type == "KING"){
					tmp_king = unit_board[i,j];
					break;
				}
			}
		}
		if(_text == "KING"){
			return tmp_king;	
		}

		// 自分のチームなら，そいつのターゲットを返す。なければそいつ自身を返す
		if(_team == this.team){
			Unit tmp_unit = tmp_king.GetNearUnit(this.enemy_team);
			if(tmp_unit != null){
				tmp_king = tmp_unit;
			}
		}
		return tmp_king;
	}

	// 近い敵を取得
	public Unit GetNearUnit(string _team){
		int min_dist = 10000;
		Unit min_target = null;
		List<Unit> tmp_sub_targets;
		if(_team == this.team){
			tmp_sub_targets = sub_friend_targets;
		}
		else{
			tmp_sub_targets	= sub_enemy_targets;
		}

		foreach(Unit _enemy in tmp_sub_targets){
			int dist = GetManhattanDistance(this.xz_position, _enemy.xz_position);
			if(min_dist>dist){
				min_dist = dist;
				min_target = _enemy;
			}
		}
		// 自分のチームなら，そいつのターゲットを返す。なければそいつ自身を返す
		if(_team == this.team){
			Unit tmp_unit = min_target.GetNearUnit(this.enemy_team);
			if(tmp_unit != null){
				min_target = tmp_unit;
			}
		}
		return min_target;
	}

	void ViewAreaClear(MapBoard _mcBoard){
		if(old_marea.GetLength(0)>0 && old_marea.GetLength(1)>0){
			for(int i=0; i<old_marea.GetLength(0); i++){
				for(int j=0; j<old_marea.GetLength(1); j++){
					if(old_marea[i,j] < 0){
						continue;
					}
					if((i+j)%2 == 0){
						_mcBoard.DrawBlock(i,j,-2);
					}
					else{
						_mcBoard.DrawBlock(i,j,-1);
					}
				}
			}
		}
	}


	string IsTeamMate(Unit _mCube){
		if(_mCube == null){
			return "Empty";
		}
		return _mCube.team;
	}

	int IsGoodValue(int _state, int _top_value,int _compar_value){
		switch(_state){
			case Constants.S_ATK_ENEMY:
			case Constants.S_ATK_KING:
			case Constants.S_DEF_KING:
			case Constants.S_DEF_FRIEND:
			return IsSmallValue(_top_value, _compar_value);
			case Constants.S_GETWAY:
			return -1*IsSmallValue(_top_value, _compar_value);
			default:
			return IsSmallValue(_top_value, _compar_value);
		}
	}

	// 0=同じ, 1=大きい， -1=小さい
	int IsSmallValue(int _top_value,int _compar_value){
		if(_top_value == _compar_value){//同じならランダム
			return 0; 
		}
		else if(_top_value > _compar_value){
			return 1;
		}
		else{
			return -1;
		}
	}
	int SetTopValue(int _state){
		switch(_state){
			case Constants.S_ATK_ENEMY:
			case Constants.S_ATK_KING:
			case Constants.S_DEF_KING:
			case Constants.S_DEF_FRIEND:
			return 1000000;
			case Constants.S_GETWAY:
			return -100000;
			default:
			return 1000000;
		}
	}

	public void ViewArea(MapBoard _mcBoard){

		int top_value = SetTopValue(this.state);
		int[] next_position = new int[2]{xz_position[0],xz_position[1]};

		int[,] marea = _mcBoard.GetMoveArea(xz_position[0],xz_position[1], locomotion, this.team);
		old_marea = (int[,])marea.Clone();
		for(int i=0; i<marea.GetLength(0); i++){
			for(int j=0; j<marea.GetLength(1); j++){
				if(marea[i,j] >= 0){
					// 仲間なら飛ばす 自分なら飛ばさない
					if(IsTeamMate(_mcBoard.GetMapBoardUnit(i,j)) == this.team){
						continue;
					}
					_mcBoard.DrawBlock(i,j,marea[i,j]);
					int tmp_dist1 = GetManhattanDistance(new int[2]{i,j}, target_position);
					int tmp_dist2 = GetManhattanDistance(xz_position, new int[2]{i,j});
					int tmp_dist 	= tmp_dist1*6+tmp_dist2;
					if(IsGoodValue(this.state, top_value, tmp_dist) == 0 && Random.value <0.5f){
						top_value = tmp_dist;
						next_position = new int[2]{i,j};
					}
					else if(IsGoodValue(this.state, top_value, tmp_dist) == 1){
						top_value = tmp_dist;
						next_position = new int[2]{i,j};
					}
				}
			}
		}
		// もしどれも良くないなら
		if(top_value == SetTopValue(this.state)){
			next_position[0] = xz_position[0];
			next_position[1] = xz_position[1];
		}

		_mcBoard.SetMapBoard(xz_position[0],xz_position[1], null);
		_mcBoard.DrawBlock(next_position[0],next_position[1],4); // min_position = 目的地
		List<string> path = GetPath(marea, next_position[0], next_position[1]);
		if(path.Count > 0){
			float sum_time = 0.3f;
			float play_time = 0.5f;
			Vector3 now_position = this.gameObject.transform.position;
			Vector3[] path_array =  new Vector3[path.Count];
			for(int i=0; i<path.Count; i++){
				path_array[i] = now_position;
			}
			for(int i=0; i<path.Count; i++){
				if(i==0){
					path_array[i] += GetRoot(path[i]);
				}
				else{
					path_array[i] = path_array[i-1] + GetRoot(path[i]);
				}
				play_time += 0.2f;
			}
			_mcBoard.SetMapBoard(xz_position[0],xz_position[1], this);
			animator.SetInteger("animation",14);
			if(path_array.Length == 1){
				TemplateMove( path_array[0], play_time, sum_time, "Animated", _mcBoard, this.gameObject);
			}
			else{
				TemplateMoveArray( path_array, play_time, sum_time, "Animated", _mcBoard, this.gameObject);
			}
		}
		else{
			_mcBoard.SetMapBoard(xz_position[0],xz_position[1], this);
			Animated(_mcBoard);
		}
	}


	
	List<string> GetPath(int[,] _marea, int _target_x, int _target_z){
		List<string> tmp_path_text = new List<string>();
		int count = 0;
		int max = _marea[_target_x, _target_z];
		while(max < locomotion){
			count ++ ;
			if(count >2000){
				break;
			}
			int[,] arround = new int[4,2]{{1,0}, {-1,0}, {0,1}, {0,-1}};
			string[] arround_text = new string[4]{"左","右","下","上"};
			for(int i=0; i<arround.GetLength(0); i++){
				int tmp_x = _target_x + arround[i, 0];
				int tmp_z = _target_z + arround[i, 1];

				int value = _marea[tmp_x, tmp_z];
				if(value > max){
					max = value;
					tmp_path_text.Add(arround_text[i]);
					_target_x = tmp_x;
					_target_z = tmp_z;
				}
			}
		}
		tmp_path_text.Reverse();
		return tmp_path_text;// list[list.Count-1];
	}

	int GetManhattanDistance(int[] _start, int[] _end){
		return Mathf.Abs(_start[0] - _end[0]) + Mathf.Abs(_start[1] - _end[1]);
	}



	Vector3 GetRoot(string _direction){
		switch(_direction){
			case "上":
			this.xz_position[1]++;
			return Vector3.forward*Constants.BLOCK_SIZE;
			case "右":
			this.xz_position[0]++;
			return Vector3.right*Constants.BLOCK_SIZE;
			case "下":
			this.xz_position[1]--;
			return Vector3.back*Constants.BLOCK_SIZE;
			case "左":
			this.xz_position[0]--;
			return Vector3.left*Constants.BLOCK_SIZE;
			default:
			return Vector3.zero;
		}
	}



	void TemplateMoveArray(Vector3[] _movePath, float _time, float _delaytime, string _oncomplete, MapBoard _oncompleteparams, GameObject _oncompletetarget){
		Hashtable moveHash = new Hashtable(); // Hashtable
		moveHash.Add ("path", _movePath);
		moveHash.Add ("time", _time);
		moveHash.Add ("delay", _delaytime);
		moveHash.Add ("orienttopath", true);
		moveHash.Add ("easeType", "easeInOutQuad");//linearTween
		// moveHash.Add ("easeType", "linearTween");//linearTween
		if(_oncomplete != ""){
			moveHash.Add ("oncomplete", _oncomplete);
			moveHash.Add("oncompleteparams", _oncompleteparams);
			moveHash.Add ("oncompletetarget", _oncompletetarget); // オブジェクトを指定			
		}
		iTween.MoveTo (this.gameObject, moveHash);					// そのメソッドを指定s		
	}
	void TemplateMove(Vector3 _position, float _time, float _delaytime, string _oncomplete, MapBoard _oncompleteparams, GameObject _oncompletetarget){
		Hashtable moveHash = new Hashtable(); // Hashtable
		moveHash.Add ("position", _position);
		moveHash.Add ("time", _time);
		moveHash.Add ("delay", _delaytime);
		moveHash.Add ("orienttopath", true);
		moveHash.Add ("easeType", "easeInOutQuad");//linearTween
		// moveHash.Add ("easeType", "linearTween");//linearTween
		if(_oncomplete != ""){
			moveHash.Add ("oncomplete", _oncomplete);
			moveHash.Add("oncompleteparams", _oncompleteparams);
			moveHash.Add ("oncompletetarget", _oncompletetarget); // オブジェクトを指定			
		}
		iTween.MoveTo (this.gameObject, moveHash);					// そのメソッドを指定s		
	}

	void Animated(MapBoard _mcBoard){
		animator.SetInteger("animation",1);
		ViewAreaClear(_mcBoard);
		gameMaster.GetComponent<GameMaster>().SetATKEventBool();
	}
}
/*
どのアクションを動かすか決める。


*/


/*	
// 回転
iTween.RotateTo(this.gameObject, iTween.Hash(
	"x", 90,
	"time", time
));
*/	

/* 
 * --- iTween.Hash 説明 ---
 * position		: 変化量
 * time			: アニメーション完了までの時間
 * easeType 		: アニメーションの仕方(リファレンス参照)
 * oncomplete		: アニメーション終了時に呼ぶメソッド名
 * oncompletetarget 	: アニメーション終了時に呼ぶメソッドを受け取るオブジェクト
 * 
 * --- iTween 説明 ---
 * MoveTo 	: 現在の位置から指定の位置まで移動
 * 引数1 	: 動かしたいオブジェクト
 * 引数2 	: 動かす挙動を設定するテーブル
 */