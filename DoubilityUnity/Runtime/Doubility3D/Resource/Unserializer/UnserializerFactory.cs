using System;
using UnityEngine;
using FlatBuffers;
using Doubility3D.Resource.Schema;
using Doubility3D.Resource.Manager;
using Doubility3D.Util;
using Doubility3D.Resource.ResourceObj;

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
				return _instance;
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
		public ResourceObject Unserialize(byte[] bytes){
			Schema.Context context = Context.Unknown;
			ByteBuffer bb = FileUnserializer.Load(bytes, out context);
			IUnserializer serializer = Create (context);
			if (serializer != null) {
				ResourceObject resourceObject = serializer.Parse (bb);
				serializer = null;
				return resourceObject;
			} 
			return null;
		}
	}
}

