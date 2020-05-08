using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RefrashText : MonoBehaviour {

	public int number = 0;
	public string text = "";

	// Use this for initialization
	void Start () {
		text = GetComponent<Text>().text;
		InvokeRepeating("TimeCount", 0.0f, 0.5f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void TimeCount()
    {
        if (number < 5)
        {
			number++;
        }
        else
        {
			number = 0;
        }
		GetComponent<Text>().text = text;
		for(int i = 0; i < number; ++i)
        {
			GetComponent<Text>().text += ".";
        }
    }

}
