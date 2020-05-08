

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

// ����chunk�����������괴��

namespace CubeMaster {

	public class ChunkManager : MonoBehaviour
	{

		public GameObject ChunkObject; // Chunk prefab

		// chunk��
		public static Dictionary<string, Chunk> Chunks;
		private static List<Chunk> ChunkUpdateQueue; //���ո��µ����ȼ��洢chunk�ǣ�����ProcessChunkQueue
		private static List<Chunk> ChunksToDestroy; // �������Ҫ�������chunks

		public static int SavesThisFrame;


		// global flags
		public static bool SpawningChunks; // ����spawning
		public static bool StopSpawning;
		public static bool Initialized;

		// local flags	
		private bool Done;
		private Index LastRequest;
		private float targetFrameDuration;
		private Stopwatch frameStopwatch;
		private int SpawnQueue;

		void Start()
		{
			targetFrameDuration = 1f / Engine.TargetFPS;

			ChunkManager.Chunks = new Dictionary<string, Chunk>();
			ChunkManager.ChunkUpdateQueue = new List<Chunk>();
			frameStopwatch = new Stopwatch();

			Engine.ChunkScale = ChunkObject.transform.localScale;
			ChunkObject.GetComponent<Chunk>().MeshContainer.transform.localScale = ChunkObject.transform.localScale;
			ChunkObject.GetComponent<Chunk>().ChunkCollider.transform.localScale = ChunkObject.transform.localScale;

			Done = true;
			ChunkManager.SpawningChunks = false;

			ChunkManager.Initialized = true;
		}

		private void ResetFrameStopwatch()
		{
			frameStopwatch.Stop();
			frameStopwatch.Reset();
			frameStopwatch.Start();
		}

		public static void AddChunkToUpdateQueue(Chunk chunk)
		{
			if (ChunkUpdateQueue.Contains(chunk) == false)
			{
				ChunkUpdateQueue.Add(chunk);
			}
		}

		private void ProcessChunkQueue()
		{ // Spawnchunks���ã�����chunk mesh

			// ���µ�һ��chunk��Ȼ��Ӷ�����ɾ��
			Chunk currentChunk = ChunkUpdateQueue[0];

			if (!currentChunk.Empty && !currentChunk.DisableMesh)
			{
				currentChunk.RebuildMesh();
			}
			currentChunk.Fresh = false;
			ChunkUpdateQueue.RemoveAt(0);
		}

		private bool ProcessChunkQueueLoopActive;
		private IEnumerator ProcessChunkQueueLoop()
		{ // ��SpawnChunksû������ʱ����Update�е���
			ProcessChunkQueueLoopActive = true;
			while (ChunkUpdateQueue.Count > 0 && !SpawningChunks && !StopSpawning)
			{
				ProcessChunkQueue();
				if (frameStopwatch.Elapsed.TotalSeconds >= targetFrameDuration)
				{
					yield return new WaitForEndOfFrame();
				}
			}
			ProcessChunkQueueLoopActive = false;
		}

		public static void RegisterChunk(Chunk chunk)
		{ //���뵽ȫ�ֵ�chunk�б���
			ChunkManager.Chunks.Add(chunk.ChunkIndex.ToString(), chunk);
		}

		public static void UnregisterChunk(Chunk chunk)
		{
			ChunkManager.Chunks.Remove(chunk.ChunkIndex.ToString());
		}

		public static GameObject GetChunk(int x, int y, int z)
		{ //������������µ�chunkIndex����null��

			return GetChunk(new Index(x, y, z));
		}

		public static GameObject GetChunk(Index index)
		{//��Ҳ��֪��д�������Ҫ��Ҫ�ã���д�˰ɣ����Ӵ����

			Chunk chunk = ChunkManager.GetChunkComponent(index);
			if (chunk == null)
			{
				return null;
			}
			else
			{
				return chunk.gameObject;
			}

		}

		public static Chunk GetChunkComponent(int x, int y, int z)
		{

			return GetChunkComponent(new Index(x, y, z));
		}

		public static Chunk GetChunkComponent(Index index)
		{

			string indexString = index.ToString();
			if (ChunkManager.Chunks.ContainsKey(indexString))
			{
				return ChunkManager.Chunks[indexString];
			}
			else
			{
				return null;
			}
		}


		// ==== ����chunk�� ====

