using UnityEngine;
// using UnityEditor;
using System;
using System.Text;
using System.Collections.Generic;
using System.Collections;

public class NodeArrow : MonoBehaviour {
    public static int current_type = 0;
    int old_type = 0;
    public static List<Dictionary<int, Node>> nodes = new List<Dictionary<int, Node>>();
    public static int ai_type = 4;
    public static Base64_Converter B64_Converter;
    List<int> keyList;
    int NodeNumer = 0;
    int NodeElement = 0;
    
    public static PanelSliderLow ps;
    public static PanelSliderLow cs;
    
    void Start(){
        ps = GameObject.Find("Screen/Panels/Panel/StatePanel").GetComponent<PanelSliderLow>();
        cs = GameObject.Find("Screen/Panels/Panel/ConditionPanel").GetComponent<PanelSliderLow>();
        B64_Converter = new Base64_Converter();
        current_type = 0;
        Initial();
    }


    public static void Initial(){
        // PlayerPrefs.DeleteAll();
        // ここで全てのデータを取得する
        NodeId.Init();
        Init();
        ConnectorManager.Init();
    }

    public static void Init(){
        current_type = 0;
        if(nodes.Count == ai_type){
            return;
        }
        for(int i=0; i<ai_type; i++){
            nodes.Add(new Dictionary<int, Node>());
        }
    }


    void OnGUI(){
        if(old_type != current_type){
            old_type = current_type;
            Saved();
            // Initial();
        }
        // ここで今のデータを表示する
        keyList = new List<int>(nodes[current_type].Keys);
        foreach(int key in keyList){
            nodes[current_type][key].Update();
        }
        var ev = Event.current;
        if (ConnectorManager.HasCurrent && ev.type == EventType.mouseDown && ev.button == 1){
            ConnectorManager.CancelConnecting();
        }
        ConnectorManager.Update();
    }

    public void AddState(){
        List<int> conditions = new List<int>();
        Node _node = new Node(NodeId.Create(), 310, 20, 1, Constants.S_INIT, conditions);
        nodes[current_type].Add(_node.id, _node);
    }
    public void Delete(int id){
        NodeId.Delete(id);
        nodes[current_type].Remove(id);
    }
    public void SetStateConditions(string state_conditions){
        nodes[current_type][NodeNumer].SetStateConditions(state_conditions, NodeElement);
    }
    public void SetNodeNumer(int number, int element){
        NodeNumer = number;
        NodeElement = element;
    }


    public static void Saved(){

        MyConnects.connectLists.Clear();
        for(int i=0; i<NodeArrow.ai_type; i++){

            /*SetDataNodes(nodes[i], i); TODO
            SaveDatas.SetDataListConnector("Connects", ConnectorManager.connectors[i], i);//TODO??*/
            ConnectorManager.SetConnectListFrom(ConnectorManager.connectors[i], i);
            MyConnects.connectLists.Add(ConnectorManager.connectLists[i]);
        }
        Enemys.connectLists.Clear();
        for(int i=0; i<NodeArrow.ai_type; i++){

            /*SetDataNodes(nodes[i], i); TODO
            SaveDatas.SetDataListConnector("Connects", ConnectorManager.connectors[i], i);//TODO??*/
            // ConnectorManager.SetConnectListFrom(ConnectorManager.connectors[i], i);
            Enemys.connectLists.Add(ConnectorManager.connectLists[i]);
        }


        // PlayerPrefs.Save();
    }
    public  void ClearData(){
        nodes[current_type].Clear();
        NodeId.DeleteAll(current_type);
        ConnectorManager.DeleteData();
        /*NodeId.Init();        
        Init();
        ConnectorManager.Init();*/
        // PlayerPrefs.DeleteAll();
    }

    public string GetBase64(){
        string tmp_string = "";
        tmp_string += B64_Converter.GetMembaTo2digit(NodeArrow.nodes[current_type].Count, Constants.NODE_COUNT_DIGIT);
        foreach(KeyValuePair<int, Node> _node in NodeArrow.nodes[current_type]){
            tmp_string += B64_Converter.GetNodeTo2digit(_node.Value);            
        }
        return tmp_string;
    }
    public void SetBase64(ref string _inputText){
        nodes[current_type].Clear();
        int node_count = B64_Converter.Get2digitToMemba(ref _inputText, Constants.NODE_COUNT_DIGIT);
        for(int i=0; i<node_count; i++){
            Node _node = B64_Converter.Get2digitToNode(ref _inputText);
            nodes[current_type].Add(_node.Id, _node);
            NodeId.SetSlot(_node.Id, current_type);
        }
    }
}
// MDAxMTAwMDAwMDAwMTAxMDAxMTAxMTAwMDAwMDEwMTAwMDAwMTAwMDAxMDAwMDEwMDAwMDAwMTAwMTAxMDAwMTAwMDEwMDEwMTAwMDAwMDEwMDEwMTEwMDAwMDAwMDAwMDAwMDAwMDAwMDExMTAwMTAwMDAxMDAwMDEwMTAwMDEwMDAxMDAwMTEwMDAxMDAwMDAxMDAwMDEwMDAxMTAwMDAwMDAwMTEwMDAwMTAwMDEwMDAwMDAwMDAxMDAwMDAxMDAwMTEwMDAwMA==

