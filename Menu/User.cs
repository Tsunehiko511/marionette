using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class User : MonoBehaviour{
	// public static int Level {get; set;}
	public static string name;

	void Awake(){
		name = FindObjectOfType<UserAuth>().currentPlayer();
		// PlayerPrefs.SetInt(name+":UserLevel", 7);
		int newVersion = PlayerPrefs.GetInt("NewVersion", 0);
		/*if(newVersion == 1){
			Level = PlayerPrefs.GetInt(name+":UserLevel", 0);
		}
		else{
			PlayerPrefs.SetInt(name+":UserLevel", 0);
			Level = PlayerPrefs.GetInt(name+":UserLevel", 0);			
		}
		Debug.Log("UserLevel:"+Level);
		*/
	}
	/*
	public static void LevelUP(){
		Level++;
		PlayerPrefs.SetInt(name+":UserLevel", Level);
		PlayerPrefs.Save();
	}*/
}

//beginner
/*
public static class Enemy{
    public static List<int[]> connectLists = new List<int[]>();
    public static string name;
}
public static class MyConnect{
    public static List<int []> connectLists = new List<int[]>(); // 接続リスト
}*/
// エキスパート expert 
public static class Enemys{
    public static List<List<int[]>> connectLists = new List<List<int[]>>();
    public static string name;
}
public static class MyConnects{
    public static List<List<int[]>> connectLists = new List<List<int[]>>();
}