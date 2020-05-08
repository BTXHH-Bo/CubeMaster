using UnityEngine;
using System.Collections;

//��ǰʵ��
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
					{ // ��һ��chunk����������

						Vector3 voxelPos = chunk.VoxelIndexToPosition(x, y, z); //������ؾ�������
						voxelPos = new Vector3(voxelPos.x + seed, voxelPos.y, voxelPos.z + seed); //ƫ�ƣ��������ӣ�

						float perlin1 = Mathf.PerlinNoise(voxelPos.x * 0.010f, voxelPos.z * 0.010f) * 70.1f; //ɽ
						float perlin2 = Mathf.PerlinNoise(voxelPos.x * 0.085f, voxelPos.z * 0.085f) * 9.1f; //ƽ��


						int currentHeight = y + (SideLength * chunky); //������صľ��Ը߶�

						//�ݵ�
						if (perlin1 > currentHeight)
						{
							if (perlin1 > perlin2 + currentHeight)
							{
								chunk.SetVoxelSimple(x, y, z, 2);   // ����grass blocks

							}
						}

						//�����
						currentHeight = currentHeight + 1; // ƫ��1
						if (perlin1 > currentHeight)
						{
							if (perlin1 > perlin2 + currentHeight)
							{
								chunk.SetVoxelSimple(x, y, z, 1);
							}
						}

						// debug
						//if (random == 1) {
						//chunk.SetVoxelSimple(x,y,z, 3); // ���
						//}

					}
				}
			}
		}
	}

}