public static class NodeId{
    const int MAX = 30; 
    static List<int> id = new List<int>();
    static List<bool[]> slot = new List<bool[]>();

    public static void Init(){
        if(slot.Count == NodeArrow.ai_type){
            return;
        }

        for(int i=0; i<NodeArrow.ai_type; i++){
            id.Add(1);
            slot.Add(new bool[MAX]);
        }
    }

    public static int Create(){
        for(int i=0; i<MAX; i++){
            if(!slot[NodeArrow.current_type][i]){
                slot[NodeArrow.current_type][i] = true;
                return i+1;
            }
        }
        slot[NodeArrow.current_type][id[NodeArrow.current_type]-1] = true;
        return id[NodeArrow.current_type]++;
    }

    public static void SetSlot(int position, int _type){
        // Debug.Log("position"+position+":"+_type);
        slot[_type][position-1] = true;
        id[_type] = position;
        // id[_type] = position + 1;
    }

    public static void Delete(int _id){
        id[NodeArrow.current_type] = 1;
        slot[NodeArrow.current_type][_id-1] = false;
    }
    public static void DeleteAll(int _type){
        id[_type] = 1;
        for(int i=0; i<MAX; i++){
            slot[_type][i] = false;
        }
    }
}

    /*
    ・Nodeデータ全て
        ・int _type
        ・int id
        ・Vector2 position
        ・int clickCount //初期値1 クリックしたら+1
        ・string state
        ・List<string> conditions)// 条件の数=clickCount-1

    */

    /*
    InputField を作って
    ・データを全て２進数にする
    ・入れるデータは二進数
    ・StringToBase64()に入れる


    Base64ToString()に入れる
    */
public class Base64_Converter{
    // Nodeをbase64用２進数へ変換
    public string GetNodeTo2digit(Node _node){
        // string tmp_2digit = GetNodeDataTo2digit(_node.type, _node.id, _node.position_x, _node.position_y, _node.clickCount, _node.state, _node.conditions);
        string tmp_2digit = GetNodeDataTo2digit(_node.id, _node.position_x, _node.position_y, _node.clickCount, _node.state, _node.conditions);
        // Debug.Log("GetNodeTo2digit");
        return tmp_2digit;
    }   
    // public string GetNodeDataTo2digit(int _type, int _id, int _position_x, int _position_y, int _clickCount, int _state, List<int> _conditions){
    public string GetNodeDataTo2digit(int _id, int _position_x, int _position_y, int _clickCount, int _state, List<int> _conditions){
        string tmp_base64 = "";        
        // tmp_base64 += GetMembaTo2digit(_type, Constants.NODE_TYPE_DIGIT);
        tmp_base64 += GetMembaTo2digit(_id, Constants.NODE_ID_DIGIT);
        tmp_base64 += GetMembaTo2digit(_position_x, Constants.NODE_POSITION_X_DIGIT);
        tmp_base64 += GetMembaTo2digit(_position_y, Constants.NODE_POSITION_Y_DIGIT);
        tmp_base64 += GetMembaTo2digit(_clickCount, Constants.NODE_CLICK_DIGIT);
        tmp_base64 += GetMembaTo2digit(_state, Constants.NODE_STATE_DIGIT);
        for(int i=0; i<_clickCount-1;i++){
            tmp_base64 += GetMembaTo2digit(Constants.GetConditionIntToID(_conditions[i]), Constants.NODE_CONDITION_DIGIT);
        }
        // Debug.Log("GetNodeDataTo2digit");

        return tmp_base64;
    }
    public string GetMembaTo2digit(int _num, int _digit){
        string numString = Convert.ToString(_num, 2);
        numString = numString.PadLeft(_digit, '0');
        // Debug.Log(_num+":"+numString+":"+_digit);
        return numString;
    }


