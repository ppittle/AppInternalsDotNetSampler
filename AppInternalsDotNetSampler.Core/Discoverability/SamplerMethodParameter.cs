using System;
using System.Collections.Generic;
using System.Linq;

namespace AppInternalsDotNetSampler.Core.Discoverability
{
    public class SamplerMethodParameter
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Type Type { get; set; }
        public string DefaultValue { get; set; }
        public Func<string, object> ParseFunc { get; set; }
        public object Value { get; set; }
    }

    public class StringParam : SamplerMethodParameter
    {
        public StringParam(string name)
        {
            Name = name;
            Type = typeof (string);
            ParseFunc = s => s;
        }
    }

    public class IntParam : SamplerMethodParameter
    {
        public IntParam(string name)
        {
            Name = name;
            Type = typeof (int);
            ParseFunc = s => int.Parse(s);
        }
    }

    public class LongParam : SamplerMethodParameter
    {
        public LongParam(string name)
        {
            Name = name;
            Type = typeof (long);
            ParseFunc = s => long.Parse(s);
        }
    }

    public static class SamplerMethodParameterExtensions
    {
        public static T GetValue<T>(
            this IEnumerable<SamplerMethodParameter> @params, int index)
        {
            var obj =
                @params.ElementAt(index).Value;
            
            if (!typeof (T).IsValueType || typeof(T) == typeof(string))
                return (T)Convert.ChangeType(obj, typeof(T));

            return (T)obj;
        }
    }
}