using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AppInternalsDotNetSampler.Core.SamplerMethods.Memory
{
    [DebuggerDisplay("{IntField} - {Children.Count}")]
    public class DummyObject
    {
        private static readonly Random random = new Random();

        public DummyObject(int currentDepth = 0)
        {
            IntField = random.Next(int.MinValue + 1, int.MaxValue - 1);
            StringField = "Constant String Field";

            Children = new List<DummyObject>();

            if (currentDepth < 10 & random.Next(1,100) > 50)
            {
                //Create some children
                var numberOfChildrenToCreate = random.Next(0, 5);

                for (var i = 0; i < numberOfChildrenToCreate; i++)
                {
                    Children.Add(new DummyObject(currentDepth + 1));
                }
            }
        }

        public int IntField { get; set; }
        public string StringField { get; set; }
        public List<DummyObject> Children { get; set; }

        public int CalculateTotalNumberOfObjects()
        {
            if (!Children.Any())
                return 1;

            return
                1 +
                Children.Sum(c => c.CalculateTotalNumberOfObjects());
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
}
