/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenOldWorld : MonoBehaviour {

	Button btn;
	// Use this for initialization
	void Start () {
		btn = GameObject.Find("NewWorldButton").GetComponent<Button>();
		btn.onClick.AddListener(OnClick);
	}
	
	void OnClick()
    {
		FileDialog.windowDialog = GUI.Window(100, fileDialog.windowDialog, myFileWindow, "");
	}

	// Update is called once per frame
	void Update () {
		
	}
}
*/

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Reflection;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OpenFile : MonoBehaviour
{

    public Button button;

    void Start()
    {
        button = GameObject.Find("OpenWorldButton").GetComponent<Button>();
        button.onClick.AddListener(OpenFileWin);
    }

    public void OpenFileWin()
    {
        OpenFileName ofn = new OpenFileName();

        ofn.structSize = Marshal.SizeOf(ofn);

        ofn.filter = "All Files\0*.*\0\0";

        ofn.file = new string(new char[256]);

        ofn.maxFile = ofn.file.Length;

        ofn.fileTitle = new string(new char[64]);

        ofn.maxFileTitle = ofn.fileTitle.Length;
        string path = Application.streamingAssetsPath;
        path = path.Replace('/', '\\');
        //默认路径  
        //ofn.initialDir = path;
        ofn.initialDir = Application.dataPath + "/../Worlds/";
        //ofn.initialDir = "D:\\MyProject\\UnityOpenCV\\Assets\\StreamingAssets";  
        ofn.title = "Open Project";

        //注意 一下项目不一定要全选 但是0x00000008项不要缺少  
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;//OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR  

        if (WindowDll.GetOpenFileName(ofn))
        {
            Debug.Log("Selected file with full path: {0}" + ofn.file);
        }
        //此处更改了大部分答案的协程方法，在这里是采用unity的videoplayer.url方法播放视频；
        //而且我认为大部分的其他答案，所给的代码并不全

        Globe.worldPath = ofn.file;
        Globe.isChooseWorld = true;
        SceneManager.LoadScene("Loading");
    }

}