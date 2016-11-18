using NCMB;
using System.Collections.Generic;

public class RatingBoard {

  public int currentRank = 0; // 自分のランク
  public List<NCMB.Rating> topRankers = null; // top5のランク
  public List<NCMB.Rating> neighbors  = null; // 近隣のランク

  // 現プレイヤーのRateを受けとってランクを取得 ---------------
  public void fetchRank( int currentScore )
  {
    // データスコアの「Connectors」から検索
    NCMBQuery<NCMBObject> rankQuery = new NCMBQuery<NCMBObject> ("ExpertData");
    rankQuery.WhereGreaterThan("Rating", currentScore);
    rankQuery.CountAsync((int count , NCMBException e )=>{

    if(e != null){
      //件数取得失敗
    }else{
      //件数取得成功
      currentRank = count+1; // 自分よりスコアが上の人がn人いたら自分はn+1位
    }
      });
  }

  // サーバーからトップ10を取得 ---------------    
  public void fetchTopRankers()
  {
    // データストアの「Connectors」クラスから検索
    NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject> ("ExpertData");
    query.OrderByDescending ("Rating");
    //query.Limit = 5;
    query.FindAsync ((List<NCMBObject> objList ,NCMBException e) => {

    if (e != null) {
      //検索失敗時の処理
    } else {
      //検索成功時の処理
      List<NCMB.Rating> list = new List<NCMB.Rating>();
      // 取得したレコードをConnectorsクラスとして保存
      foreach (NCMBObject obj in objList) {
        int    s = System.Convert.ToInt32(obj["Rating"]);
        string n = System.Convert.ToString(obj["Name"]);
        list.Add( new Rating( s, n ) );
      }
      topRankers = list;
    }
      });
  }

  // サーバーからrankの前後2件を取得 ---------------
  public void fetchNeighbors()
  {
    // neighbors = new List<NCMB.Rating>();

    // スキップする数を決める（ただし自分が1位か2位のときは調整する）
    int numSkip = currentRank - 3;
    if(numSkip < 0) numSkip = 0;

    // データストアの「Connectors」クラスから検索
    NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject> ("ExpertData");
    query.OrderByDescending ("Rating");
    query.Skip  = numSkip;
    query.Limit = 5;
    query.FindAsync ((List<NCMBObject> objList ,NCMBException e) => {

    if (e != null) {
      //検索失敗時の処理
    } else {
      //検索成功時の処理
      List<NCMB.Rating> list = new List<NCMB.Rating>();
      // 取得したレコードをConnectorsクラスとして保存
      foreach (NCMBObject obj in objList) {
        int    s = System.Convert.ToInt32(obj["Rating"]);
        string n = System.Convert.ToString(obj["Name"]);
        list.Add( new Rating( s, n ) );
      }
      neighbors = list;
    }
      });
  }
}