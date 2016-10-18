using UnityEngine;
using System.Collections;

namespace Integration{

	public class TestCase : MonoBehaviour
	{
		protected TestFixture testFixture;
		void Awake(){
			Debug.Log (name + ".Awake");		
			Debug.Assert (transform.parent != null);
			testFixture = transform.parent.gameObject.GetComponent<TestFixture> ();
			Debug.Assert (testFixture != null);
			testFixture.AddTest ();
		}
		void OnDestroy() {
			Debug.Log (name + ".OnDestroy");
			testFixture.DelTest ();
		}
	}
}