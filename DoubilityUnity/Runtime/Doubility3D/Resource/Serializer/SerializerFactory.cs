using System;
using Doubility3D.Resource.Schema;

namespace Doubility3D.Resource.Serializer
{
	public class SerializerFactory
	{
		private SerializerFactory ()
		{
		}

		static private SerializerFactory _instance = null;

		static public SerializerFactory Instance{
			get{ 
				if (_instance == null) {
					_instance = new SerializerFactory ();
				}
				return Instance;
			}
		}

		public ISerializer Create(Context context){
			switch (context)
			{
			case Context.Skeletons:
				//result = SkeletonSerializer.Load(bb);
				break;
			case Context.Mesh:
				{
					//result = MeshSerializer.Load(bb, out joints);
				}
				break;
			case Context.Material:
				{
					//result = MaterialSerializer.Load(bb, GetShader, GetTexture);
				}
				break;
			case Context.AnimationClip:
				return new AnimationClipSerializer ();
			case Context.Texture:
				return new TextureSerializer();
			}
			return null;
		}
	}
}

