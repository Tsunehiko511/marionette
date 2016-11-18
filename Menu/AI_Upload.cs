using NCMB;
using System.Collections;
using System.Collections.Generic;

namespace NCMB{
	public class AI_Upload {
		public string Name { get; set; }
		public int rating	{ get; set; }
    public bool isSearch { get; set; }
    public bool isNoConnect { get; set; }
    public bool isSaved { get; set; }
    public bool isNewSaved { get; set; }
    public bool isFetched { get; set; }
    public bool isNoData { get; set; }

		public AI_Upload(string _name){
			Name = _name;
		}


		// 思考の保存
		public void save(){
      NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject> ("ExpertData");
      query.WhereEqualTo("Name", Name);
      query.FindAsync((List<NCMBObject> objList, NCMBException e) => {

      	// 検索成功したら
      	if(e == null){
          isSearch = true;
          // 未登録だったら
          if( objList.Count == 0 ){
            NCMBObject obj = new NCMBObject("ExpertData");
            obj["Name"]  = Name;
            obj["Rating"]  = 1500;
            int type_count = (MyConnects.connectLists).Count;
            List<int[]> tmp_list = new List<int[]>();
            for(int i=0 ; i< type_count; i++){
            	int countConect = (MyConnects.connectLists[i]).Count;
              if(countConect == 0){
                tmp_list.Add(new int[6]{0,0,0,0,0,0});
              }
              else{
                int[] tmp = new int[6*countConect];
                int idx = 0;
                // Debug.Log("countConect = "+countConect);
                foreach(int[] connect in MyConnects.connectLists[i]){
                  tmp[idx]   = connect[0];
                  tmp[idx+1] = connect[1];
                  tmp[idx+2] = connect[2];
                  tmp[idx+3] = connect[3];
                  tmp[idx+4] = connect[4];
                  tmp[idx+5] = connect[5];
                  idx = idx +6;
                }
                tmp_list.Add(tmp);
              }
            }
            obj["Connect"] = tmp_list;
            obj.SaveAsync();
            isNewSaved = true;
          } 
          else{
            int type_count = (MyConnects.connectLists).Count;
            List<int[]> tmp_list = new List<int[]>();
            for(int i=0 ; i< type_count; i++){
	            int countConect = MyConnects.connectLists[i].Count;
	            int[] tmp = new int[6*countConect];
	            int idx = 0;
	            // Debug.Log("countConect = "+countConect);

	            foreach(int[] connect in MyConnects.connectLists[i]){
	              tmp[idx]   = connect[0];
	              tmp[idx+1] = connect[1];
	              tmp[idx+2] = connect[2];
	              tmp[idx+3] = connect[3];
	              tmp[idx+4] = connect[4];
	              tmp[idx+5] = connect[5];
	              idx = idx +6;
	            }
	            /*
	            for(int i= 0; i<tmp.Length; i++){
	              // Debug.Log(tmp[i]);
	            }*/
	            // Debug.Log("objList = "+objList.Count);
	            tmp_list.Add(tmp);
	          }
	          objList[0]["Connect"] = tmp_list;
        		objList[0].SaveAsync();
        		isSaved = true;
        		isNoData = false;
          }
          // saver_alert.text = "サーバ連携　成功";
          /*          
          if(User.Level == 5){
            User.LevelUP();
          }*/
      	}
        else{
        	isSaved = false;
        	isNoData = true;
          // saver_alert.text = "サーバ連携　失敗";
        }
        //Invoke("SaverAlertOFF", 2);
      });
		}

		// サーバーから指定のAIを取得
		public void fetch(string _name){
      if(_name == ""){
        _name = Name;
      }
      // データストアの「HighScore」クラスから、Nameをキーにして検索
      NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject> ("ExpertData");
      query.WhereEqualTo ("Name", _name);
      query.FindAsync ((List<NCMBObject> objList ,NCMBException e) => {
      	//検索成功したら
	      if (e == null) {
	        // 対戦相手のデータがが未登録だったら
	        if( objList.Count == 0 ){
            isNoData = true;
            // Debug.Log("取得失敗");
          }
          // 対戦相手のデータが登録済みだったら
          else {
            int idx = 0;
            int s_id;
            int s_state;
            int s_position;
            int flag;
            int e_id;
            int e_state;

            foreach(NCMBObject obj in objList){
            	int type_count = ((ArrayList)obj["Connect"]).Count;
            	// 情報を消す
              Enemys.connectLists.Clear();
              Enemys.name = _name;

              for(int i=0; i<type_count; i++){
                int count = ((ArrayList)((ArrayList)obj["Connect"])[i]).Count;
	              List<int[]> tmp_connectLists = new List<int[]>();
	              var tmp_data = (ArrayList)(((ArrayList)obj["Connect"])[i]);
	              for(int j=0; j<count; j = j +6){
			            s_id    		= (int)System.Convert.ToInt32(tmp_data[j]);
			            s_state 		= (int)System.Convert.ToInt32(tmp_data[j+1]);
	                s_position 	= (int)System.Convert.ToInt32(tmp_data[j+2]);
	                flag    		= (int)System.Convert.ToInt32(tmp_data[j+3]);
	                e_id    		= (int)System.Convert.ToInt32(tmp_data[j+4]);
	                e_state 		= (int)System.Convert.ToInt32(tmp_data[j+5]);
			            tmp_connectLists.Add(new int[6]{s_id, s_state, s_position, flag, e_id, e_state});
	            	}
	            	// 追加
	            	Enemys.connectLists.Add(tmp_connectLists);
              }
              int ai_count = Enemys.connectLists.Count;
              for(int i = ai_count; i<NodeArrow.ai_type; i++){
                Enemys.connectLists.Add(Enemys.connectLists[ai_count-1]);
              }
            }
            // Debug.Log("取得成功");
            isFetched = true;
          }
        }
      });
    }
	}
}