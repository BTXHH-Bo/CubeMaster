using UnityEngine;
using System.Collections;

//player��block����ײ����


namespace CubeMaster {

	public class ColliderEventsSender : MonoBehaviour
	{

		private Index LastIndex;
		private Chunk LastChunk;

		public void Update()
		{

			// ���chunk����
			GameObject chunkObject = Engine.PositionToChunk(transform.position);
			if (chunkObject == null) return;

			// ��ȡ������Ϣ
			Chunk chunk = chunkObject.GetComponent<Chunk>();
			Index voxelIndex = chunk.PositionToVoxelIndex(transform.position);
			VoxelInfo voxelInfo = new VoxelInfo(voxelIndex, chunk);

			// ��ȡ����
			GameObject voxelObject = Instantiate(Engine.GetVoxelGameObject(voxelInfo.GetVoxel())) as GameObject;

			VoxelEvents events = voxelObject.GetComponent<VoxelEvents>();
			if (events != null)
			{
				//����ĳ����
				if (chunk != LastChunk || voxelIndex.IsEqual(LastIndex) == false)
				{
					events.OnBlockEnter(this.gameObject, voxelInfo);
				}
				//ͣ��
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

























