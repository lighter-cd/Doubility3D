using System;
using System.Collections.Generic;

namespace Doubility3D.Resource.Manager
{
	public class ResourceSchedulerLimitless : IResourceScheduler
	{
		public ResourceSchedulerLimitless ()
		{
		}
		public void ProcessQueue (PriorityQueue<ResourceRef> queueResources)
		{
			while (queueResources.Count > 0) {
				ResourceRef resource = queueResources.Top ();

				// 尝试取到一个引用计数不为0的资源
				while (resource.Refs == 0 && queueResources.Count > 0) {
					queueResources.Pop ();
					resource = queueResources.Top ();
				}

				// 取到了一个引用计数不为0的资源
				if (!resource.Async) {
					resource.Start ();
					queueResources.Pop ();
				} else {
					break;
				}
			}
		}
		public void OnResourceComplate(ResourceRef resource)
		{
		}
	}
}