    // base64の2進数をNodeheへ変換
    public Node Get2digitToNode(ref string _2digit){
        // 分割して取得
        // int tmp_type        = Get2digitToMemba(ref _2digit, Constants.NODE_TYPE_DIGIT);
        int tmp_id          = Get2digitToMemba(ref _2digit, Constants.NODE_ID_DIGIT);
        int tmp_positon_x   = Get2digitToMemba(ref _2digit, Constants.NODE_POSITION_X_DIGIT);
        int tmp_positon_y   = Get2digitToMemba(ref _2digit, Constants.NODE_POSITION_Y_DIGIT);
        int tmp_clickCount  = Get2digitToMemba(ref _2digit, Constants.NODE_CLICK_DIGIT);
        int tmp_state       = Get2digitToMemba(ref _2digit, Constants.NODE_STATE_DIGIT);
        List<int> tmp_conditions = new List<int>();
        for(int i=0; i<tmp_clickCount-1; i++){
            tmp_conditions.Add(Constants.GetConditionIDToInt(Get2digitToMemba(ref _2digit, Constants.NODE_CONDITION_DIGIT)));
        }
        return new Node(tmp_id, tmp_positon_x, tmp_positon_y, tmp_clickCount, tmp_state, tmp_conditions);
    }    
    public int Get2digitToMemba(ref string _2digit, int _digit){
        string numString = _2digit.Substring(0, _digit);
        _2digit = _2digit.Remove(0, _digit);
        int _num = Convert.ToInt32(numString, 2);
        // Debug.Log(_num+":"+numString+":"+_digit);
        return _num;
    }



    public Connector Get2digitToConnector(ref string _2digit){
        Connector tmp_connector = new Connector();

        int tmp_startId         = Get2digitToMemba(ref _2digit, Constants.NODE_ID_DIGIT);
        int tmp_startPositon    = Get2digitToMemba(ref _2digit, Constants.NODE_CONNECT_POSITION);
        int tmp_endId           = Get2digitToMemba(ref _2digit, Constants.NODE_ID_DIGIT);
        int tmp_endPositon      = Get2digitToMemba(ref _2digit, Constants.NODE_CONNECT_POSITION);
        Dictionary<int, Node> _nodes = NodeArrow.nodes[NodeArrow.current_type];

        List<int> keyList = new List<int>(_nodes.Keys);
        foreach(int key in keyList){
            // Debug.Log("key"+key);
        }
        /*

        Debug.Log("nodes:"+_nodes.Count);
        Debug.Log("tmp_startId:"+tmp_startId+", tmp_startPositon"+tmp_startPositon);
        Debug.Log("tmp_endId:"+tmp_endId +", tmp_endPositon:"+tmp_endPositon);
        */
        tmp_connector.Init(_nodes[tmp_startId], tmp_startPositon, _nodes[tmp_endId], tmp_endPositon);
        return tmp_connector;
    }
    public string GetConnectorTo2digit(Connector _connector){
        /*Debug.Log("S ID:"+_connector.StartNode.Id);
        Debug.Log("S P:"+_connector.StartPosition);
        Debug.Log("E ID:"+_connector.EndNode.Id);
        Debug.Log("E P:"+_connector.EndPosition);
        */
        string tmp_base64 = "";

        tmp_base64 += GetMembaTo2digit(_connector.StartNode.Id, Constants.NODE_ID_DIGIT);
        tmp_base64 += GetMembaTo2digit(_connector.StartPosition, Constants.NODE_CONNECT_POSITION);
        tmp_base64 += GetMembaTo2digit(_connector.EndNode.Id, Constants.NODE_ID_DIGIT);
        tmp_base64 += GetMembaTo2digit(_connector.EndPosition, Constants.NODE_CONNECT_POSITION);
        return tmp_base64;
    }
}



public class Node : MonoBehaviour{
    // public int type;                                    // 16種類     4桁
    public int id;                                      // 32        5
    public int position_x;                              // 1024      10
    public int position_y;                              //           10
    public int clickCount;                              //           5
    public int state;                                   //           4
    public List<int> conditions;                        //           5

    public Rect rect;

    int hgt = 80;
    int width = 150;

    public int Id{ get { return id; } }
    // public Vector2 Position{ get { return position; } }
    public int ClickCount{ get { return clickCount; } }
    public int State{ get { return state; } }
    public List<int> Conditions{ get { return conditions; } }
    public int Height{ set { hgt = value; } }
    public Rect Rect { get { return rect; } }

    bool isStateClick = false;
    bool isConditionClick = false;

