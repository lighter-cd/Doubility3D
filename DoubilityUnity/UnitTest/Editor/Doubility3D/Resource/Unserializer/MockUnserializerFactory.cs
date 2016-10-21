using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using Doubility3D.Resource.Unserializing;
using Doubility3D.Resource.ResourceObj;

namespace UnitTest.Doubility3D.Resource.Unserializing
{
	class FakeUnserializerFactory : UnserializerFactory{
		Func<byte[],ResourceObject> funcUnserialize;
		public FakeUnserializerFactory(Func<byte[],ResourceObject> func):base(){
			funcUnserialize = func;
		}
		override public ResourceObject Unserialize(byte[] bytes){
			return funcUnserialize (bytes);
		}
	}

	public static class MockUnserializerFactory
	{
		static private void SetInstance(UnserializerFactory factory){
			Assembly assembly = Assembly.GetAssembly(typeof(UnserializerFactory));
			Type type = assembly.GetType("Doubility3D.Resource.Unserializing.UnserializerFactory");

			//UnserializerFactory  uf = UnserializerFactory.Instance;
			BindingFlags flag = BindingFlags.NonPublic | BindingFlags.Static;
			FieldInfo f_key = type.GetField("_instance", flag);
			f_key.SetValue(null, factory);
		}

		static public void Initialize (Func<byte[],ResourceObject> funcUnserialize)
		{
			SetInstance(new FakeUnserializerFactory(funcUnserialize));
		}
		static public void CleanUp()
		{
			SetInstance(null);
		}
	}
}

