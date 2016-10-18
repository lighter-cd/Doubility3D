using UnityEngine;
using System.Collections;
using Integration;

namespace Integration.ShaderManagerTest
{
	public class BundleLoadError : TestCase
	{

		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
			IntegrationTest.Pass ();
		}
		void OnDisable(){
			Debug.Log (name + ".OnDisable");
		}
	}
}