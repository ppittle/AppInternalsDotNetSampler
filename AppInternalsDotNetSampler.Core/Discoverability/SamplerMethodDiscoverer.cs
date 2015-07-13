using System;
using System.Collections.Generic;
using System.Linq;


namespace AppInternalsDotNetSampler.Core.Discoverability
{
    public class SamplerMethodDiscoverer
    {
        public List<ISamplerMethod> GetAllMethods()
        {
            return this.GetType().Assembly.GetTypes()
                .Where(t => 
                    !t.IsAbstract && !t.IsInterface &&
                    (typeof (ISamplerMethod).IsAssignableFrom(t)))
                .Select(t => (ISamplerMethod)Activator.CreateInstance(t))
                .ToList();
        }
    }
}