    // public Node(int _type, int _id, int _position_x, int _position_y, int _clickCount, int _state, List<int> _conditions){
    public Node(int _id, int _position_x, int _position_y, int _clickCount, int _state, List<int> _conditions){
        // this.type = _type;
        this.id = _id;
        this.position_x = _position_x;
        this.position_y = _position_y;
        this.clickCount = _clickCount;
        this.state = _state;
        this.conditions = _conditions;
        this.rect = new Rect(_position_x, _position_y, width, hgt + 25*(_clickCount-1));
    }

    public void SetStateConditions(string state_conditions, int element){
        if(element == 0){
            this.state = Constants.GetStatusInt(state_conditions);
        }
        else{
            this.conditions[element-1] = Constants.GetConditionInt(state_conditions);
        }
    }

    public void Update(){
        position_x = (int)(rect.x);
        position_y = (int)(rect.y);
        
        rect = GUI.Window(id, rect, WindowCallback, String.Empty);
        Vector2 btnRect = new Vector2(25, 20);
        if (GUI.Button(new Rect(rect.x + rect.width - btnRect.x-3, rect.y - btnRect.y, btnRect.x, btnRect.y), "×")){
            NodeArrow.cs.SlideOut();
            NodeArrow.ps.SlideOut();

            // 曲線削除
            for(int i=0; i<clickCount; i++){
                if (ConnectorManager.IsConnected(this, i)){
                    ConnectorManager.Disconnect(this, i);
                }               
            }

            // Node削除   
            GameObject.Find("Node").GetComponent<NodeArrow>().Delete(this.id);
        }


        for(int element=0; element<clickCount; element++){
            if (ConnectorManager.HasCurrent){
                // 決定中の接続がある場合は始点となっている場合, 既に接続済みである場合に非アクティブ
                // GUI.enabled = !ConnectorManager.IsConnected(this, position) && !ConnectorManager.IsCurrent(this, position);
                GUI.enabled = !ConnectorManager.IsCurrent(this, element);
                if(element ==0){
                    if (GUI.Button(new Rect(rect.x - btnRect.x, rect.y + 40 + element*25, btnRect.x, btnRect.y), ">")){
                        NodeArrow.cs.SlideOut();
                        NodeArrow.ps.SlideOut();
                        ConnectorManager.Connect(this, element);
                    }
                    /*
                    if (GUI.Button(new Rect(rect.x + rect.width , rect.y + 40 + position*25, btnRect.x, btnRect.y), "<")){
                        Debug.Log("終点");
                        // クリックされたら接続
                        ConnectorManager.Connect(this, position);
                    }*/
                }
                else{
                    if (GUI.Button(new Rect(rect.x + rect.width, rect.y + 40 + element*25, btnRect.x, btnRect.y), ">")){
                        NodeArrow.cs.SlideOut();
                        NodeArrow.ps.SlideOut();
                        // クリックされたら取得
                        // 直前の始点と終点を結ぶ
                        ConnectorManager.StartConnecting(this, element);
                        // ConnectorManager.Connect(this, position);
                    }
                }
                GUI.enabled = true;
            }
            else{
                if(element ==0){
                    if (GUI.Button(new Rect(rect.x - btnRect.x, rect.y + 40 + element*25, btnRect.x, btnRect.y), ">")){
                        NodeArrow.cs.SlideOut();
                        NodeArrow.ps.SlideOut();
                        if (ConnectorManager.IsConnected(this, element)){
                          ConnectorManager.Disconnect(this, element);
                        }
                        else{
                            // Debug.Log("StartConnecting");
                            //ConnectorManager.Connect(this, position);
                          // ConnectorManager.StartConnecting(this, position);
                        }
                    }
                                /*
                                if (GUI.Button(new Rect(rect.x + rect.width , rect.y + 40 + position*25, btnRect.x, btnRect.y), "<")){
                    if (ConnectorManager.IsConnected(this, position))
                    {
                        Debug.Log("Disconnect");
                      ConnectorManager.Disconnect(this, position);
                    }
                    else
                    {
                        Debug.Log("StartConnecting");
                        //ConnectorManager.Connect(this, position);
                      // ConnectorManager.StartConnecting(this, position);
                    }
                                }*/
                }
                else{
                    if (GUI.Button(new Rect(rect.x + rect.width, rect.y + 40 + element*25, btnRect.x, btnRect.y), ">")){
                        NodeArrow.cs.SlideOut();
                        NodeArrow.ps.SlideOut();
                        if (ConnectorManager.IsConnected(this, element)){
                            ConnectorManager.Disconnect(this, element);
                        }
                        else{
                            ConnectorManager.StartConnecting(this, element);
                        }
                    }
                }
            }
        }
    }

