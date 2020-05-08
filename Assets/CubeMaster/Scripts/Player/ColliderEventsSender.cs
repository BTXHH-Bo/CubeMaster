using UnityEngine;
using System.Collections;

//player与block的碰撞交互


namespace CubeMaster {

	public class ColliderEventsSender : MonoBehaviour
	{

		private Index LastIndex;
		private Chunk LastChunk;

		public void Update()
		{

			// 检测chunk存在
			GameObject chunkObject = Engine.PositionToChunk(transform.position);
			if (chunkObject == null) return;

			// 获取体素信息
			Chunk chunk = chunkObject.GetComponent<Chunk>();
			Index voxelIndex = chunk.PositionToVoxelIndex(transform.position);
			VoxelInfo voxelInfo = new VoxelInfo(voxelIndex, chunk);

			// 获取拷贝
			GameObject voxelObject = Instantiate(Engine.GetVoxelGameObject(voxelInfo.GetVoxel())) as GameObject;

			VoxelEvents events = voxelObject.GetComponent<VoxelEvents>();
			if (events != null)
			{
				//进入某体素
				if (chunk != LastChunk || voxelIndex.IsEqual(LastIndex) == false)
				{
					events.OnBlockEnter(this.gameObject, voxelInfo);
				}
				//停留
				else
				{
					events.OnBlockStay(this.gameObject, voxelInfo);
				}
			}

			LastChunk = chunk;
			LastIndex = voxelIndex;

			Destroy(voxelObject);

		}

	}

}

























