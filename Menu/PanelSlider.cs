using UnityEngine;
using UnityEngine.UI;
using System.Collections;
 
public class PanelSlider : MonoBehaviour {
  public AnimationCurve animCurve = AnimationCurve.Linear(0, 0, 1, 1);
  public Vector3 arenaPosition;        // スライドイン後の位置
  public Vector3 battlePosition;      // スライドアウト後の位置

  public float duration = 1.0f;    // スライド時間（秒）

  public void InitPosition(){
    duration = 0f;
    StartCoroutine( StartSlidePanel("arena") );
  }

  // スライドイン（Pauseボタンが押されたときに、これを呼ぶ）
  public void SlideArena(){
      StartCoroutine( StartSlidePanel("arena") );
  }
  public void SlideBattle(){
      StartCoroutine( StartSlidePanel("battle") );
  }
  void Start(){
    /*
    if(User.Level < 6){
      SlideStory();
    }
    else{
      InitPosition();
    }*/
  }

  private IEnumerator StartSlidePanel( string type ){
    float startTime = Time.time;    // 開始時間
    Vector3 startPos = transform.localPosition;  // 開始位置
    Vector3 moveDistance;            // 移動距離および方向
    switch(type){
      case "arena":
      moveDistance = (arenaPosition - startPos);
      break;
    	case "battle":
    	moveDistance = (battlePosition - startPos);
    	break;
    	default:
    	moveDistance = new Vector3(0,0,0);
    	break;
    }

    while((Time.time - startTime) < duration){
        transform.localPosition = startPos + moveDistance * animCurve.Evaluate((Time.time - startTime) / duration);
        yield return 0;        // 1フレーム後、再開
    }
    duration = 1.0f;
    transform.localPosition = startPos + moveDistance;
  }
}