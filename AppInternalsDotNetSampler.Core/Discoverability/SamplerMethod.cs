using System.Collections.Generic;
using AppInternalsDotNetSampler.Core.Logging;

namespace AppInternalsDotNetSampler.Core.Discoverability
{
    public interface ISamplerMethod
    {
        string MethodName { get;  }
        SamplerMethodCategories Category { get; }
        string Description { get;  }
        List<SamplerMethodParameter> Parameters { get; }
        void WarmUp();
        void Execute(IMethodLogger logger, List<SamplerMethodParameter> parameters);
    }
}
