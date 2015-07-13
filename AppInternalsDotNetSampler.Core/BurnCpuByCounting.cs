using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AppInternalsDotNetSampler.Core.Logging;

namespace AppInternalsDotNetSampler.Core
{
    /// <summary>
    /// Simulates heavy CPU load by performing arbitrary calcualtions on an array
    /// </summary>
    public class BurnCpuByCounting
    {
        private readonly int[] _array;

        private readonly IMethodLogger _logger;

        public BurnCpuByCounting(IMethodLogger logger)
        {
            _logger = logger;

            _array = new int[500];

            var random = new Random(DateTime.Now.Second);

            //warm up random
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            random.Next();

            for (var i = 0; i < _array.Length; i++)
            {
                _array[i] = random.Next(1, 1000);
            }
        }

        public void SumNumbersInArray(long numberOfTimesToCount)
        {
            var stopwatch = Stopwatch.StartNew();

            _logger.WriteMethodBegin("Begin SumNumbersInArray.  Counting [" + numberOfTimesToCount + "] times.");

            for (long l = 0; l < numberOfTimesToCount; l++)
            {
                var sum = _array.Sum();

                if (sum < 0)
                    throw new Exception(
                        "This exception is here to ensure the compiler did not 'optimize' away the call to _array.Sum().  It should never be hit.");
            }

            _logger.WriteMethodEnd("End SumNumbersInArray.  Completed in [" + stopwatch.ElapsedMilliseconds + " ] milliseconds.");
        }

        public void SumNumbersInArrayMultiThreaded(long numberOfTimesToCount, int numberOfThreadsToUse)
        {
            var stopwatch = Stopwatch.StartNew();

            _logger.WriteMethodBegin("Begin SumNumbersInArrayMultiThreaded.  Counting [" + numberOfTimesToCount + "] times spread out over [" + numberOfThreadsToUse + "] threads.");

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

            _logger.WriteMethodEnd("End SumNumbersInArrayMultiThreaded.  Completed in [" + stopwatch.ElapsedMilliseconds + " ] milliseconds.");
        }
    }
}