    void WindowCallback(int id){

        // コンテンツ
        if(GUI.Button(new Rect(3, 3, this.rect.width/2 -3, 20), "-") &&  clickCount > 1){
            NodeArrow.cs.SlideOut();
            NodeArrow.ps.SlideOut();
            minusClick();
        }
        // コンテンツ
        if(GUI.Button(new Rect(this.rect.width - this.rect.width/2, 3, this.rect.width/2 -3, 20), "+") && clickCount > 0){
            NodeArrow.cs.SlideOut();
            NodeArrow.ps.SlideOut();
            plusClick();                        
        }
         GUILayout.Space(20);
        if( GUILayout.Button( Constants.GetStatusString(this.state) ) ){
            NodeArrow.cs.SlideOut();
            NodeArrow.ps.SlideIn();
            GameObject.Find("Node").GetComponent<NodeArrow>().SetNodeNumer(this.id, 0);
        }
        // 状態と条件
        for(int element=1; element<clickCount; element++){
            if( GUILayout.Button( Constants.GetConditionString(conditions[element-1]) ) ){
                // PanelSliderLow ps = GameObject.Find("Screen/Panels/Panel"+ NodeArrow.current_type +"/StatePanel").GetComponent<PanelSliderLow>();
                // PanelSliderLow cs = GameObject.Find("Screen/Panels/Panel"+ NodeArrow.current_type +"/ConditionPanel").GetComponent<PanelSliderLow>();
                NodeArrow.ps.SlideOut();
                NodeArrow.cs.SlideIn();
                GameObject.Find("Node").GetComponent<NodeArrow>().SetNodeNumer(this.id, element);
            }
        }
        // ドラッグ可能
        OnGUI();
        GUI.DragWindow();
    }

    void plusClick(){
        hgt = 80 + 25*(clickCount);
        this.rect = new Rect(this.rect.x, this.rect.y, width, hgt);
        conditions.Add(Constants.F_INIT);
        clickCount ++;
    }
    void minusClick(){
        clickCount--;
        hgt = 80 + 25*(clickCount-1);
        this.rect = new Rect(this.rect.x, this.rect.y, width, hgt);
        conditions.RemoveAt(clickCount-1);
        if (ConnectorManager.IsConnected(this, clickCount))
        {
          ConnectorManager.Disconnect(this, clickCount);
        }
    }

    void OnGUI(){

    }
    // abstract protected void OnGUI();

}

/*
public static class MyConnects{
    public static List<List<int []>> connectLists  = new List<List<int[]>>(); // 接続リスト
}
public static class Enemys{
    public static List<List<int []>> connectLists  = new List<List<int[]>>(); // 接続リスト
}
*/

/// <summary>
/// ノードの接続を管理するクラス
/// </summary>
public static class ConnectorManager{
    public static List<List<Connector>> connectors = new List<List<Connector>>(); // 保存データ
    public static List<List<int []>> connectLists  = new List<List<int[]>>(); // 接続リスト
    static List<Dictionary<int, Dictionary<int, List<Connector>>>> connected = new List<Dictionary<int, Dictionary<int, List<Connector>>>>();     // 接続したリスト [始点ID,[,]]
    public static List<Connector> current = new List<Connector>();

    public static bool HasCurrent{
        get{
            return current[NodeArrow.current_type] != null; 
        }
    }


    public static void Init(){
        // codeの数はユーザレベルで変化する
        /*
        connectors.Clear();
        connectLists.Clear();
        connected.Clear();
        current.Clear();
        */
        if(connectors.Count == NodeArrow.ai_type){
            return;
        }
        for(int i=0; i<NodeArrow.ai_type; i++){
            connectors.Add(GetConnectors(i));
            connectLists.Add(new List<int[]>());
            if(i == NodeArrow.current_type){
                SetConnectListFrom(connectors[i], i);
            }
            connected.Add(new Dictionary<int, Dictionary<int, List<Connector>>>());
            current.Add(null);
            if(i == NodeArrow.current_type){
                for(int j = connectors[i].Count-1; j>=0; j--){
                    StartConnecting(connectors[i][j].StartNode, connectors[i][j].StartPosition);
                    Connect(connectors[i][j].EndNode, connectors[i][j].EndPosition);
                }
            }
        }
    }

