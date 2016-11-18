using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GoToMenu : MonoBehaviour {
	public void goToMenu() {
        NodeArrow.Saved();
        /*
		PlayerPrefs.Save();
        /*
        if(User.Level == 1){
            User.LevelUP();
        }
        else if(User.Level == 3){
            User.LevelUP();
        }*/
		SceneManager.LoadScene("Menu");
	}
}
