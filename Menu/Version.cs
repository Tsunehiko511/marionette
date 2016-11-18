using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using NCMB;
using UnityEngine.SceneManagement;

public class Version : MonoBehaviour {
	private string version;
	Text textVersion;
	bool IsVersionCheck;

 private Rect windowRect = new Rect (10, 10, 600, 90);
 bool show = false;

	NCMB.ServerVersion ver;
	// Use this for initialization
	void Start () {
		version = PlayerPrefs.GetString("Version", Constants.VERSION);
		textVersion = GetComponent<Text>();
		textVersion.text = "ver "+version;
		ver = new ServerVersion();
		ver.fetch();
	}
	
	// Update is called once per frame
	void Update () {
		if(ver.isNoData){
			ver.isNoData = false;
			ver.fetch();
			Debug.Log("データ取れない");	
		}
		if(ver.isFetched && !IsVersionCheck){
			if(version != ver.version){
				Debug.Log("バージョン違い");	
				// PlayerPrefs.SetInt("NewVersion", 0);
				// User.Level = 0;
				show = true;
			}
			else{
				Debug.Log("バージョン同じ");	
				// PlayerPrefs.SetInt("NewVersion", 1);
			}
			PlayerPrefs.Save();
			IsVersionCheck = true;
		}
	}





  void OnGUI () 
  {
    if(show){
    	GUI.skin.label.fontSize = 18;
    	windowRect = GUI.Window (0, windowRect, DialogWindow, "");
    }
  }

  // This is the actual window.
  void DialogWindow (int windowID){
    float y = 20;
    GUI.Label(new Rect(5,y, windowRect.width-5, 60), "専用サイトで新しいバージョンと取り替えてください。\n");
    if(GUI.Button(new Rect(windowRect.width - 50 -5,windowRect.height - 35, 50, 30), "OK")){
    	Application.OpenURL("http://fromalgorithm.jimdo.com/new-game/");
    	MoveToTitle();
    }
    else{
    	Invoke("MoveToTitle", 10.0f);
    }
  }

  void MoveToTitle(){
  	show = false;
  	Application.OpenURL("http://fromalgorithm.jimdo.com/new-game/");
  	SceneManager.LoadScene("Title");
  }

}
