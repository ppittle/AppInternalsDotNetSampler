using System;
using System.Collections.Generic;
using AppInternalsDotNetSampler.Core.Discoverability;
using AppInternalsDotNetSampler.Core.Logging;

namespace AppInternalsDotNetSampler.Core.SamplerMethods.Memory
{
    class ArrayOfFloats : ISamplerMethod
    {
        public string MethodName
        {
            get { return "Array of Floats"; }
        }

        public SamplerMethodCategories Category
        {
            get { return SamplerMethodCategories.Memory; }
        }
        public string Description { get { return ""; } }

        public List<SamplerMethodParameter> Parameters
        {
            get
            {
                return new List<SamplerMethodParameter>
                {
                    new LongParam("numberOfFloats")
                    {
                        DefaultValue = "1000",
                        Description = "Size of the array to create"
                    }
                };
            }
        }

        public void WarmUp()
        {
            //nothing to do here
        }

        public void Execute(IMethodLogger logger, List<SamplerMethodParameter> parameters)
        {
            AllocateObjects(logger, parameters.GetValue<long>(0));
        }

        private void AllocateObjects(IMethodLogger logger, long numberOfFloats)
        {
            var startMemory = GC.GetTotalMemory(false);

            var array = new float[numberOfFloats];

            for (long l = 0; l < numberOfFloats; l++)
            {
                array[l] = 1;
            }

            logger.WriteMethodInfo("");

            logger.WriteMethodInfo(
                string.Format(
                "Approx difference in Bytes of Managed Heap after allocation. (0 is expected)" +
                "[{0:n0}] bytes.",
                (GC.GetTotalMemory(false) - startMemory)));

            logger.WriteMethodInfo("");
        }
    }
}
