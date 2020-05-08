using UnityEngine;
using System.Collections;

// ���������Χ��chunk

namespace CubeMaster
{

	public class ChunkLoader : MonoBehaviour
	{

		private Index LastPos;
		private Index currentPos;

		void Awake()
		{
		}

		public void Update()
		{

			// δ��ʼ���򲻴�����EngineҪ���ȳ�ʼ����
			if (!Engine.Initialized || !ChunkManager.Initialized)
			{
				return;
			}

			//׷��player��ǰ������chunk
			currentPos = Engine.PositionToChunkIndex(transform.position);

			if (currentPos.IsEqual(LastPos) == false)
			{
				ChunkManager.SpawnChunks(currentPos.x, currentPos.y, currentPos.z);
			}

			LastPos = currentPos;
		}

		IEnumerator InitialPositionAndRangeUpdate()
		{
			yield return new WaitForEndOfFrame();

		}
	}

}