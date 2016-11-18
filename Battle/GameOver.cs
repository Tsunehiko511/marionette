using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {
	public void goToFSM() {
		SceneManager.LoadScene("FSM");
	}
}