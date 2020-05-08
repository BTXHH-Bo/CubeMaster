using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OpenNewWorldMenu : MonoBehaviour {

	void Start()
	{
		this.GetComponent<Button>().onClick.AddListener(OnClick);
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void OnClick()
	{
		SceneManager.LoadScene("NewWorldMenu");
	}


}

