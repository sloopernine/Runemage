using UnityEngine;
using PDollarGestureRecognizer;

namespace _Content.Scripts.Data.Containers.GlobalSignal
{
	public class RuneData : GlobalSignalBaseData
	{
		public Vector3 position;
		public Vector3 angle;
		public Vector3 scale;

		public Result result;

		public RuneData()
		{
			
		}

		public RuneData(Result newResult, Vector3 newPosition)
		{
			result = newResult;
			position = newPosition;
		}
	}
}