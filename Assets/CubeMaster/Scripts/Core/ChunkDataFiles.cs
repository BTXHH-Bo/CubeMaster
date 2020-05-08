using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

//���ʹ�ü���
namespace CubeMaster {

	public class ChunkDataFiles : MonoBehaviour
	{

		public static bool SavingChunks;
		public static Dictionary<string, string> TempChunkData; // ����chunk���ݣ������д��һ���ļ�
		public static Dictionary<string, string[]> LoadedRegions; // �����chunk����

		//���ļ����������ݣ�δ�ҵ��ļ��Ļ��ͷ���false
		public bool LoadData()
		{

			Chunk chunk = GetComponent<Chunk>();
			string chunkData = GetChunkData(chunk.ChunkIndex);

			if (chunkData != "")
			{

				ChunkDataFiles.DecompressData(chunk, GetChunkData(chunk.ChunkIndex));
				chunk.VoxelsDone = true;
				return true;
			}

			else
			{
				return false;
			}
		}

		public void SaveData()
		{

			Chunk chunk = GetComponent<Chunk>();
			string compressedData = ChunkDataFiles.CompressData(chunk);

			WriteChunkData(chunk.ChunkIndex, compressedData);
		}

		//��ѹvoxel���ݣ����뵽voxeldata������
		public static void DecompressData(Chunk chunk, string data)
		{

			//chunk�Ƿ�Ϊ��
			if (data.Length == 2 && data[1] == (char)0)
			{
				chunk.Empty = true;
			}

			StringReader reader = new StringReader(data);

			int i = 0;
			int length = chunk.GetDataLength(); //voxeldata�ĳ���

			try
			{
				//��ȫ���voxeldata�����ֹͣѭ��
				while (i < length)
				{

					ushort currentCount = (ushort)reader.Read();
					ushort currentData = (ushort)reader.Read(); // read the data

					int ii = 0;

					//��ÿһ��currentcount��д��һ��voxel
					while (ii < currentCount)
					{
						chunk.SetVoxelSimple(i, (ushort)currentData);
						ii++;
						i++;
					}
				}
			}
			catch (System.Exception)
			{
				Debug.LogError("CubeMaster: ������ chunk: " + chunk.ChunkIndex.ToString() + ".");
				reader.Close();
				return;
			}

			reader.Close();

		}

		//ѹ��һ��chunk�����ݳ�һ���ַ�����������
		public static string CompressData(Chunk chunk)
		{

			StringWriter writer = new StringWriter();

			int i = 0;
			int length = chunk.GetDataLength(); // ����

			ushort currentCount = 0; //������ͬ��voxel��Ŀ
			ushort currentData = 0; // ��ǰvoxel������

			//��ÿһ��voxel
			for (i = 0; i < length; i++)
			{
				//��ȡ������
				ushort thisData = chunk.GetVoxelSimple(i);
				//���������ݺ�ǰһ�����ݲ�һ����д����һ�����ݣ�Ȼ���һ���µ�ȥд��this����
				if (thisData != currentData)
				{

					// д��current��Ҳ������һ��
					if (i != 0)
					{
						writer.Write((char)currentCount);
						writer.Write((char)currentData);
					}
					// �µ�block
					currentCount = 1;
					currentData = thisData;
				}
				//���this���ݺ���һ��currentһ�����Ǿ�����currentcount��
				else
				{
					currentCount++;
				}

				//�����һ��ѭ���йرղ�д��current
				if (i == length - 1)
				{
					writer.Write((char)currentCount);
					writer.Write((char)currentData);
				}

			}

			string compressedData = writer.ToString();
			writer.Flush();
			writer.Close();
			return compressedData;

		}

		//����chunkData��û�ҵ��ͷ��ؿմ�
		private string GetChunkData(Index index)
		{

			//���Դ�temp��Ѱ�ҷ���
			string indexString = index.ToString();
			if (TempChunkData.ContainsKey(indexString))
			{
				return TempChunkData[indexString];
			}

			int regionIndex = GetChunkRegionIndex(index);
			string[] regionData = GetRegionData(GetParentRegion(index));
			if (regionData == null)
			{
				return "";
			}
			return regionData[regionIndex];

		}

		//��chunk��dataд�뵽temp
		private void WriteChunkData(Index index, string data)
		{
			TempChunkData[index.ToString()] = data;
		}

		//�������
		private static int GetChunkRegionIndex(Index index)
		{

			Index newIndex = new Index(index.x, index.y, index.z);
			if (newIndex.x < 0) newIndex.x = -newIndex.x - 1;
			if (newIndex.y < 0) newIndex.y = -newIndex.y - 1;
			if (newIndex.z < 0) newIndex.z = -newIndex.z - 1;

			int flatIndex = (newIndex.z * 100) + (newIndex.y * 10) + newIndex.x;

			while (flatIndex > 999)
			{
				flatIndex -= 1000;
			}

			return flatIndex;
		}

		private static string[] GetRegionData(Index regionIndex)
		{

			if (LoadRegionData(regionIndex) == true)
			{
				return LoadedRegions[regionIndex.ToString()];
			}
			else
			{
				return null;
			}
		}

