using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using Castle.MicroKernel;

namespace GlobalWeatherApi
{
    public class DependencyResolver : System.Web.Http.Dependencies.IDependencyResolver
    {
        private readonly IKernel _container;

        public DependencyResolver(IKernel container)
        {
            _container = container;
        }

        public IDependencyScope BeginScope()
        {
            return new DependencyScope(_container);
        }

        public object GetService(Type serviceType)
        {
            return _container.HasComponent(serviceType) ? _container.Resolve(serviceType) : null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _container.ResolveAll(serviceType).Cast<object>();
        }

        public void Dispose()
        {
        }
    }
}