using UnityEngine;

namespace CubeMaster {

	public class Debugger : MonoBehaviour
	{

		private float saveTimer = 60.0f;
		private bool isCursorFree;

		void Start()
		{
			isCursorFree = false;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		void OnGUI()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(20);
			GUILayout.BeginVertical();
			GUILayout.Space(Screen.height - 200);
			GUILayout.BeginVertical("Tips");
			GUILayout.Label("1-9: Select block");
			GUILayout.Label("RMB: Place block");
			GUILayout.Label("LMB: Remove block");
			GUILayout.Label("Shift: Run");
			GUILayout.Label("Space: Jump");
			GUILayout.Label("ESC: Menu");
			GUILayout.Label("V: Save");
			GUILayout.EndVertical();
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}
		

		void Update()
		{

			/*
			if(Input.GetKey(KeyCode.LeftShift)&&Time.realtimeSinceStartup>3.0f)
			{
				GetComponent<CharacterMotor>().movement.maxForwardSpeed = 7;
				GetComponent<CharacterMotor>().movement.maxBackwardsSpeed = 7;
				GetComponent<CharacterMotor>().movement.maxSidewaysSpeed = 7;
				GetComponent<CharacterMotor>().jumping.baseHeight = 0.9f;
			}

			if (Input.GetKeyUp(KeyCode.LeftShift) && Time.realtimeSinceStartup > 3.0f)
			{
				GetComponent<CharacterMotor>().movement.maxForwardSpeed = 3;
				GetComponent<CharacterMotor>().movement.maxBackwardsSpeed = 3;
				GetComponent<CharacterMotor>().movement.maxSidewaysSpeed = 3;
				GetComponent<CharacterMotor>().jumping.baseHeight = 0.6f;
			}

			if (Input.GetKeyDown(KeyCode.Escape) && Time.realtimeSinceStartup > 3.0f)
			{
				if (isCursorFree)
				{
					//Cursor.lockState = CursorLockMode.Locked;
					Cursor.visible = false;
					isCursorFree = false;
				}
				else
				{
					//Cursor.lockState = CursorLockMode.Confined;
					Cursor.visible = true;
					isCursorFree = true;
				}
			}
			*/

			//保存世界
			if (Input.GetKeyUp("v"))
			{
				Engine.SaveWorldInstant();
			}

			if (Input.GetKeyUp(KeyCode.Escape))
			{
				Globe.isMouseVisible = true;
			}


			// 保存世界场景 计时器
			if (saveTimer < 0.0f)
			{
				saveTimer = 60.0f;
				Engine.SaveWorld();
			}
			else
			{
				saveTimer -= Time.deltaTime;
			}

		}


	}

}