    public static void SetBase64(ref string _inputText){
        connectors[NodeArrow.current_type].Clear();
        while(_inputText != ""){
            Connector _connector = NodeArrow.B64_Converter.Get2digitToConnector(ref _inputText);
            connectors[NodeArrow.current_type].Add(_connector);
        }

        connectLists[NodeArrow.current_type].Clear();
        SetConnectListFrom(connectors[NodeArrow.current_type], NodeArrow.current_type);
        // connected.Add(new Dictionary<int, Dictionary<int, List<Connector>>>());
        connected[NodeArrow.current_type].Clear();//Add(new Dictionary<int, Dictionary<int, List<Connector>>>());
        current[NodeArrow.current_type] = null;
        for(int j = connectors[NodeArrow.current_type].Count-1; j>=0; j--){
            StartConnecting(connectors[NodeArrow.current_type][j].StartNode, connectors[NodeArrow.current_type][j].StartPosition);
            Connect(connectors[NodeArrow.current_type][j].EndNode, connectors[NodeArrow.current_type][j].EndPosition);
        }
    }
    public static string GetBase64(){
        string tmp_string = "";

        foreach(Connector _connector in connectors[NodeArrow.current_type]){
            tmp_string += NodeArrow.B64_Converter.GetConnectorTo2digit(_connector);            
        }
        return tmp_string;
    }
    /// <summary>
    /// あるノードを始点にして接続を作成
    /// </summary>
    /// <param name="startNode">始点となるノード</param>
    /// <param name="startPosition">ノードの接点の位置</param>
    public static void StartConnecting(Node startNode, int startPosition){
        if (current[NodeArrow.current_type] != null){
            throw new UnityException("Already started connecting.");
        }

        if (connected[NodeArrow.current_type].ContainsKey(startNode.Id) && connected[NodeArrow.current_type][startNode.Id].ContainsKey(startPosition)){
            return;
            // throw new UnityException("Already connected node.");
        }
        current[NodeArrow.current_type] = new Connector();// ここ怪しいね
        current[NodeArrow.current_type].SetStartNode(startNode);
        current[NodeArrow.current_type].SetStartPosition(startPosition);
    }

    public static void CancelConnecting(){
        current[NodeArrow.current_type] = null;
    }
    public static void DeleteData(){
        connectors[NodeArrow.current_type].Clear();
        connectLists[NodeArrow.current_type].Clear(); // 接続リスト
        connected[NodeArrow.current_type].Clear();     // 接続したリスト [始点ID,[,]]
        current[NodeArrow.current_type] = null;
    }

    public static bool IsCurrent(Node node, int position){
        return HasCurrent && current[NodeArrow.current_type].StartNode.Id == node.Id && current[NodeArrow.current_type].StartPosition == position;
    }

    /// <summary>
    /// 終点となるノードを決定
    /// </summary>
    /// <param name="endNode">終点となるノード</param>
    /// <param name="endPosition">ノードの接点の位置</param>
    //public static void Connect(Node endNode, int endPosition)
    public static void Connect(Node endNode, int position){
        if (current[NodeArrow.current_type] == null){
            // 接続設定中の曲線がない
            return;
            throw new UnityException("No current connector.");
        }

        current[NodeArrow.current_type].Connect(endNode, position);                                 // 始点と終点を結ぶ
        connectors[NodeArrow.current_type].Add(current[NodeArrow.current_type]);

        if (!connected[NodeArrow.current_type].ContainsKey(current[NodeArrow.current_type].StartNode.Id)){
            connected[NodeArrow.current_type][current[NodeArrow.current_type].StartNode.Id] = new Dictionary<int, List<Connector>>();
        }
        List<Connector> list = new List<Connector>();
        list.Add(current[NodeArrow.current_type]);
        connected[NodeArrow.current_type][current[NodeArrow.current_type].StartNode.Id].Add(current[NodeArrow.current_type].StartPosition, list);

        if (!connected[NodeArrow.current_type].ContainsKey(current[NodeArrow.current_type].EndNode.Id)){
            connected[NodeArrow.current_type][current[NodeArrow.current_type].EndNode.Id] = new Dictionary<int, List<Connector>>();
        }


        if(connected[NodeArrow.current_type][current[NodeArrow.current_type].EndNode.Id].ContainsKey(current[NodeArrow.current_type].EndPosition)){
            connected[NodeArrow.current_type][current[NodeArrow.current_type].EndNode.Id][current[NodeArrow.current_type].EndPosition].Add(current[NodeArrow.current_type]); // ここかー！！
        }
        else{
            connected[NodeArrow.current_type][current[NodeArrow.current_type].EndNode.Id].Add(current[NodeArrow.current_type].EndPosition, list);
        }

        // 終点が決定したので設定中の曲線をnullにする
        current[NodeArrow.current_type] = null;
    }

    /// <summary>
    /// あるノードの接続点に接続されている接続を返します
    /// </summary>
    /// <param name="node">ノード</param>
    /// <param name="position">接続点の位置</param>
    /// <returns>接続. 接続されていない場合はnull</returns>
    public static List<Connector> GetConnector(Node node, int position){
        if (connected[NodeArrow.current_type].ContainsKey(node.Id) && connected[NodeArrow.current_type][node.Id].ContainsKey(position)){
            return connected[NodeArrow.current_type][node.Id][position];
        }
        else{
            return null;
        }
    }