		//����������data���뵽�ڴ��У����ļ���
		private static bool LoadRegionData(Index regionIndex)
		{

			string indexString = regionIndex.ToString();
			//���û������
			if (LoadedRegions.ContainsKey(indexString) == false)
			{

				// ����ļ����ڣ�������
				string regionPath = GetRegionPath(regionIndex);
				if (File.Exists(regionPath))
				{

					StreamReader reader = new StreamReader(regionPath);
					string[] regionData = reader.ReadToEnd().Split((char)ushort.MaxValue);
					reader.Close();
					LoadedRegions[indexString] = regionData;

					return true;

				}

				else
				{
					//�ļ�������
					return false;
				}
			}
			//�����Ѿ�����			
			return true;
		}

		//��ȡ������Ӧ���ļ���
		private static string GetRegionPath(Index regionIndex)
		{

			return Engine.WorldPath + (regionIndex.ToString() + ",.region");
		}


		private static Index GetParentRegion(Index index)
		{

			Index newIndex = new Index(index.x, index.y, index.z);

			if (index.x < 0) newIndex.x -= 9;
			if (index.y < 0) newIndex.y -= 9;
			if (index.z < 0) newIndex.z -= 9;

			int x = newIndex.x / 10;
			int y = newIndex.y / 10;
			int z = newIndex.z / 10;

			return new Index(x, y, z);
		}

		//����һ���յ��ļ�
		private static void CreateRegionFile(Index index)
		{ // creates an empty region file

			Directory.CreateDirectory(Engine.WorldPath);
			StreamWriter writer = new StreamWriter(GetRegionPath(index));

			for (int i = 0; i < 999; i++)
			{
				writer.Write((char)ushort.MaxValue);
			}

			writer.Flush();
			writer.Close();
		}

		public static IEnumerator SaveAllChunks()
		{

			if (!Engine.SaveVoxelData)
			{
				Debug.LogWarning("CubeMaster: �������档����Engine�����á�");
				yield break;
			}

			while (ChunkDataFiles.SavingChunks)
			{
				yield return new WaitForEndOfFrame();
			}
			ChunkDataFiles.SavingChunks = true;

			//����ÿһ��chunk�������ݱ��浽�ڴ���
			int count = 0;
			List<Chunk> chunksToSave = new List<Chunk>(ChunkManager.Chunks.Values);


			foreach (Chunk chunk in chunksToSave)
			{
				chunk.gameObject.GetComponent<ChunkDataFiles>().SaveData();
				count++;
				if (count > Engine.MaxChunkSaves)
				{
					yield return new WaitForEndOfFrame();
					count = 0;
				}
			}

			// ������д�뵽����
			ChunkDataFiles.WriteLoadedChunks();
			ChunkDataFiles.SavingChunks = false;

			Debug.Log("CubeMaster: ���浱ǰ����ɹ���");
		}

		//��data��tempд���ļ�
		public static void SaveAllChunksInstant()
		{

			if (!Engine.SaveVoxelData)
			{
				Debug.LogWarning("CubeMaster: �������档����Engine�����á�");
				return;
			}

			// ��ÿһ��chunk��������д���ڴ�
			foreach (Chunk chunk in ChunkManager.Chunks.Values)
			{
				chunk.gameObject.GetComponent<ChunkDataFiles>().SaveData();
			}

			// д�뱾��
			ChunkDataFiles.WriteLoadedChunks();

			Debug.Log("CubeMaster: World saved successfully. (Instant)");

		}

		//��data���ڴ�д�뱾�أ���������ڴ�
		public static void WriteLoadedChunks()
		{

			// ��ÿһ��temp�������chunk
			foreach (string chunkIndex in TempChunkData.Keys)
			{

				Index index = Index.FromString(chunkIndex);
				string region = GetParentRegion(index).ToString();

				// ���һ�������Ƿ��Ѿ����룬û�о�����
				if (LoadRegionData(GetParentRegion(index)) == false)
				{
					CreateRegionFile(GetParentRegion(index));
					LoadRegionData(GetParentRegion(index));
				}

				// ��chunk����д����ɶ
				int chunkRegionIndex = GetChunkRegionIndex(index);
				LoadedRegions[region][chunkRegionIndex] = TempChunkData[chunkIndex];
			}
			TempChunkData.Clear();


			// ��ÿһ������Ķ�д��
			foreach (string regionIndex in LoadedRegions.Keys)
			{
				WriteRegionFile(regionIndex);
			}
			LoadedRegions.Clear();

		}

		private static void WriteRegionFile(string regionIndex)
		{

			string[] regionData = LoadedRegions[regionIndex];

			StreamWriter writer = new StreamWriter(GetRegionPath(Index.FromString(regionIndex)));
			int count = 0;
			foreach (string chunk in regionData)
			{
				writer.Write(chunk);
				if (count != regionData.Length - 1)
				{
					writer.Write((char)ushort.MaxValue);
				}
				count++;
			}

			writer.Flush();
			writer.Close();
		}
	}

}