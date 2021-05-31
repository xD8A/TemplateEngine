using System;
using System.Diagnostics;


namespace TemplateEngine.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class FunctionAttribute : Attribute
    {
        public FunctionAttribute(string key, int resultTypeByInput = -1)
        {
            Key = key;
            ResultTypeByInput = resultTypeByInput;
        }

        public string Key { get; }
        public int ResultTypeByInput { get; }
    }
}
