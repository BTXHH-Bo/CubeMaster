using UnityEngine;
using System.Collections;

//生成平坦地形

namespace CubeMaster
{

	public class FlatTerrainGenerator : TerrainGenerator
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
					{

						int currentHeight = y + (SideLength * chunky);

						if (currentHeight < 8)
						{
							chunk.SetVoxelSimple(x, y, z, 1); //放泥巴
						}


					}
				}
			}
		}
	}

}