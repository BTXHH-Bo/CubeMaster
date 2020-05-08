using UnityEngine;
using System.Collections;

// ��1��9������

namespace CubeMaster
{

	public class ExampleInventory : MonoBehaviour
	{

		public static ushort HeldBlock;

		public void Update()
		{

			// change held block with 1-9 keys
			for (ushort i = 0; i < 10; i++)
			{
				if (Input.GetKeyDown(i.ToString()))
				{
					if (Engine.GetVoxelType(i) != null)
					{
						ExampleInventory.HeldBlock = i;
						Debug.Log("Held block is now:" + i.ToString());
					}
				}
			}

		}
	}
}