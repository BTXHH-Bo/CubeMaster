using UnityEngine;
using System.Collections;


//地形生成器

namespace CubeMaster {

	public class TerrainGenerator : MonoBehaviour
	{

		protected Chunk chunk;
		protected int seed;

		public void InitializeGenerator()
		{

			// 载入种子
			while (Engine.WorldSeed == 0)
			{
				Engine.GetSeed();
			}
			seed = Engine.WorldSeed;

			// 获取chunk部件
			chunk = GetComponent<Chunk>();

			// 生成voxel信息
			GenerateVoxelData();

			chunk.Empty = true;
			foreach (ushort voxel in chunk.VoxelData)
			{
				if (voxel != 0)
				{
					chunk.Empty = false;
					break;
				}
			}

			chunk.VoxelsDone = true;
		}

		public virtual void GenerateVoxelData()
		{

		}
	}

}