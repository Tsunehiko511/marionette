using UnityEngine;
using UnityEngine.UI;
using System.Collections;
 
public class Right_Left_Arrow : MonoBehaviour {
  public GameObject RightButton;
  public GameObject LeftButton;
  public Text TypeText;
  public int right_count = 0;

  void Awake(){
    RightButton = GameObject.Find("Screen/Panels/Panel/Right");
    LeftButton  = GameObject.Find("Screen/Panels/Panel/Left");
    TypeText    = GameObject.Find("Screen/Panels/Panel/Text").GetComponent<Text>();
  }

  void Start(){
    LeftButton.SetActive(false);
  }
  void SetTypeText(int _count){
    switch(_count){
      case 0:
      TypeText.text = "KING";
      break;
      case 1:
      TypeText.text = "POON";
      break;
      case 2:
      TypeText.text = "ROOK";
      break;
      case 3:
      TypeText.text = "BISHOP";
      break;
      case 4:
      TypeText.text = "QUEEN";
      break;
      default:
      TypeText.text = "KING";      
      break;
    }
  }

  public void PushRight(){
    right_count++;
    NodeArrow.current_type = right_count;
    SetTypeText(right_count);

    if(right_count == NodeArrow.ai_type -1){
      RightButton.SetActive(false);
      LeftButton.SetActive(true);
    }
    else{
      RightButton.SetActive(true);
      LeftButton.SetActive(true);
    }
  }
  public void PushLeft(){
    right_count--;
    NodeArrow.current_type = right_count;
    SetTypeText(right_count);
    if(right_count == 0){
      RightButton.SetActive(true);
      LeftButton.SetActive(false);
    }
    else{
      RightButton.SetActive(true);
      LeftButton.SetActive(true);
    }
  }
}