using UnityEngine;
using System.Collections;

public class MapBoard : MonoBehaviour {
	public	GameObject[]	prefab_BL;		// 床ブロック格納用のプレファブ配列
	private	GameObject[,]	map_BL;			// マップに配置したブロックの格納用
	// private int				MAP_SIZE_X	= 20;	// マップ横幅のブロック数
	// private int				MAP_SIZE_Z	= 10;	// マップ奥幅のブロック数
	private	Vector3			BL_SIZE		= new Vector3(Constants.BLOCK_SIZE,Constants.BLOCK_SIZE,Constants.BLOCK_SIZE);		// ブロックのサイズ
	private GameObject block_folder;		// 作成したブロックを入れるフォルダー
	public int[,] map_Board;
	// public int[,] map_Board_Unit;
	public Unit[,] map_Board_Unit;
	public Material mate_BLACK;
	public Material mate_WHITE;
	public Material[] materials;
	/*
	public int[] MyKing;
	public int[] EnemyKing;
	*/
	// ■■■■■■
	void Awake(){
		block_folder = new GameObject();				// 作成したブロックを入れるフォルダー(空オブジェクト)を作成
		block_folder.name = "BL_Folder";						// 名前を変更.
		beginning_map_Board();
		beginning_BL_Arrangement();		// 初期ブロック配置
	}

	// ■■■■■■
	void Start () {
		// DrawBlock(1,1,mate_MOVE_AREA);
		// DrawBlock(1,2,mate_MOVE_POINT);
	}

	private void beginning_map_Board(){
		map_Board = new int[Constants.MAP_SIZE_X+2, Constants.MAP_SIZE_Z+2];
		map_Board_Unit = new Unit[Constants.MAP_SIZE_X+2, Constants.MAP_SIZE_Z+2];
		for(int x=0; x<map_Board.GetLength(0); x++){
			for(int z=0; z<map_Board.GetLength(1); z++){
				if(x==0 || x==Constants.MAP_SIZE_X+1 || z==0 || z == Constants.MAP_SIZE_Z+1){
					map_Board[x,z] = -100;
					map_Board_Unit[x,z] = null;
					continue;
				}
				map_Board[x,z] = -1;
				map_Board_Unit[x,z] = null;
				
				if(x==18 && z == 9){
					map_Board[x,z] = -10;
				}
			}			
		}
	}

	// ■■■初期ブロック配置■■■
	private void beginning_BL_Arrangement(){
		map_BL = new GameObject[Constants.MAP_SIZE_X+2 , Constants.MAP_SIZE_Z+2];	// MAP_SIZE分の配列を作成.
		
		int n = 0;		// X列のブロックを順の配置するための変数
		int m = 0;		// Z列のブロックを順の配置するための変数
		
		for(int z=0 ; z< map_BL.GetLength(1) ; z++){
			for(int x=0 ; x< map_BL.GetLength(0) ; x++){
				if(map_Board[x,z]==-100){
					continue;
				}
				Vector3 block_pos = new Vector3( BL_SIZE.x * x , -BL_SIZE.y / 2 , BL_SIZE.z * z);	// ブロック位置の算出
				GameObject block = Instantiate(prefab_BL[n] , block_pos , Quaternion.identity) as GameObject;		// プレハブ作成

				block.name = "BL[" + x + "," + z + "]";				// 作成したブロックの名前変更
				block.transform.parent = block_folder.transform;	// 作成したブロックの親、フォルダーにする

				map_BL[x,z] = block;						// 作成したブロックを、マップ配列に格納
				n = (n+1) % prefab_BL.Length;				// 次に作るブロックの番号
			}
			m = (m+1) % prefab_BL.Length;					// Z列の最初に来るブロックの番号
			n = m;
		}
	}
	public void DrawInitBlock(){
		int n = 0;		// X列のブロックを順の配置するための変数
		int m = 0;		// Z列のブロックを順の配置するための変数
		
		for(int z=0 ; z< map_BL.GetLength(1); z++){
			for(int x=0 ; x< map_BL.GetLength(0); x++){
				if(map_Board[x,z]==-100){
					continue;
				}
				Vector3 block_pos = new Vector3( BL_SIZE.x * x , -BL_SIZE.y / 2 , BL_SIZE.z * z);	// ブロック位置の算出
				GameObject block = Instantiate(prefab_BL[n] , block_pos , Quaternion.identity) as GameObject;		// プレハブ作成

				block.name = "BL[" + x + "," + z + "]";				// 作成したブロックの名前変更
				block.transform.parent = block_folder.transform;	// 作成したブロックの親、フォルダーにする

				map_BL[x,z] = block;						// 作成したブロックを、マップ配列に格納
				n = (n+1) % prefab_BL.Length;				// 次に作るブロックの番号
			}
			m = (m+1) % prefab_BL.Length;					// Z列の最初に来るブロックの番号
			n = m;
		}		
	}