    public static bool IsConnected(Node node, int position){
        return GetConnector(node, position) != null;
    }

    /// <summary>
    /// ある接続点に接続されている接続を解除します
    /// </summary>
    /// <param name="node">始点若しくは終点として接続されているノード</param>
    /// <param name="position">接続点の位置</param>
    public static void Disconnect(Node node, int position){
        // start Node Id と startPosition からstartを削除
        // end Node Id と endPosition　からendを削除

        var con = GetConnector(node, position);  // node positionと繋がっている　node position
        if (con == null){
            return;
        }
        // RemoveAt(i)すると要素数が変わるのでfor文に注意
        for (int i = connectors[NodeArrow.current_type].Count - 1; i >= 0; i--){
            var other = connectors[NodeArrow.current_type][i];
            bool connectors_deleted = false;
            for(int j=0; j<con.Count; j++){
                if( !((con[j].StartNode.Id == node.Id && con[j].StartPosition == position) 
                    ||(con[j].EndNode.Id == node.Id && con[j].EndPosition == position)) ){
                    continue;
                }
                if (con[j].StartNode.Id == other.StartNode.Id && con[j].StartPosition == other.StartPosition &&
                    con[j].EndNode.Id == other.EndNode.Id && con[j].EndPosition == other.EndPosition){
                    if(!connectors_deleted){
                        connectors[NodeArrow.current_type].RemoveAt(i);
                        connectors_deleted = true;
                    }

                    connected[NodeArrow.current_type][con[j].StartNode.Id].Remove(con[j].StartPosition);
                    // startPosition がなければEndPositionを削除
                    if(connected[NodeArrow.current_type][con[j].StartNode.Id].Count == 0){
                        connected[NodeArrow.current_type][con[j].EndNode.Id].Remove(con[j].EndPosition);
                    }
                }               
            }
        }
    }
    public static List<Connector> GetConnectors(int _type){
        return new List<Connector>();// SaveDatas.GetDataListConnector("Connects", _type);
    }

    public static void SetConnectListFrom(List<Connector> _connectors, int _type){
        // idの小さい順　idが同じならpositionが小さい順
        connectLists[_type].Clear();

        foreach (Connector connect in _connectors){
            int add_position = 0;
            int [] add_list = new int [6];
            // 追加するデータを取得
            int ins_StartID    = connect.StartNode.Id;
            int ins_StartState = connect.StartNode.State;
            int ins_StartPosition    = connect.StartPosition;
            int ins_StartFlag  = connect.StartNode.Conditions[connect.StartPosition-1];
            int ins_endID      = connect.EndNode.Id;
            int ins_endState   = connect.EndNode.State;
            bool IsInsert = true;
        
            add_list[0] = ins_StartID;
            add_list[1] = ins_StartState;
            add_list[2] = ins_StartPosition;
            add_list[3] = ins_StartFlag;
            add_list[4] = ins_endID;
            add_list[5] = ins_endState;

            foreach(int[] list in connectLists[_type]){
                // 比較する既存のデータを取得
                int data_startID        = list[0];
                int data_startState     = list[1];
                int data_startPosition  = list[2];
                int data_startFlag      = list[3];
                int data_endID          = list[4];
                int data_endState       = list[5];

                // すでに同じデータがあるなら終了
                if( (ins_StartID == data_startID)
                &&  (ins_StartPosition == data_startPosition)){
                    IsInsert = false;
                    break;
                }

                if(data_startID < ins_StartID){
                    // まだ小さい
                    add_position++;
                }
                else if( ins_StartID < data_startID){
                    // すでに超えている
                    break;
                }
                else{
                     //add_position+1番目の既存データのIDが自分のIDと同じ

                     // positionの場所を探す
                    if( data_startPosition < ins_StartPosition ){
                        add_position++;
                        continue;
                    }
                    else{
                        // 同じpositionは除外されている
                        break;
                    }
                }
            }
            if(IsInsert){
                (connectLists[_type]).Insert(add_position, add_list);
            }            
        }
    }

    /// <summary>
    /// 管理している接続の描画
    /// </summary>
    /// <param name="mousePosition">マウスの位置情報</param>
    public static void Update()//Vector2 mousePosition)
    {
        connectors[NodeArrow.current_type].ForEach(con => con.Draw());

        if (current != null)
        {
            // current.DrawTo(mousePosition);
        }
    }
}

