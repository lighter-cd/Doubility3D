using System;
using UnityEngine;
using Doubility3D.Resource.Schema;
using Doubility3D.Resource.Manager;
using Doubility3D.Util;

namespace Doubility3D.Resource.Unserializing
{
	public class UnserializerFactory
	{
		private UnserializerFactory ()
		{
		}

		static private UnserializerFactory _instance = null;

		static public UnserializerFactory Instance{
			get{ 
				if (_instance == null) {
					_instance = new UnserializerFactory ();
				}
				return Instance;
			}
		}

		public IUnserializer Create(Context context){
			switch (context)
			{
			case Context.Skeletons:
				return new SkeletonUnserializer ();
			case Context.Mesh:
				return new MeshUnserializer ();
			case Context.Material:
				return new MaterialUnserializer ();
			case Context.AnimationClip:
				return new AnimationClipUnserializer ();
			case Context.Texture:
				return new TextureUnserializer();
			}
			return null;
		}
	}
}

