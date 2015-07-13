using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppInternalsDotNetSampler.Core.Discoverability;
using AppInternalsDotNetSampler.Core.Logging;

namespace AppInternalsDotNetSampler.Core.SamplerMethods.CPU
{
    public class SumArrayMultiThreaded : ISamplerMethod
    {
        public string MethodName
        {
            get { return "Sum Array Multi Threaded"; }
        }

        public SamplerMethodCategories Category
        {
            get { return SamplerMethodCategories.CPU; }
        }

        public string Description
        {
            get
            {
                return "Simulates heavy CPU Load by adding all of 5000 numbers in an array.  " +
                       "Addition is done on a single thread.";
            }
        }

        public List<SamplerMethodParameter> Parameters
        {
            get
            {
                return new List<SamplerMethodParameter>
                {
                    new LongParam("numberOfTimesToCount")
                    {
                        DefaultValue = "100000",
                        Description = "Number of times to add the sum of all numbers in the array"
                    },
                    new IntParam("numberOfThreadsToUse")
                    {
                        DefaultValue = "4"
                    }
                };
            }
        }

        private int[] _array;
        public void WarmUp()
        {
            _array = new int[5000];

            var random = new Random(DateTime.Now.Second);

            //warm up random
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            random.Next();

            for (var i = 0; i < _array.Length; i++)
            {
                _array[i] = random.Next(1, 1000);
            }
        }

        public void Execute(IMethodLogger logger, List<SamplerMethodParameter> parameters)
        {
            SumNumbersInArray(
                logger, 
                parameters.GetValue<long>(0),
                parameters.GetValue<int>(1));
        }

        private void SumNumbersInArray(IMethodLogger logger, long numberOfTimesToCount, int numberOfThreadsToUse)
        {
            Parallel.For(0, numberOfTimesToCount,
                new ParallelOptions
                {
                    MaxDegreeOfParallelism = numberOfThreadsToUse
                },
                l =>
                {
                    var sum = _array.Sum();

                    if (sum < 0)
                        throw new Exception(
                            "This exception is here to ensure the compiler did not 'optimize' away the call to _array.Sum().  It should never be hit.");
                });

            logger.WriteMethodInfo(
                string.Format("Sum is [{0:n0}]", _array.Sum()));
        }
    }
}