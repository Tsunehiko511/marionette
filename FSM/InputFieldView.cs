using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;

public class InputFieldView : MonoBehaviour {
  private InputField inputField1;
  private NodeArrow node_A;

  void Start(){
      inputField1 = GetComponent<InputField>();
      node_A = GameObject.Find("Node").GetComponent<NodeArrow>();
  }
  void Update(){
  	if(Input.GetKeyUp(KeyCode.Return)){
  		EndEdit();
  	}
  }

  // このメソッドをOn Value Changeに指定すると文字変更があった時に呼び出される
  public void OnValueChange()
  {
      // Debug.Log(inputField1.text);  // 入力された文字を表示
  }

  // このメソッドをEnd Editに指定すると入力が確定した時に呼び出される
  public void EndEdit()
  {
      if(inputField1.text == ""){
      	return;
      }
      string _inputText = Base64ToString(inputField1.text);
      node_A.SetBase64(ref _inputText);
      ConnectorManager.SetBase64(ref _inputText);
  }

  public void Viewdata(){
  	string _2digit = node_A.GetBase64();
  	string tmp_2digit = ConnectorManager.GetBase64();
    _2digit += tmp_2digit;
  	inputField1.text = StringToBase64(_2digit);
  }


  // 2進数データからbase64に変換
  public string StringToBase64(string _inputText){
  	byte[] bytesToEncode = Encoding.UTF8.GetBytes (_inputText);
  	string encodedText = Convert.ToBase64String (bytesToEncode);
    return encodedText;
  }

  // base64から2進数データに変換
  public string Base64ToString(string _encodedText){
		byte[] decodedBytes = Convert.FromBase64String (_encodedText);
		string decodedText = Encoding.UTF8.GetString (decodedBytes);  	
    // Debug.Log(decodedText);
    return decodedText;
  }
}