		public static GameObject SpawnChunk(int x, int y, int z)
		{ // spawnһ��������chunk

			GameObject chunk = ChunkManager.GetChunk(x, y, z);
			if (chunk == null)
			{
				return Engine.ChunkManagerInstance.DoSpawnChunk(new Index(x, y, z));
			}
			else return chunk;
		}

		public static GameObject SpawnChunk(Index index)
		{

			GameObject chunk = ChunkManager.GetChunk(index);
			if (chunk == null)
			{
				return Engine.ChunkManagerInstance.DoSpawnChunk(index);
			}
			else return chunk;
		}


		GameObject DoSpawnChunk(Index index)
		{
			GameObject chunkObject = Instantiate(ChunkObject, index.ToVector3(), transform.rotation) as GameObject;
			Chunk chunk = chunkObject.GetComponent<Chunk>();
			AddChunkToUpdateQueue(chunk);
			return chunkObject;
		}

		public static void SpawnChunks(float x, float y, float z)
		{ // ͨ����������
			Index index = Engine.PositionToChunkIndex(new Vector3(x, y, z));
			Engine.ChunkManagerInstance.TrySpawnChunks(index);
		}
		public static void SpawnChunks(Vector3 position)
		{//���Ӵ����
			Index index = Engine.PositionToChunkIndex(position);
			Engine.ChunkManagerInstance.TrySpawnChunks(index);
		}

		public static void SpawnChunks(int x, int y, int z)
		{ //���Ӵ����2����չ��ʱ������õ�����
			Engine.ChunkManagerInstance.TrySpawnChunks(x, y, z);
		}
		public static void SpawnChunks(Index index)
		{
			Engine.ChunkManagerInstance.TrySpawnChunks(index.x, index.y, index.z);
		}

		private void TrySpawnChunks(Index index)
		{
			TrySpawnChunks(index.x, index.y, index.z);
		}
		private void TrySpawnChunks(int x, int y, int z)
		{//����ǲο���

			if (Done == true)
			{ // ������spawn
				StartSpawnChunks(x, y, z);
			}
			else
			{ // if we are spawning chunks already, flag to spawn again once the previous round is finished using the last requested position as origin.
				LastRequest = new Index(x, y, z);
				SpawnQueue = 1;
				StopSpawning = true;
				ChunkUpdateQueue.Clear();
			}
		}






		public void Update()
		{

			//����Ѿ���һ��chunk spawn queue׼�����ˣ�����ǰһ��Ҳok��
			if (SpawnQueue == 1 && Done == true)
			{
				SpawnQueue = 0;
				StartSpawnChunks(LastRequest.x, LastRequest.y, LastRequest.z);
			}

			// �������û��spawn�ģ��ʹ���һ���Ѿ��źöӵ�chunk��
			if (!SpawningChunks && !ProcessChunkQueueLoopActive && ChunkUpdateQueue != null && ChunkUpdateQueue.Count > 0)
			{
				StartCoroutine(ProcessChunkQueueLoop());
			}

			ResetFrameStopwatch();
		}


		private void StartSpawnChunks(int originX, int originY, int originZ)
		{

			ChunkManager.SpawningChunks = true;
			Done = false;

			int range = Engine.ChunkSpawnDistance;

			StartCoroutine(SpawnMissingChunks(originX, originY, originZ, range));

		}