	public void DrawBlock(int _x, int _z, int _num){
		if(_x<=0 || _z<=0 || Constants.MAP_SIZE_X < _x || Constants.MAP_SIZE_Z < _z){
			return;
		}
		Material _mate;
		if(_num == -1){
			_mate = mate_BLACK;
		}
		else if(_num == -2){
			_mate = mate_WHITE;
		}
		else{
			_mate = materials[_num];
		}
		GameObject tmp_block = map_BL[_x,_z];
		//iTween.ColorFrom(tmp_block, _mate.color, 1f);
		//iTween.ValueTo(tmp_block, _mate.color, 1f);
		tmp_block.GetComponent<Renderer>().material = _mate;
	}
	public int GetMapBoard(int _x, int _z){
		return map_Board[_x, _z];
	}
	public void SetMapBoard(int _x, int _z, Unit _mCube){
		map_Board_Unit[_x, _z] = _mCube;
		/*
		switch(_text){
			case "Empty":
			map_Board[_x, _z] = -1;
			map_Board_Unit[_x, _z] = null;
			break;
			case "Enemy":
			map_Board_Unit[_x, _z] = -1;
			break;
			case "My":
			map_Board_Unit[_x, _z] = 1;
			break;
			default:
			Debug.Log("地形設定エラー");
			break;
		}*/
	}
	public Unit GetMapBoardUnit(int _x, int _z){
		if(_x < 0 || Constants.MAP_SIZE_X +1 < _x || _z < 0 || Constants.MAP_SIZE_Z +1 < _z){
			return null;
		}
		return map_Board_Unit[_x, _z];
	}

	public int[,] GetMoveArea(int _x, int _y, int _locomotion, string _team){
		int[,] map_Board_Clone = (int[,])map_Board.Clone();
		// 初期を設定
		map_Board_Clone[_x, _y] = _locomotion;
		// 上下左右を調べる。
		Search4(ref map_Board_Clone, _x, _y, _locomotion, _team, "right");
		Search4(ref map_Board_Clone, _x, _y, _locomotion, _team, "left");
		Search4(ref map_Board_Clone, _x, _y, _locomotion, _team, "up");
		Search4(ref map_Board_Clone, _x, _y, _locomotion, _team, "down");
		return map_Board_Clone;
	}

	public void Search4(ref int[,] _clone_map, int _x, int _z, int _locomotion, string _team, string _text){
		int tmp_x = _x;
		int tmp_z = _z;
		switch(_text){
			case "up":
			tmp_z++;
			break;
			case "right":
			tmp_x ++;
			break;
			case "down":
			tmp_z--;
			break;
			case "left":
			tmp_x --;
			break;
			default:
			Debug.Log("Search4エラー");
			break;
		}
		Search(ref _clone_map, tmp_x, tmp_z, _locomotion, _team, _text);
	}
	public void Search(ref int[,] _clone_map, int _x, int _z,  int _locomotion, string _team, string _text){
		// 壁にぶつかるなら終了
		if(_clone_map[_x, _z] == -100){
			return;
		}
		// 味方なら地形データ，敵なら -10
		/*
		if(map_Board_Unit[_x, _z]){

		}*/
		int tmp_loc = _locomotion;

		// 自分の敵なら-10
		if(map_Board_Unit[_x, _z] == null){
			tmp_loc += map_Board[_x, _z]; // 地形の影響を与える
		}
		else if(map_Board_Unit[_x, _z].enemy_team == _team){
			tmp_loc -= 10;
		}
		else if(map_Board_Unit[_x, _z].team == _team){
			tmp_loc += map_Board[_x, _z];
		}
		// 登録したやつより大きければ登録し直す。
		if(tmp_loc > _clone_map[_x, _z]){
			_clone_map[_x, _z] = tmp_loc;
		}
		if(tmp_loc <= 0){
			return;
		}
		if(_text != "left"){
			Search4(ref _clone_map, _x, _z, tmp_loc, _team, "right");
		}
		if(_text != "right"){
			Search4(ref _clone_map, _x, _z, tmp_loc, _team, "left");
		}
		if(_text != "down"){
			Search4(ref _clone_map, _x, _z, tmp_loc, _team, "up");
		}
		if(_text != "up"){
			Search4(ref _clone_map, _x, _z, tmp_loc, _team, "down");
		}
	}
}
/*
public class Tile {
	GameObject block;
	string type;			// Red/Blue
	int cost; 				// 空：-1，Red：

	void Tile(){

	}
}
*/