using CubeMaster;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class StartNewWorld : MonoBehaviour{

	public GameObject startButton;
	public GameObject input;
	

	void Start () {
		input = GameObject.Find("Input");
		startButton = GameObject.Find("Start");
		startButton.GetComponent<Button>().onClick.AddListener(OnClick);

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnClick()
    {
		if (input.GetComponent<InputField>().text == "")
		{
			input.GetComponent<InputField>().placeholder.GetComponent<Text>().text = "请输入世界名！";
		}
		else
		{
			Engine.SetWorldName(input.GetComponent<InputField>().text);
			Globe.worldName = input.GetComponent<InputField>().text;
			SceneManager.LoadScene("Loading");
		}
    }


}
