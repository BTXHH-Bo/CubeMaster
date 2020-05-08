using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackToWorld : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<Button>().onClick.AddListener(OnClick);
	}
	
	void OnClick()
    {
		Globe.isMouseVisible = false;		
	}
	// Update is called once per frame
	void Update () {
		
	}
}
