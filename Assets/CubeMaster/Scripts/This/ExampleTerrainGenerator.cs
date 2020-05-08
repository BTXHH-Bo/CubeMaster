using UnityEngine;
using System.Collections;

//当前实例
namespace CubeMaster {

	public class ExampleTerrainGenerator : TerrainGenerator
	{

		public override void GenerateVoxelData()
		{

			int chunky = chunk.ChunkIndex.y;
			int SideLength = Engine.ChunkSideLength;


			for (int x = 0; x < SideLength; x++)
			{
				for (int y = 0; y < SideLength; y++)
				{
					for (int z = 0; z < SideLength; z++)
					{ // 对一个chunk的所有体素

						Vector3 voxelPos = chunk.VoxelIndexToPosition(x, y, z); //获得体素绝对坐标
						voxelPos = new Vector3(voxelPos.x + seed, voxelPos.y, voxelPos.z + seed); //偏移（按照种子）

						float perlin1 = Mathf.PerlinNoise(voxelPos.x * 0.010f, voxelPos.z * 0.010f) * 70.1f; //山
						float perlin2 = Mathf.PerlinNoise(voxelPos.x * 0.085f, voxelPos.z * 0.085f) * 9.1f; //平地


						int currentHeight = y + (SideLength * chunky); //获得体素的绝对高度

						//草地
						if (perlin1 > currentHeight)
						{
							if (perlin1 > perlin2 + currentHeight)
							{
								chunk.SetVoxelSimple(x, y, z, 2);   // 放置grass blocks

							}
						}

						//纯泥块
						currentHeight = currentHeight + 1; // 偏移1
						if (perlin1 > currentHeight)
						{
							if (perlin1 > perlin2 + currentHeight)
							{
								chunk.SetVoxelSimple(x, y, z, 1);
							}
						}

						// debug
						//if (random == 1) {
						//chunk.SetVoxelSimple(x,y,z, 3); // 随便
						//}

					}
				}
			}
		}
	}

}