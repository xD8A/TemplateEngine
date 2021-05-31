using System;
using System.Linq;
using TemplateEngine.Attributes;


namespace TemplateEngine.Functions
{
    public static class BooleanFunctions
    {
        [Function("Not")]
        public static bool Not(bool value)
        {
            return !value;
        }

        [Function("And")]
        public static bool And(bool value, params bool[] other)
        {
            return value && other.All(v => v);
        }

        [Function("Or")]
        public static object Or(bool value, params bool[] other)
        {
            return value || other.Any(v => v);
        }
    }
}
