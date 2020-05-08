using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CallMenu : MonoBehaviour {


	// Use this for initialization
	void Start () {
		Globe.characterController = GetComponent<CharacterController>();
		Globe.audioSource = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update () {

	}
}
