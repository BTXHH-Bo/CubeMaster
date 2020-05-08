using UnityEngine;
using System.Collections;

namespace CubeMaster {

	public class ExampleTerrainGeneratorWithTrees : TerrainGenerator
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
					{ //对当前chunk中所有体素

						Vector3 voxelPos = chunk.VoxelIndexToPosition(x, y, z); //获取绝对坐标
						voxelPos = new Vector3(voxelPos.x + seed, voxelPos.y, voxelPos.z + seed);

						float perlin1 = Mathf.PerlinNoise(voxelPos.x * 0.010f, voxelPos.z * 0.010f) * 70.1f;
						float perlin2 = Mathf.PerlinNoise(voxelPos.x * 0.085f, voxelPos.z * 0.085f) * 9.1f;


						int currentHeight = y + (SideLength * chunky);
						bool setToGrass = false;


						if (perlin1 > currentHeight)
						{
							if (perlin1 > perlin2 + currentHeight)
							{
								chunk.SetVoxelSimple(x, y, z, 2);
								setToGrass = true;
							}
						}


						currentHeight = currentHeight + 1;
						if (perlin1 > currentHeight)
						{
							if (perlin1 > perlin2 + currentHeight)
							{
								chunk.SetVoxelSimple(x, y, z, 1);
								setToGrass = false;
							}
						}

						// tree-只在有空间且是grass block时
						if (setToGrass && TreeCanFit(x, y, z))
						{
							if (Random.Range(0.0f, 1.0f) < 0.01f)
							{ // 1%概率
								AddTree(x, y + 1, z);
							}
						}

					}
				}
			}
		}


		bool TreeCanFit(int x, int y, int z)
		{
			if (x > 0 && x < Engine.ChunkSideLength - 1 && z > 0 && z < Engine.ChunkSideLength - 1 && y + 5 < Engine.ChunkSideLength)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		void AddTree(int x, int y, int z)
		{

			// 创建chunk
			for (int trunkHeight = 0; trunkHeight < 4; trunkHeight++)
			{
				chunk.SetVoxelSimple(x, y + trunkHeight, z, 6); // 放置树木
			}


			// 创建叶子
			for (int offsetY = 2; offsetY < 4; offsetY++)
			{
				for (int offsetX = -1; offsetX <= 1; offsetX++)
				{
					for (int offsetZ = -1; offsetZ <= 1; offsetZ++)
					{
						if ((offsetX == 0 && offsetZ == 0) == false)
						{
							chunk.SetVoxelSimple(x + offsetX, y + offsetY, z + offsetZ, 9);
						}
					}
				}
			}

			// add one more leaf block on top
			chunk.SetVoxel(x, y + 4, z, 9, false);
		}
	}

}