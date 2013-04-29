using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using K2Field.Helpers.Core.Code;
using System.CodeDom;
using System.Reflection;

namespace K2Field.Utilities.Testing.WF.Core
{
    public static class DynamicCodeExecution
    {

        public static Object InvokeMethod(string AssemblyName,string ClassName, string MethodName, Object[] args)
        {
            // load the assemly
            Assembly assembly = Assembly.LoadFrom(AssemblyName);

            try
            {

            // Walk through each type in the assembly looking for our class
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsClass == true)
                {
                    if (type.FullName.EndsWith("." + ClassName))
                    {
                        if (type.IsAbstract)
                        {
                            //Static method.
                            return type.InvokeMember(
                                            MethodName,
                                            BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static,
                                            null,
                                            null,
                                            args);

                        }
                        else
                        {
                            // create an instance of the object
                            object ClassObj = Activator.CreateInstance(type);
                            // Dynamically Invoke the method
                            object Result = type.InvokeMember(MethodName,
                              BindingFlags.Default | BindingFlags.InvokeMethod,
                                   null,
                                   ClassObj,
                                   args);
                            return (Result);

                        }


                    }
                }
            }
            }
            catch (MissingMethodException ex)
            {
                throw (new System.Exception(ex.Message + ", with " + args.Count().ToString() + " parameters, " + ex.InnerException.ToString()));
            }
            throw (new System.Exception("unhandled exception"));
        }
    }

}