/// <summary>
/// ノード間の接続を表すクラス
/// </summary>
public class Connector
{
    public Node StartNode { get; protected set; }
    public int StartPosition { get; protected set; }
    public Node EndNode { get; protected set; }
    public int EndPosition{ get; protected set; }
    
    public void Init(Node StartNode, int StartPosition, Node EndNode, int EndPosition){
        this.StartNode = StartNode;
        this.StartPosition = StartPosition;
        this.EndNode = EndNode;
        this.EndPosition = EndPosition;
    }

    public void SetStartNode(Node node) {
        StartNode = node;
    }
    public void SetStartPosition(int index) {
        StartPosition = index;
    }

    // 終点を決める = Connect
    public void Connect(Node node, int position){
        EndNode = node;
        EndPosition = position;
    }
    /// <summary>
    /// 接続を曲線として描画
    /// </summary>
    public void Draw()
    {
        if (EndNode == null)
        {
            // throw new UnityException("No end node.");
        }

        Vector2 start1 = new Vector2(StartNode.Rect.x + StartNode.Rect.width + 25, StartNode.Rect.y + 25 + (StartPosition+1)*25);
        Vector2 start2 = new Vector2(EndNode.Rect.x - 25, StartNode.Rect.y + 25 + (StartPosition+1)*25);
        Vector2 end1 = new Vector2(EndNode.Rect.x - 25, StartNode.Rect.y + 25 + (StartPosition+1)*25);
        Vector2 end2 = new Vector2(EndNode.Rect.x - 25, EndNode.Rect.y + 25 + (EndPosition+1)*25);
        Drawinged.DrawLine(start1, end2);
    }

}



    /*Init(Node StartNode, int StartPosition, Node EndNode, int EndPosition){

    ・connectors.Count
    ・connectを作る
        ・StartNode
        ・StartPosition
        ・EndNode
        ・EndPosition





    ・connectors[i].StartNodeの
        ・node.Id
        ・node.Position.x
        ・node.Position.y
        ・node.ClickCount
        ・node.State
        ・node.Conditions:
            =>  ・conditions.Count
                ・conditions[i]

    ・connectors[i].EndNodeの
        ・node.Id
        ・node.Position.x
        ・node.Position.y
        ・node.ClickCount
        ・node.State
        ・node.Conditions:
            =>  ・conditions.Count
                ・conditions[i]

    ・connectors[i].StartPosition
    ・connectors[i].EndPosition
    ・SetDataListInt("ConnectStartNodeId", startIdList, _type);// startIdList＝startIdList.Add(connectors[i].StartNode.Id);
        ・startIdList.Count
        ・startIdList[i]
    ・SetDataListInt("ConnectEndNodeId", endIdList, _type);//EndIdの記録
        endIdList.Count
        endIdList[i]
    */

    // DataNode Set Get Delete

public class Drawinged
{ 
    public static Texture2D lineTex;
 
    public static void DrawLine(Rect rect) { DrawLine(rect, GUI.contentColor, 1.0f); }
    public static void DrawLine(Rect rect, Color color) { DrawLine(rect, color, 1.0f); }
    public static void DrawLine(Rect rect, float width) { DrawLine(rect, GUI.contentColor, width); }
    public static void DrawLine(Rect rect, Color color, float width) { DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y + rect.height), color, width); }
    public static void DrawLine(Vector2 pointA, Vector2 pointB) { DrawLine(pointA, pointB, GUI.contentColor, 1.0f); }
    public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color) { DrawLine(pointA, pointB, color, 1.0f); }
    public static void DrawLine(Vector2 pointA, Vector2 pointB, float width) { DrawLine(pointA, pointB, GUI.contentColor, width); }
    public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width)
    {
        Matrix4x4 matrix = GUI.matrix;
 
        if (!lineTex) { lineTex = new Texture2D(1, 1); }
 
        Color savedColor = GUI.color;
        GUI.color = color;
 
        float angle = Vector3.Angle(pointB - pointA, Vector2.right);
 
        if (pointA.y > pointB.y) { angle = -angle; }
 
        // GUIUtility.ScaleAroundPivot(new Vector2((pointB - pointA).magnitude, width), new Vector2(pointA.x, pointA.y + 0.5f));
        GUIUtility.RotateAroundPivot(angle, pointA);
 
        GUI.DrawTexture(new Rect(pointA.x, pointA.y, (pointB - pointA).magnitude, width), lineTex);
        // GUI.DrawTexture(new Rect(pointA.x, pointA.y, 1, 1), lineTex);
 
        GUI.matrix = matrix;
        GUI.color = savedColor;
    }
}
