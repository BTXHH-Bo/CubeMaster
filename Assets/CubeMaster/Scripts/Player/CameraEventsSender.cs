using UnityEngine;
using System.Collections;

// ����voxelevent

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
		{ // ���λ��

			VoxelInfo raycast = Engine.VoxelRaycast(Camera.main.ScreenPointToRay(Input.mousePosition), 9999.9f, false);

			if (raycast != null)
			{

				//����һ������׼���صĿ���
				GameObject voxelObject = Instantiate(Engine.GetVoxelGameObject(raycast.GetVoxel())) as GameObject;

				//voxel��event
				if (voxelObject.GetComponent<VoxelEvents>() != null)
				{

					voxelObject.GetComponent<VoxelEvents>().OnLook(raycast);

					// ������궯������Ϣ
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
				// δ�����У�����ʾ
				if (SelectedBlockGraphics != null)
				{
					SelectedBlockGraphics.GetComponent<Renderer>().enabled = false;
				}
			}

		}

		private void CameraLookEvents()
		{ //��һ�˳��ӽ�

			VoxelInfo raycast = Engine.VoxelRaycast(Camera.main.transform.position, Camera.main.transform.forward, Range, false);

			if (raycast != null)
			{

				//��������
				GameObject voxelObject = Instantiate(Engine.GetVoxelGameObject(raycast.GetVoxel())) as GameObject;

				//���������ж���
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
				// ���ʱ����ʾ��ѡ�е�ui
				if (SelectedBlockGraphics != null)
				{
					SelectedBlockGraphics.GetComponent<Renderer>().enabled = false;
				}
			}
		}



	}

}