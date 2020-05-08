using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlHeadBob : MonoBehaviour {

	GameObject buttonObject;
	Button button;
	Text text;

	// Use this for initialization
	void Start () {
		buttonObject = GameObject.Find("HeadBobButton");
		button = buttonObject.GetComponent<Button>();
		text = button.transform.Find("Text").GetComponent<Text>();
		button.onClick.AddListener(OnHeadButtonClick);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnHeadButtonClick()
	{
		if (text.text == "点击关闭")
		{
			Globe.HeadBob = false;
			text.text = "点击开启";
		}
		else if (text.text == "点击开启")
		{
			Globe.HeadBob = true;
			text.text = "点击关闭";
		}
	}
}
