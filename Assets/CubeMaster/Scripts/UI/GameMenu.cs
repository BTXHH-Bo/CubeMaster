using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class GameMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Canvas>().enabled = false;
		GameObject.Find("WalkSpeedBar").GetComponent<Slider>().
			onValueChanged.AddListener(OnWalkSpeedBarValueChanged);
		GameObject.Find("RunSpeedBar").GetComponent<Slider>().
			onValueChanged.AddListener(OnRunSpeedBarValueChanged);
		GameObject.Find("JumpSpeedBar").GetComponent<Slider>().
			onValueChanged.AddListener(OnJupmSpeedBarValueChanged);
		GameObject.Find("HeightBar").GetComponent<Slider>().
			onValueChanged.AddListener(OnHeightBarValueChanged);
		GameObject.Find("GravityMultiplierBar").GetComponent<Slider>().
			onValueChanged.AddListener(OnGravityMultiplierBarValueChanged);
		GameObject.Find("StepIntervalBar").GetComponent<Slider>().
			onValueChanged.AddListener(OnStepIntervalBarValueChanged);


	}

    private void OnWalkSpeedBarValueChanged(float arg0)
    {
        Globe.walkSpeed = GameObject.Find("WalkSpeedBar").GetComponent<Slider>().value * 7f + 1f;
    }

    private void OnRunSpeedBarValueChanged(float arg0)
    {
        Globe.RunSpeed = GameObject.Find("RunSpeedBar").GetComponent<Slider>().value * 12f + 3f;
    }

    private void OnJupmSpeedBarValueChanged(float arg0)
    {
        Globe.JumpSpeed = GameObject.Find("JumpSpeedBar").GetComponent<Slider>().value * 15f + 5f;
    }

    private void OnHeightBarValueChanged(float arg0)
    {
        Globe.characterController.height = GameObject.Find("HeightBar").GetComponent<Slider>().value * 2.5f + 0.5f;
    }

    private void OnGravityMultiplierBarValueChanged(float arg0)
    {
        Globe.GravityMultiplier = GameObject.Find("GravityMultiplierBar").GetComponent<Slider>().value * 4.9f + 0.1f;
    }

    private void OnStepIntervalBarValueChanged(float arg0)
    {
        Globe.StepInterval = GameObject.Find("StepIntervalBar").GetComponent<Slider>().value * 6f + 2f;
    }




    // Update is called once per frame
    void Update () {
        if (Globe.isMouseVisible == true)
        {
            GetComponent<Canvas>().enabled = true;
        }
        else
        {
            GetComponent<Canvas>().enabled = false;
        }

    }
}
