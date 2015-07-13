using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AppInternalsDotNetSampler.Core.Discoverability;
using AppInternalsDotNetSampler.Core.Logging;

namespace AppInternalsDotNetSampler.Core
{
    public class SamplerMethodExecutor
    {
        private readonly IMethodLogger _logger;
        private readonly SamplerMethodDiscoverer _methodDiscoverer;

        public SamplerMethodExecutor(IMethodLogger logger)
        {
            _logger = logger;
            _methodDiscoverer = new SamplerMethodDiscoverer();
        }

        public void Execute(string methodName, List<SamplerMethodParameter> paramaters)
        {
            var method = _methodDiscoverer
                .GetAllMethods()
                .FirstOrDefault(m =>
                    string.Equals(
                        m.MethodName,
                        methodName,
                        StringComparison.InvariantCultureIgnoreCase));

            if (null == method)
                throw new Exception("Couldn't find a SamplerMethod with name [" + methodName + "]");

            foreach(var p in paramaters.Where(p => p.Value == null))
                throw new Exception("Parameter " + p.Name + " does not have a value.");

            try
            {
                method.WarmUp();
            }
            catch (Exception e)
            {
                _logger.WriteError(method.MethodName + " threw an Exception during WarmUp." +
                                   "Method will not be executed: " + 
                                    e.Message + Environment.NewLine + e.StackTrace);
            }

            var stopwatch = Stopwatch.StartNew();

            try
            {
                _logger.WriteMethodBegin(
                    method.MethodName +
                    Environment.NewLine +
                    Environment.NewLine + 
                    "Params: " +
                    string.Join(", ",
                        paramaters.Select(p =>
                            p.Name + " [" + p.Value + "]")) + 
                    Environment.NewLine);

                method.Execute(_logger, paramaters);

                _logger.WriteMethodEnd(
                    string.Format(
                    "End: {0} " + 
                    Environment.NewLine +      
                    "Completed in [{1:n0}] milliseconds.",
                    method.MethodName,
                    stopwatch.ElapsedMilliseconds));
            }
            catch (Exception e)
            {
                _logger.WriteError(method.MethodName + " threw an Exception during Execution." +
                                    e.Message + Environment.NewLine + e.StackTrace);
            }
        }
    }
}
