using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

//这个使用即可
namespace CubeMaster {

	public class ChunkDataFiles : MonoBehaviour
	{

		public static bool SavingChunks;
		public static Dictionary<string, string> TempChunkData; // 保存chunk数据，后面会写入一个文件
		public static Dictionary<string, string[]> LoadedRegions; // 载入的chunk数据

		//从文件中载入数据，未找到文件的话就返回false
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

		//解压voxel数据，载入到voxeldata数组中
		public static void DecompressData(Chunk chunk, string data)
		{

			//chunk是否为空
			if (data.Length == 2 && data[1] == (char)0)
			{
				chunk.Empty = true;
			}

			StringReader reader = new StringReader(data);

			int i = 0;
			int length = chunk.GetDataLength(); //voxeldata的长度

			try
			{
				//完全填充voxeldata数组后，停止循环
				while (i < length)
				{

					ushort currentCount = (ushort)reader.Read();
					ushort currentData = (ushort)reader.Read(); // read the data

					int ii = 0;

					//对每一个currentcount，写入一个voxel
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
				Debug.LogError("CubeMaster: 数据损坏 chunk: " + chunk.ChunkIndex.ToString() + ".");
				reader.Close();
				return;
			}

			reader.Close();

		}

		//压缩一个chunk的数据成一个字符串，并返回
		public static string CompressData(Chunk chunk)
		{

			StringWriter writer = new StringWriter();

			int i = 0;
			int length = chunk.GetDataLength(); // 长度

			ushort currentCount = 0; //连续的同类voxel数目
			ushort currentData = 0; // 当前voxel的数据

			//对每一个voxel
			for (i = 0; i < length; i++)
			{
				//读取行数据
				ushort thisData = chunk.GetVoxelSimple(i);
				//如果这个数据和前一个数据不一样，写入上一个数据，然后搞一个新的去写入this数据
				if (thisData != currentData)
				{

					// 写如current，也就是上一个
					if (i != 0)
					{
						writer.Write((char)currentCount);
						writer.Write((char)currentData);
					}
					// 新的block
					currentCount = 1;
					currentData = thisData;
				}
				//如果this数据和上一个current一样，那就自增currentcount。
				else
				{
					currentCount++;
				}

				//在最后一次循环中关闭并写入current
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

		//返回chunkData，没找到就返回空串
		private string GetChunkData(Index index)
		{

			//尝试从temp中寻找返回
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

		//将chunk的data写入到temp
		private void WriteChunkData(Index index, string data)
		{
			TempChunkData[index.ToString()] = data;
		}

		//返回这个
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

		//把这个区域的data载入到内存中（从文件）
		private static bool LoadRegionData(Index regionIndex)
		{

			string indexString = regionIndex.ToString();
			//如果没有载入
			if (LoadedRegions.ContainsKey(indexString) == false)
			{

				// 如果文件存在，就载入
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
					//文件不存在
					return false;
				}
			}
			//数据已经载入			
			return true;
		}

		//获取区域块对应的文件名
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

		//创造一个空的文件
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
				Debug.LogWarning("CubeMaster: 不允许保存。请在Engine中设置。");
				yield break;
			}

			while (ChunkDataFiles.SavingChunks)
			{
				yield return new WaitForEndOfFrame();
			}
			ChunkDataFiles.SavingChunks = true;

			//对于每一个chunk，把数据保存到内存中
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

			// 把数据写入到本地
			ChunkDataFiles.WriteLoadedChunks();
			ChunkDataFiles.SavingChunks = false;

			Debug.Log("CubeMaster: 保存当前世界成功。");
		}

		//将data从temp写入文件
		public static void SaveAllChunksInstant()
		{

			if (!Engine.SaveVoxelData)
			{
				Debug.LogWarning("CubeMaster: 不允许保存。请在Engine中设置。");
				return;
			}

			// 对每一个chunk，把数据写入内存
			foreach (Chunk chunk in ChunkManager.Chunks.Values)
			{
				chunk.gameObject.GetComponent<ChunkDataFiles>().SaveData();
			}

			// 写入本地
			ChunkDataFiles.WriteLoadedChunks();

			Debug.Log("CubeMaster: World saved successfully. (Instant)");

		}

		//将data从内存写入本地，而后清除内存
		public static void WriteLoadedChunks()
		{

			// 对每一个temp中载入的chunk
			foreach (string chunkIndex in TempChunkData.Keys)
			{

				Index index = Index.FromString(chunkIndex);
				string region = GetParentRegion(index).ToString();

				// 检查一个区域是否已经载入，没有就在如
				if (LoadRegionData(GetParentRegion(index)) == false)
				{
					CreateRegionFile(GetParentRegion(index));
					LoadRegionData(GetParentRegion(index));
				}

				// 将chunk数据写入那啥
				int chunkRegionIndex = GetChunkRegionIndex(index);
				LoadedRegions[region][chunkRegionIndex] = TempChunkData[chunkIndex];
			}
			TempChunkData.Clear();


			// 把每一个区域的都写入
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