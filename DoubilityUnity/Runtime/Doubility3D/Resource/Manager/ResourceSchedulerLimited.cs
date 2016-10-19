using System;
using System.Collections.Generic;

namespace Doubility3D.Resource.Manager
{
	public class ResourceSchedulerLimited : IResourceScheduler
	{
		class Processor
		{
			public ResourceRef current = null;
		}

		Processor[] processor;

		public ResourceSchedulerLimited (int numberOfProcessor)
		{
			processor = new Processor[numberOfProcessor];
			for (int i = 0; i < numberOfProcessor; i++) {
				processor [i] = new Processor ();
			}
		}
		public void ProcessQueue (PriorityQueue<ResourceRef> queueResources)
		{
			for (int i = 0; i < processor.Length; i++) {
				if ((processor[i].current == null) && (queueResources.Count > 0)) {
					ResourceRef resource = queueResources.Pop ();

					// 尝试取到一个引用计数不为0的资源
					while (resource.Refs == 0 && queueResources.Count > 0) {
						resource = queueResources.Pop ();
					}

					// 取到了一个引用计数不为0的资源
					if (resource.Refs > 0) {
						resource.Start ();
						resource.Processor = i;
						processor [i].current = resource;
					}
				}
			}
		}
		public void OnResourceComplate(ResourceRef resource)
		{
			if (processor[resource.Processor].current==resource)
			{
				processor[resource.Processor].current = null;
			}
		}
	}
}

