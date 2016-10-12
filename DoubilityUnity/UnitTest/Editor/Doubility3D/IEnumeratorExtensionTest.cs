using UnityEngine;
using System.Collections;
using NUnit.Framework;

namespace Tests.Extensions
{
	public class CoroutineBehaviour : MonoBehaviour
	{
		public int current;

		public IEnumerator OneYield ()
		{
			current++;
			yield return null;
			current++;
		}

		public IEnumerator WaitForSecondsCoroutine ()
		{
			current++;
			yield return new WaitForSeconds (1);
			current++;
		}

		public IEnumerator WaitForFixedUpdateCoroutine ()
		{
			current++;
			yield return new WaitForFixedUpdate ();
			current++;
		}

		public IEnumerator WaitForEndOfFrameCoroutine ()
		{
			current++;
			yield return new WaitForEndOfFrame ();
			current++;
		}

		public IEnumerator WaitWhileCoroutine ()
		{
			current++;
			int waitCounter = 0;
			yield return new WaitWhile (() => {
				waitCounter++;
				return waitCounter <= 2;
			});
			current++;
		}

		public IEnumerator WaitUntilCoroutine ()
		{
			current++;
			int waitCounter = 0;
			yield return new WaitUntil (() => {
				waitCounter++;
				return waitCounter == 2;
			});
			current++;
		}

		public IEnumerator NestedIEnumerator ()
		{
			current++;
			yield return null;
			current++;
			yield return OneYield ();
			current++;
		}

		public IEnumerator NestedCoroutine ()
		{
			current++;
			yield return null;
			current++;
			yield return StartCoroutine (OneYield ());
			current++;
		}

		public IEnumerator InfiniteLoopCoroutine ()
		{
			while (true) {
				current++;
				yield return null;
			}
		}

		public IEnumerator WWWCoroutine(){
			current++;
			WWW www = new WWW ("http://127.0.0.1");
			while (!www.isDone) {
				yield return www;
			}
			current++;
			UnityEngine.Debug.Log(www.text);
		}
	}

	[TestFixture]
	public class IEnumeratorExtensionTest
	{
		GameObject obj;
		CoroutineBehaviour behaviour;

		[SetUp]
		public void Setup ()
		{
			obj = new GameObject ();
			behaviour = obj.AddComponent<CoroutineBehaviour> ();
		}

		[TearDown]
		public void Teardown ()
		{
			GameObject.DestroyImmediate (obj);
		}


		[Test]
		public void RunCoroutineWithoutYields_OneYieldReturnNull_Completes ()
		{
			IEnumerator enumerator = behaviour.OneYield ();
			bool completed = enumerator.RunCoroutineWithoutYields (1000);

			Assert.AreEqual (2, behaviour.current);
			Assert.IsTrue (completed);
		}

		[Test]
		public void RunCoroutineWithoutYields_WaitForSeconds_Completes ()
		{
			IEnumerator enumerator = behaviour.WaitForSecondsCoroutine ();
			bool completed = enumerator.RunCoroutineWithoutYields (1000);

			Assert.AreEqual (2, behaviour.current);
			Assert.IsTrue (completed);
		}

		[Test]
		public void RunCoroutineWithoutYields_WaitForFixedUpdate_Completes ()
		{
			IEnumerator enumerator = behaviour.WaitForFixedUpdateCoroutine ();
			bool completed = enumerator.RunCoroutineWithoutYields (1000);

			Assert.AreEqual (2, behaviour.current);
			Assert.IsTrue (completed);
		}

		[Test]
		public void RunCoroutineWithoutYields_WaitForEndOfFrame_Completes ()
		{
			IEnumerator enumerator = behaviour.WaitForEndOfFrameCoroutine ();
			bool completed = enumerator.RunCoroutineWithoutYields (1000);

			Assert.AreEqual (2, behaviour.current);
			Assert.IsTrue (completed);
		}

		[Test]
		public void RunCoroutineWithoutYields_WaitWhile_Completes ()
		{
			IEnumerator enumerator = behaviour.WaitWhileCoroutine ();
			bool completed = enumerator.RunCoroutineWithoutYields (1000);

			Assert.AreEqual (2, behaviour.current);
			Assert.IsTrue (completed);
		}

		[Test]
		public void RunCoroutineWithoutYields_WaitUntil_Completes ()
		{
			IEnumerator enumerator = behaviour.WaitUntilCoroutine ();
			bool completed = enumerator.RunCoroutineWithoutYields (1000);

			Assert.AreEqual (2, behaviour.current);
			Assert.IsTrue (completed);
		}

		[Test]
		public void RunCoroutineWithoutYields_NestedIEnumerator_Completes ()
		{
			IEnumerator enumerator = behaviour.NestedIEnumerator ();
			bool completed = enumerator.RunCoroutineWithoutYields (1000);

			Assert.AreEqual (5, behaviour.current);
			Assert.IsTrue (completed);
		}

		[Test]
		[ExpectedException (typeof(System.NotSupportedException))]
		public void RunCoroutineWithoutYields_NestedStartCoroutine_ThrowsException ()
		{
			IEnumerator enumerator = behaviour.NestedCoroutine ();
			enumerator.RunCoroutineWithoutYields (1000);
		}

		[Test]
		public void RunCoroutineWithoutYields_InfiniteLoop_BailsOut ()
		{
			int maxYields = 10;
			IEnumerator enumerator = behaviour.InfiniteLoopCoroutine ();
			bool completed = enumerator.RunCoroutineWithoutYields (maxYields);

			Assert.AreEqual (maxYields, behaviour.current);
			Assert.IsFalse (completed);
		}
		[Test]
		public void RunCoroutineWWW(){
			IEnumerator enumerator = behaviour.WWWCoroutine ();
			bool completed = enumerator.RunCoroutineWithoutYields (int.MaxValue);

			Assert.Less (behaviour.current,int.MaxValue);
			Assert.IsTrue (completed);

		}
	}
}