		//�н���ο�
		private IEnumerator SpawnMissingChunks(int originX, int originY, int originZ, int range)
		{

			int heightRange = Engine.HeightRange;

			//��ո��¶���updatequeue
			ChunkUpdateQueue = new List<Chunk>();

			//flag chunk���ڷ�Χ��
			ChunksToDestroy = new List<Chunk>();
			foreach (Chunk chunk in Chunks.Values)
			{
				if (Vector2.Distance(new Vector2(chunk.ChunkIndex.x, chunk.ChunkIndex.z), new Vector2(originX, originZ)) > range + Engine.ChunkDespawnDistance)
				{
					ChunksToDestroy.Add(chunk);
				}
				else if (Mathf.Abs(chunk.ChunkIndex.y - originY) > range + Engine.ChunkDespawnDistance)
				{
					ChunksToDestroy.Add(chunk);
				}
			}


			// main loop==�������ˣ�ԭע�ͱ���
			for (int currentLoop = 0; currentLoop <= range; currentLoop++)
			{
				for (var x = originX - currentLoop; x <= originX + currentLoop; x++)
				{ // iterate through all potential chunk indexes within range
					for (var y = originY - currentLoop; y <= originY + currentLoop; y++)
					{
						for (var z = originZ - currentLoop; z <= originZ + currentLoop; z++)
						{

							if (Mathf.Abs(y) <= heightRange)
							{ // skip chunks outside of height range
								if (Mathf.Abs(originX - x) + Mathf.Abs(originZ - z) < range * 1.3f)
								{ // skip corners

									// pause loop while the queue is not empty
									while (ChunkUpdateQueue.Count > 0)
									{
										ProcessChunkQueue();
										if (frameStopwatch.Elapsed.TotalSeconds >= targetFrameDuration)
										{
											yield return new WaitForEndOfFrame();
										}
									}

									Chunk currentChunk = ChunkManager.GetChunkComponent(x, y, z);


									// chunks that already exist but haven't had their mesh built yet should be added to the update queue
									if (currentChunk != null)
									{

										// chunks without meshes spawned by server should be changed to regular chunks
										if (currentChunk.DisableMesh || currentChunk.EnableTimeout)
										{
											currentChunk.DisableMesh = false;
											currentChunk.EnableTimeout = false;
											currentChunk.Fresh = true;
										}

										if (currentChunk.Fresh)
										{

											// spawn neighbor chunks
											for (int d = 0; d < 6; d++)
											{
												Index neighborIndex = currentChunk.ChunkIndex.GetAdjacentIndex((Direction)d);
												GameObject neighborChunk = GetChunk(neighborIndex);
												if (neighborChunk == null)
												{
													neighborChunk = Instantiate(ChunkObject, neighborIndex.ToVector3(), transform.rotation) as GameObject;
												}
												currentChunk.NeighborChunks[d] = neighborChunk.GetComponent<Chunk>(); // always add the neighbor to NeighborChunks, in case it's not there already

												// continue loop in next frame if the current frame time is exceeded
												if (frameStopwatch.Elapsed.TotalSeconds >= targetFrameDuration)
												{
													yield return new WaitForEndOfFrame();
												}
												if (StopSpawning)
												{
													EndSequence();
													yield break;
												}
											}

											if (currentChunk != null)
												currentChunk.AddToQueueWhenReady();
										}


									}

									else
									{ // if chunk doesn't exist, create new chunk (it adds itself to the update queue when its data is ready)

										// spawn chunk
										GameObject newChunk = Instantiate(ChunkObject, new Vector3(x, y, z), transform.rotation) as GameObject; // Spawn a new chunk.
										currentChunk = newChunk.GetComponent<Chunk>();

										// spawn neighbor chunks if they're not spawned yet
										for (int d = 0; d < 6; d++)
										{
											Index neighborIndex = currentChunk.ChunkIndex.GetAdjacentIndex((Direction)d);
											GameObject neighborChunk = GetChunk(neighborIndex);
											if (neighborChunk == null)
											{
												neighborChunk = Instantiate(ChunkObject, neighborIndex.ToVector3(), transform.rotation) as GameObject;
											}
											currentChunk.NeighborChunks[d] = neighborChunk.GetComponent<Chunk>(); // always add the neighbor to NeighborChunks, in case it's not there already

											// continue loop in next frame if the current frame time is exceeded
											if (frameStopwatch.Elapsed.TotalSeconds >= targetFrameDuration)
											{
												yield return new WaitForEndOfFrame();
											}
											if (StopSpawning)
											{
												EndSequence();
												yield break;
											}
										}

										if (currentChunk != null)
											currentChunk.AddToQueueWhenReady();



									}

								}
							}



							// continue loop in next frame if the current frame time is exceeded
							if (frameStopwatch.Elapsed.TotalSeconds >= targetFrameDuration)
							{
								yield return new WaitForEndOfFrame();
							}
							if (StopSpawning)
							{
								EndSequence();
								yield break;
							}


						}
					}
				}
			}

			yield return new WaitForEndOfFrame();
			EndSequence();
		}





		private void EndSequence()
		{

			ChunkManager.SpawningChunks = false;
			Resources.UnloadUnusedAssets();
			Done = true;
			StopSpawning = false;

			foreach (Chunk chunk in ChunksToDestroy)
			{
				chunk.FlagToRemove();
			}

		}

	}

}

