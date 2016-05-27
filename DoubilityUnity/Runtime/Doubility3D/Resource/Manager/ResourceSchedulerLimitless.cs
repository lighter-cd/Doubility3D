using System;
using System.Collections.Generic;

namespace Doubility3D.Resource.Manager
{
	public class ResourceSchedulerLimitless : IResourceScheduler
	{
		private Stack<ResourceRef> stackWaiter = new Stack<ResourceRef> ();
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
		public void ProcessDependWaiter()
		{
			if (stackWaiter.Count > 0) {
				ResourceRef top = stackWaiter.Peek ();
				if (top.IsDone) {
					stackWaiter.Pop ();
				}
			}
		}
		public void RegisterDependWaiter(ResourceRef resource)
		{
			stackWaiter.Push (resource);
		}
		public void OnResourceComplate(ResourceRef resource)
		{
		}
	}
}

