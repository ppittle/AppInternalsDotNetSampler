using System;
using System.Collections.Generic;
using AppInternalsDotNetSampler.Core.Discoverability;
using AppInternalsDotNetSampler.Core.Logging;

namespace AppInternalsDotNetSampler.Core.SamplerMethods.Memory
{
    public class RunGarbageCollector : ISamplerMethod
    {
        public string MethodName
        {
            get
            {
                return "Run Garbage Collector";
            }
        }

        public SamplerMethodCategories Category
        {
            get
            {
                return SamplerMethodCategories.Memory;
                
            }
        }

        public string Description
        {
            get
            {
                return "Measures the size of the Managed Heap, " +
                       "invokes the .NET Garbage Collector (blocking " +
                       "on all GC generations)  and then measures" +
                       "the Managed Heap Size again.";
            }
        }

        public List<SamplerMethodParameter> Parameters
        {
            get{return new List<SamplerMethodParameter>();}
        }

        public void WarmUp()
        {
            //nothing to do here
        }

        public void Execute(IMethodLogger logger, List<SamplerMethodParameter> parameters)
        {
            var startMemory = GC.GetTotalMemory(false);

            GC.Collect(0, GCCollectionMode.Forced,true);
            GC.Collect(1, GCCollectionMode.Forced, true);
            GC.Collect(2, GCCollectionMode.Forced, true);

            logger.WriteMethodInfo("");

            logger.WriteMethodInfo(
                string.Format(
                "Approx difference in Bytes of Managed Heap after GC (should be negative) " +
                "[{0:n0}] bytes.",
                (GC.GetTotalMemory(false) - startMemory)));

            logger.WriteMethodInfo("");
        }
    }
}
