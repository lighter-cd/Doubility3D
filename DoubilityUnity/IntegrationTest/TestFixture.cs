using UnityEngine;
using System.Collections;

namespace Integration
{
	public class TestFixture : MonoBehaviour
	{
		int tests = 0;
		public void AddTest(){
			if (tests == 0) {
				TestFixtureSetup ();
			}
			tests++;
		}
		public void DelTest(){
			tests--;
			if (tests <= 0) {
				TestFixtureTeardown ();
			}
		}
		protected virtual void TestFixtureSetup(){
		}
		protected virtual void TestFixtureTeardown(){
		}
	}
}