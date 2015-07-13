using System;
using System.Collections.Generic;
using System.Linq;
using AppInternalsDotNetSampler.Core;
using AppInternalsDotNetSampler.Core.Discoverability;

namespace AppInternalsDotNetSampler.Console
{
    class Program
    {
        private static List<ISamplerMethod> _methods; 

        static void Main(string[] args)
        {
            #region Write Preamble
            System.Console.WriteLine("");
            System.Console.WriteLine("********************************************************");
            System.Console.WriteLine("*     WELCOME TO THE APP INTERNALS DOT NOT SAMPLER     *");
            System.Console.WriteLine("*              CONSOLE APPLICATION                     *");
            System.Console.WriteLine("*                                                      *");
            System.Console.WriteLine("* Source Code Available At:                            *");
            System.Console.WriteLine("* https://github.com/ppittle/AppInternalsDotNetSampler *");
            System.Console.WriteLine("*                                                      *");
            System.Console.WriteLine("* -- Philip Pittle philip.pittle@gmail.com             *");
            System.Console.WriteLine("********************************************************");

            System.Console.WriteLine();
            System.Console.WriteLine();
            #endregion

            _methods = new SamplerMethodDiscoverer().GetAllMethods();

            System.Console.WriteLine("Found [{0}] Sampler Methods",
                _methods.Count);

            System.Console.WriteLine();

            while (true)
            {
                NavigateToCategories();
            }
        }

        private static void NavigateToCategories()
        {
            System.Console.WriteLine();

            var cats = Enum.GetNames(typeof (SamplerMethodCategories));

            System.Console.WriteLine("Please choose a category:");
            System.Console.WriteLine();

            for (int i = 0; i < cats.Length; i++)
            {
                System.Console.WriteLine(i + " - " + cats[i]);
            }

            System.Console.Write("Choice: ");
            var choiceString = System.Console.ReadLine();

            int choice;
            if (!int.TryParse(choiceString, out choice) ||
                choice < 0 ||
                choice >= cats.Length )
            {
                System.Console.WriteLine("Invalid Entry");
            }
            else
            {
                NavigateToCategory(
                    (SamplerMethodCategories)
                    Enum.Parse(typeof(SamplerMethodCategories),
                    cats[choice]));   
            }
        }

        private static void NavigateToCategory(SamplerMethodCategories cat)
        {
            var catMethods = _methods.Where(m => m.Category == cat).ToArray();

            if (!catMethods.Any())
            {
                System.Console.WriteLine(
                    "No Methods available for " +
                    Enum.GetName(typeof (SamplerMethodCategories), cat));

                return;
            }

            System.Console.WriteLine("Please choose a method:");
            System.Console.WriteLine();

            for (var i = 0; i < catMethods.Length; i++)
            {
                System.Console.WriteLine(i + " - " + catMethods[i].MethodName);
                System.Console.WriteLine("\t" + 
                    string.Join(Environment.NewLine + "\t",
                    catMethods[i].Description.SplitIntoChunks(60)));
            }
            System.Console.WriteLine("Enter - Return to Categories");

            System.Console.Write("Choice: ");
            var choiceString = System.Console.ReadLine();

            if (string.IsNullOrEmpty(choiceString))
                return;

            int choice;
            if (!int.TryParse(choiceString, out choice) ||
                choice < 0 ||
                choice >= catMethods.Length)
            {
                System.Console.WriteLine("Invalid Entry");
            }
            else
            {
                NavigateToMethod(catMethods[choice]);
            }
        }

        private static void NavigateToMethod(ISamplerMethod method)
        {
            var @params = method.Parameters;

            #region Get Paramters from User
            if (@params.Any())
            {
                System.Console.WriteLine("Enter Required Paramaters:");

                foreach (var p in @params)
                {
                    while (true)
                    {
                        System.Console.WriteLine();

                        System.Console.Write(p.Name);

                        System.Console.Write(" (" + p.Type.Name + ")");
                        if (!string.IsNullOrEmpty(p.DefaultValue))
                            System.Console.Write(" [" + p.DefaultValue + "]");

                        if (!string.IsNullOrEmpty(p.Description))
                        {
                            System.Console.WriteLine();
                            System.Console.WriteLine("\t" +
                                string.Join("\r\n\t",
                                    p.Description.SplitIntoChunks(60)));
                        }


                        System.Console.Write(":");

                        var choice = System.Console.ReadLine();

                        if (string.IsNullOrEmpty(choice) && !string.IsNullOrEmpty(p.DefaultValue))
                        {
                            try
                            {
                                p.Value = p.ParseFunc(p.DefaultValue);
                                break;
                            }
                            catch
                            {
                                System.Console.WriteLine(
                                    "THIS IS A BUG: The Default Value[{0}] is not valid.  " +
                                    "You're going to have to enter your own value.",
                                    p.DefaultValue);
                            }
                        }
                        else if (!string.IsNullOrEmpty(choice))
                        {
                            try
                            {
                                p.Value = p.ParseFunc(choice);
                                break;
                            }
                            catch
                            {
                                System.Console.WriteLine(choice + "is not a valid " + p.Type.Name);
                            }
                        }
                        else
                        {
                            System.Console.WriteLine("You must enter a value.");
                        }
                    }
                }
            }
            #endregion

            System.Console.WriteLine();

            //Confirm Method Execution
            System.Console.Write("Confirm Execute Method " + method.MethodName + " {[y]|n}: ");
            var executeChoice = System.Console.ReadLine();

            if (!string.IsNullOrEmpty(executeChoice) ||
                string.Equals(executeChoice, "n", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }
            
            ExecuteMethod(method.MethodName, @params);
        }

        private static void ExecuteMethod(string methodName, List<SamplerMethodParameter> @params)
        {
            System.Console.WriteLine();
            new SamplerMethodExecutor(new ConsoleMethodLogger())
                .Execute(methodName, @params);
            System.Console.WriteLine();
        }
    }
}
