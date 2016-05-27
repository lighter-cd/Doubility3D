using System;

namespace Doubility3D.Resource.Manager
{
	public interface IResourceScheduler
	{
		void ProcessQueue (PriorityQueue<ResourceRef> queueResources);
		void ProcessDependWaiter();
		void RegisterDependWaiter(ResourceRef resource);
		void OnResourceComplate(ResourceRef resource);
	}
}

