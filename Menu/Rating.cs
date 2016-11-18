using NCMB;
using System.Collections.Generic;

namespace NCMB
{
  public class Rating
  {
    public int rate   { get; set; }
    public string name { get; private set; }
    public bool isSaved{ get; private set; }
    public bool isFetched{ get; private set; }
    public bool isNoData{ get; private set; }
    
    // コンストラクタ -----------------------------------
    public Rating(int _rate, string _name)
    {
      rate = _rate;
      name  = _name;
    }

    // サーバーにRateを保存 -------------------------
    public void save()
    {
      // データストアの「HighScore」クラスから、Nameをキーにして検索
      NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject> ("ExpertData");
      query.WhereEqualTo ("Name", name);
      query.FindAsync ((List<NCMBObject> objList ,NCMBException e) => {

        //検索成功したら    
        if (e == null) {
          objList[0]["Rating"] = rate;
          objList[0].SaveAsync();
          isSaved = true;
        }
      });
    }

    // サーバーからRateを取得  -----------------
    public void fetch()
    {
      // データストアの「Connectors」クラスから、Nameをキーにして検索
      NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject> ("ExpertData");
      query.WhereEqualTo ("Name", name);
      query.FindAsync ((List<NCMBObject> objList ,NCMBException e) => {

      //検索成功したら  
      if (e == null) {
        // Rateが未登録だったら
        if( objList.Count == 0 ){
          isNoData = true;
            /*
            NCMBObject obj = new NCMBObject("Connectors");
            obj["Name"]  = name;
            obj["Rating"] = 1500;
            obj.SaveAsync();
            rate = 1500;
            */
            // Debug.Log("未登録");
          } 
          // Rateが登録済みだったら
          else {
            rate = System.Convert.ToInt32( objList[0]["Rating"] );
            isFetched = true;
          }
        }
      });
    }
    public string print(){
       return name + ':' + rate;
    }    
  }
}