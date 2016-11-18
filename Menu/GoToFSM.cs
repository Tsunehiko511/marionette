using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GoToFSM : MonoBehaviour {
	public void goToFSM() {
		GameConfiguration.IsSetAI = true;
		SceneManager.LoadScene("FSM");
	}
}
