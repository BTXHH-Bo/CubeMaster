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
			{ //左键销毁
				Voxel.DestroyBlock(voxelInfo);
			}
			else if (mouseButton == 1)
			{ //右键放置

				if (voxelInfo.GetVoxel() == 8)
				{ //草（一种植物）可以被覆盖
					Voxel.PlaceBlock(voxelInfo, ExampleInventory.HeldBlock);
				}
				else
				{ //其他block不可以
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

			//更改ui
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
			//当即将被压倒的block是个草（一种植物）泥块，就把他换成纯泥块
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
			//当要消除的块上面是草（一种植物）块，顺道把草（一种植物）也去掉
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

