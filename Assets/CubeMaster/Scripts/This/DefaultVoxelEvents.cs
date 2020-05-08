using UnityEngine;
using System.Collections;

// daerzi

namespace CubeMaster {

	public class DefaultVoxelEvents : VoxelEvents
	{

		public override void OnMouseDown(int mouseButton, VoxelInfo voxelInfo)
		{

			if (Globe.isMouseVisible)
			{
				return;
			}

			if (mouseButton == 0)
			{ //�������
				Voxel.DestroyBlock(voxelInfo);
			}
			else if (mouseButton == 1)
			{ //�Ҽ�����

				if (voxelInfo.GetVoxel() == 8)
				{ //�ݣ�һ��ֲ����Ա�����
					Voxel.PlaceBlock(voxelInfo, ExampleInventory.HeldBlock);
				}
				else
				{ //����block������
					VoxelInfo newInfo = new VoxelInfo(voxelInfo.adjacentIndex, voxelInfo.chunk);
					Voxel.PlaceBlock(newInfo, ExampleInventory.HeldBlock);
				}
			}

		}

		public override void OnLook(VoxelInfo voxelInfo)
		{
			if (Globe.isMouseVisible)
			{
				return;
			}

			//����ui
			GameObject blockSelection = GameObject.Find("selected block graphics");
			if (blockSelection != null)
			{
				blockSelection.transform.position = voxelInfo.chunk.VoxelIndexToPosition(voxelInfo.index);
				blockSelection.GetComponent<Renderer>().enabled = true;
				blockSelection.transform.rotation = voxelInfo.chunk.transform.rotation;
			}

		}

		public override void OnBlockPlace(VoxelInfo voxelInfo)
		{
			if (Globe.isMouseVisible)
			{
				return;
			}
			//��������ѹ����block�Ǹ��ݣ�һ��ֲ���飬�Ͱ������ɴ����
			Index indexBelow = new Index(voxelInfo.index.x, voxelInfo.index.y - 1, voxelInfo.index.z);

			if (voxelInfo.GetVoxelType().VTransparency == Transparency.solid
			&& voxelInfo.chunk.GetVoxel(indexBelow) == 2)
			{
				voxelInfo.chunk.SetVoxel(indexBelow, 1, true);
			}
		}

		public override void OnBlockDestroy(VoxelInfo voxelInfo)
		{
			if (Globe.isMouseVisible)
			{
				return;
			}
			//��Ҫ�����Ŀ������ǲݣ�һ��ֲ��飬˳���Ѳݣ�һ��ֲ�Ҳȥ��
			Index indexAbove = new Index(voxelInfo.index.x, voxelInfo.index.y + 1, voxelInfo.index.z);

			if (voxelInfo.chunk.GetVoxel(indexAbove) == 8)
			{
				voxelInfo.chunk.SetVoxel(indexAbove, 0, true);
			}

		}

		public override void OnBlockEnter(GameObject enteringObject, VoxelInfo voxelInfo)
		{

			Debug.Log("OnBlockEnter at " + voxelInfo.chunk.ChunkIndex.ToString() + " / " + voxelInfo.index.ToString());

		}

	}

}

