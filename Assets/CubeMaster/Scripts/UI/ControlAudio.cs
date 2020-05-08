using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlAudio : MonoBehaviour {

    GameObject buttonObject;
    Button button;
    Text text;

	// Use this for initialization
	void Start () {
        buttonObject = GameObject.Find("AudioButton");
        button = buttonObject.GetComponent<Button>();
        text = button.transform.Find("Text").GetComponent<Text>();
        button.onClick.AddListener(OnAudioButtonClick);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnAudioButtonClick()
    {
        if (text.text == "点击关闭")
        {
            Globe.audioSource.enabled = false;
            text.text = "点击开启";
        }
        else if (text.text == "点击开启")
        {
            Globe.audioSource.enabled = true;
            text.text = "点击关闭";
        }
    }
}
