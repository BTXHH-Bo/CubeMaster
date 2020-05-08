using UnityEngine;
using System.Collections;

// 发送voxelevent

namespace CubeMaster {

	public class CameraEventsSender : MonoBehaviour
	{

		public float Range;
		private GameObject SelectedBlockGraphics;

		public void Awake()
		{
			if (Range <= 0)
			{
				Debug.LogWarning("CubeMaster: CameraEventSender.Range must be greater than 0. Setting Range to 5.");
				Range = 5.0f;
			}

			SelectedBlockGraphics = GameObject.Find("selected block graphics");
		}

		public void Update()
		{

			if (Engine.SendCameraLookEvents)
			{
				CameraLookEvents();
			}
			if (Engine.SendCursorEvents)
			{
				MouseCursorEvents();
			}
		}

		private void MouseCursorEvents()
		{ // 鼠标位置

			VoxelInfo raycast = Engine.VoxelRaycast(Camera.main.ScreenPointToRay(Input.mousePosition), 9999.9f, false);

			if (raycast != null)
			{

				//创建一个被瞄准体素的拷贝
				GameObject voxelObject = Instantiate(Engine.GetVoxelGameObject(raycast.GetVoxel())) as GameObject;

				//voxel有event
				if (voxelObject.GetComponent<VoxelEvents>() != null)
				{

					voxelObject.GetComponent<VoxelEvents>().OnLook(raycast);

					// 对于鼠标动作发信息
					for (int i = 0; i < 3; i++)
					{
						if (Input.GetMouseButtonDown(i))
						{
							voxelObject.GetComponent<VoxelEvents>().OnMouseDown(i, raycast);
						}
						if (Input.GetMouseButtonUp(i))
						{
							voxelObject.GetComponent<VoxelEvents>().OnMouseUp(i, raycast);
						}
						if (Input.GetMouseButton(i))
						{
							voxelObject.GetComponent<VoxelEvents>().OnMouseHold(i, raycast);
						}
					}
				}

				Destroy(voxelObject);

			}

			else
			{
				// 未被击中，不显示
				if (SelectedBlockGraphics != null)
				{
					SelectedBlockGraphics.GetComponent<Renderer>().enabled = false;
				}
			}

		}

		private void CameraLookEvents()
		{ //第一人称视角

			VoxelInfo raycast = Engine.VoxelRaycast(Camera.main.transform.position, Camera.main.transform.forward, Range, false);

			if (raycast != null)
			{

				//创建拷贝
				GameObject voxelObject = Instantiate(Engine.GetVoxelGameObject(raycast.GetVoxel())) as GameObject;

				//当体素上有动作
				if (voxelObject.GetComponent<VoxelEvents>() != null)
				{

					voxelObject.GetComponent<VoxelEvents>().OnLook(raycast);

					for (int i = 0; i < 3; i++)
					{
						if (Input.GetMouseButtonDown(i))
						{
							voxelObject.GetComponent<VoxelEvents>().OnMouseDown(i, raycast);
						}
						if (Input.GetMouseButtonUp(i))
						{
							voxelObject.GetComponent<VoxelEvents>().OnMouseUp(i, raycast);
						}
						if (Input.GetMouseButton(i))
						{
							voxelObject.GetComponent<VoxelEvents>().OnMouseHold(i, raycast);
						}
					}
				}

				Destroy(voxelObject);

			}

			else
			{
				// 瞄空时不显示被选中的ui
				if (SelectedBlockGraphics != null)
				{
					SelectedBlockGraphics.GetComponent<Renderer>().enabled = false;
				}
			}
		}



	}

}