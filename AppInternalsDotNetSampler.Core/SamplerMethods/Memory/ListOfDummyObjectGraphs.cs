using System;
using System.Collections.Generic;
using System.Linq;
using AppInternalsDotNetSampler.Core.Discoverability;
using AppInternalsDotNetSampler.Core.Logging;

namespace AppInternalsDotNetSampler.Core.SamplerMethods.Memory
{
    public class ListOfDummyObjectGraphs : ISamplerMethod
    {
        public string MethodName
        {
            get { return "List of Dummy Object Graphs"; }
        }

        public SamplerMethodCategories Category
        {
            get{ return SamplerMethodCategories.Memory;}
        }
        public string Description { get { return ""; } }

        public List<SamplerMethodParameter> Parameters
        {
            get
            {
                return new List<SamplerMethodParameter>
                {
                    new LongParam("numberOfTopLevelDummyObjects")
                    {
                        DefaultValue = "100",
                        Description = "Number of Top Level Dummy Objects - " +
                                      "Note: More objects will be added because " +
                                      "the Dummy Objects randomly gives itself a " +
                                      "random number of children to form the graph."
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

        private void AllocateObjects(IMethodLogger logger, long numberOfTopLevelDummyObjects)
        {
            var startMemory = GC.GetTotalMemory(false);

            var list = new List<DummyObject>();

            for (long l = 0; l < numberOfTopLevelDummyObjects; l++)
            {
                list.Add(new DummyObject());
            }

            logger.WriteMethodInfo(
                string.Format(
                "Total Number of Objects [{0:n0}]. Max Depth of Object Graph [{1:n0}]",
                list.Sum(d => d.CalculateTotalNumberOfObjects()),
                list.Max(d => d.CalculateMaxDepth())));

            logger.WriteMethodInfo("");

            logger.WriteMethodInfo(
                string.Format(
                "Approx difference in Bytes of Managed Heap after allocation " +
                "[{0:n0}] bytes.",
                (GC.GetTotalMemory(false) - startMemory)));

            logger.WriteMethodInfo("");
        }
    }
}
