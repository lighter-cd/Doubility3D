using System;

namespace Doubility3D.Resource.Manager
{
	public interface IResourceScheduler
	{
		void ProcessQueue (PriorityQueue<ResourceRef> queueResources);
		void OnResourceComplate(ResourceRef resource);
	}
}

