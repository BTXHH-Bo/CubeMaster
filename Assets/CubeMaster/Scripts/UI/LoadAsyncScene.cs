using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadAsyncScene : MonoBehaviour {

    private AsyncOperation async = null;

	// Use this for initialization
	void Start () {
        if (SceneManager.GetActiveScene().name == "Loading")
        {
            StartCoroutine(AsyncLoading());
        }
    }

    IEnumerator AsyncLoading()
    {
        async = SceneManager.LoadSceneAsync("World");
        async.allowSceneActivation = false;
        yield return async;
    }

    // Update is called once per frame
    void Update () {
        if (async.progress>0.89f)
        {
            async.allowSceneActivation = true;
        }
	}
}
