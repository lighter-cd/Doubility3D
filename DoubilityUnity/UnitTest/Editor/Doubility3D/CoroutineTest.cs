using System.Collections;

namespace UnitTest.Doubility3D
{
	public class CoroutineTest
	{
		public static void Run(IEnumerator e)
		{
			while (e.MoveNext ()) {
				var instruction = e.Current; //the yielded object
			}
		}
	}
}

