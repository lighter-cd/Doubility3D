using System;
using System.Collections;

namespace Doubility3D.Util
{
	public class CoroutineRunner
	{
		private CoroutineRunner ()
		{
		}

		static private CoroutineRunner _instance = null;

		static public CoroutineRunner Instance {
			get { 
				if (_instance == null) {
					_instance = new CoroutineRunner ();
				}
				return _instance;
			}
		}
		public Action<IEnumerator> ActRunner { get; set;} 
		public void Run(IEnumerator e){
			ActRunner (e);
		}
	}
}

