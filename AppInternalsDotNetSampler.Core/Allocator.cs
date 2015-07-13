using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AppInternalsDotNetSampler.Core.Logging;

namespace AppInternalsDotNetSampler.Core
{
    /// <summary>
    /// Allocates dummy primitives and objects to the stack / heap.
    /// </summary>
    public class Allocator
    {
        private readonly IMethodLogger _logger;

        #region Dummy Object
        public class DummyObject
        {
            public DummyObject()
            {
                var random = new Random(DateTime.Now.Second);

                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                random.Next();

                IntField = random.Next(int.MinValue + 1, int.MaxValue - 1);
                StringField = "Constant String Field";

                Children = new List<DummyObject>();

                if (random.Next(1, 100) > 60)
                {
                    //Create some children
                    var numberOfChildrenToCreate = random.Next(0, 5);

                    for (var i = 0; i < numberOfChildrenToCreate; i++)
                    {
                        Children.Add(new DummyObject());
                    }
                }
            }

            public int IntField { get; set; }
            public string StringField { get; set; }
            public List<DummyObject> Children { get; set; }

            public int CalculateTotalNumberOfChildren()
            {
                if (!Children.Any())
                    return 0;

                return 
                    Children.Count + 
                    Children.Sum(c => c.CalculateTotalNumberOfChildren());
            }

            public int CalculateMaxDepth()
            {
                if (!Children.Any())
                    return 0;

                return
                    1 +
                    Children.Max(c => c.CalculateMaxDepth());
            }
        }
        #endregion


        public Allocator(IMethodLogger logger)
        {
            _logger = logger;
        }

        public void AllocateAndInitializeArrayOfFloats(long numberOfFloats)
        {
            var stopwatch = Stopwatch.StartNew();

            _logger.WriteMethodBegin("Begin AllocateAndInitializeArrayOfFloats.  Initializing [" + numberOfFloats + "] floats.");

            var array = new float[numberOfFloats];

            for (long l = 0; l < numberOfFloats; l++)
            {
                array[l] = 1;
            }

            _logger.WriteMethodEnd("End AllocateAndInitializeArrayOfFloats.  Completed in [" + stopwatch.ElapsedMilliseconds + " ] milliseconds.");
        }

        public void AllocateAndInitializeArrayOfDummyObjectGraphs(long numberOfTopLevelDummyObjects)
        {
            var stopwatch = Stopwatch.StartNew();

            _logger.WriteMethodBegin("Begin AllocateAndInitializeArrayOfDummyObjects.  " +
                                     "Initializing [" + numberOfTopLevelDummyObjects + "] floats.");

            var array = new DummyObject[numberOfTopLevelDummyObjects];

            for (long l = 0; l < numberOfTopLevelDummyObjects; l++)
            {
                array[l] = new DummyObject();
            }

            _logger.WriteMethodInfo(
                string.Format(
                "Total Number of Objects [{0}]. Max Depth of Object Graph [{1}]",
                array.Sum(d => d.CalculateTotalNumberOfChildren()),
                array.Max(d => d.CalculateMaxDepth())));

            _logger.WriteMethodEnd("End AllocateAndInitializeArrayOfDummyObjects.  " +
                                   "Completed in [" + stopwatch.ElapsedMilliseconds + " ] milliseconds.");
        }

        public void AllocateAndInitializeListOfDummyObjectGraphs(long numberOfTopLevelDummyObjects)
        {
            var stopwatch = Stopwatch.StartNew();

            _logger.WriteMethodBegin("Begin AllocateAndInitializeListOfDummyObjectGraphs.  " +
                                     "Initializing [" + numberOfTopLevelDummyObjects + "] floats.");

            var list = new List<DummyObject>();
            
            for (long l = 0; l < numberOfTopLevelDummyObjects; l++)
            {
                list.Add(new DummyObject());
            }

            _logger.WriteMethodInfo(
                string.Format(
                "Total Number of Objects [{0}]. Max Depth of Object Graph [{1}]",
                list.Sum(d => d.CalculateTotalNumberOfChildren()),
                list.Max(d => d.CalculateMaxDepth())));

            _logger.WriteMethodEnd("End AllocateAndInitializeListOfDummyObjectGraphs.  " +
                                   "Completed in [" + stopwatch.ElapsedMilliseconds + " ] milliseconds.");
        }
    }
}
