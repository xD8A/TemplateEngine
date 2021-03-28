using System;
using System.Linq;
using System.Reflection;
using HandlebarsDotNet;
using TemplateEngine.Attributes;
using TemplateEngine.Models;


namespace TemplateEngine.Filters
{

    static class CommonFilters
    {
        [Filter("eq", "Checks whether an input value is equal to another one")]
        [Input(typeof(sbyte), typeof(short), typeof(int), typeof(long),
            typeof(byte), typeof(ushort), typeof(uint), typeof(ulong),
            typeof(float), typeof(double),
            typeof(string))]
        [Parameter(null, "Other value")]
        [Output(typeof(bool))]
        public static object EqualFilter(Context context, Arguments arguments)
        {
            Filter.GetCached(MethodBase.GetCurrentMethod()).Check(arguments);
            var input = arguments[0];
            var other = arguments[1];
            return (dynamic)input == (dynamic)other;
        }

        [Filter("ne", "Checks whether an input value is not equal to another one")]
        [Input(typeof(sbyte), typeof(short), typeof(int), typeof(long),
            typeof(byte), typeof(ushort), typeof(uint), typeof(ulong),
            typeof(float), typeof(double),
            typeof(string))]
        [Parameter(null, "Other value")]
        [Output(typeof(bool))]
        public static object NotEqualFilter(Context context, Arguments arguments)
        {
            Filter.GetCached(MethodBase.GetCurrentMethod()).Check(arguments);
            var input = arguments[0];
            var other = arguments[1];
            return (dynamic)input != (dynamic)other;
        }

        [Filter("lt", "Checks whether an input value is less than another one")]
        [Input(typeof(sbyte), typeof(short), typeof(int), typeof(long),
            typeof(byte), typeof(ushort), typeof(uint), typeof(ulong),
            typeof(float), typeof(double),
            typeof(string))]
        [Parameter(null, "Other value")]
        [Output(typeof(bool))]
        public static object LessThanFilter(Context context, Arguments arguments)
        {
            Filter.GetCached(MethodBase.GetCurrentMethod()).Check(arguments);
            var input = arguments[0];
            var other = arguments[1];
            return (dynamic)input < (dynamic)other;
        }

        [Filter("gt", "Checks whether an input value is greater than another one")]
        [Input(typeof(sbyte), typeof(short), typeof(int), typeof(long),
            typeof(byte), typeof(ushort), typeof(uint), typeof(ulong),
            typeof(float), typeof(double),
            typeof(string))]
        [Parameter(null, "Other value")]
        [Output(typeof(bool))]
        public static object GreaterThanFilter(Context context, Arguments arguments)
        {
            Filter.GetCached(MethodBase.GetCurrentMethod()).Check(arguments);
            var input = arguments[0];
            var other = arguments[1];
            return (dynamic)input > (dynamic)other;
        }

        [Filter("le", "Checks whether an input value is less or equal to another one")]
        [Input(typeof(sbyte), typeof(short), typeof(int), typeof(long),
            typeof(byte), typeof(ushort), typeof(uint), typeof(ulong),
            typeof(float), typeof(double),
            typeof(string))]
        [Parameter(null, "Other value")]
        [Output(typeof(bool))]
        public static object LessOrEqualFilter(Context context, Arguments arguments)
        {
            Filter.GetCached(MethodBase.GetCurrentMethod()).Check(arguments);
            var input = arguments[0];
            var other = arguments[1];
            return (dynamic)input <= (dynamic)other;
        }

        [Filter("ge", "Checks whether an input value is greater or equal to another one")]
        [Input(typeof(sbyte), typeof(short), typeof(int), typeof(long),
            typeof(byte), typeof(ushort), typeof(uint), typeof(ulong),
            typeof(float), typeof(double),
            typeof(string))]
        [Parameter(null, "Other value")]
        [Output(typeof(bool))]
        public static object GreaterOrEqualFilter(Context context, Arguments arguments)
        {
            Filter.GetCached(MethodBase.GetCurrentMethod()).Check(arguments);
            var input = arguments[0];
            var other = arguments[1];
            return (dynamic)input >= (dynamic)other;
        }

        [Filter("between", "Checks whether an input value is between the lower and upper bounds (including them)")]
        [Input(typeof(sbyte), typeof(short), typeof(int), typeof(long),
            typeof(byte), typeof(ushort), typeof(uint), typeof(ulong),
            typeof(float), typeof(double),
            typeof(string))]
        [Parameter(null, "Lower bound")]
        [Parameter(null, "Upper bound")]
        [Output(typeof(bool))]
        public static object BetweenFilter(Context context, Arguments arguments)
        {
            Filter.GetCached(MethodBase.GetCurrentMethod()).Check(arguments);
            var input = (dynamic)arguments[0];
            var lower = arguments[1];
            var upper = arguments[2];
            return (dynamic)lower <= input && input <= (dynamic)upper;
        }

        [Filter("not", "Inverts the input value")]
        [Input(typeof(bool))]
        public static object NotFilter(Context context, Arguments arguments)
        {
            Filter.GetCached(MethodBase.GetCurrentMethod()).Check(arguments);
            var input = arguments[0];
            return !(bool)Convert.ChangeType(input, typeof(bool));
        }

        [Filter("and", "Returns the result of a logical multiplication")]
        [Input(typeof(bool))]
        [Parameter(null, "Other values", null, null, false, true)]
        public static object AndFilter(Context context, Arguments arguments)
        {
            Filter.GetCached(MethodBase.GetCurrentMethod()).Check(arguments);
            return arguments.AsEnumerable().Select(v => Convert.ChangeType(v, typeof(bool))).All(v => v is true);
        }

        [Filter("or", "Returns the result of a logical addition")]
        [Input(typeof(bool))]
        [Parameter(null, "Other values", null, null, false, true)]
        public static object OrFilter(Context context, Arguments arguments)
        {
            Filter.GetCached(MethodBase.GetCurrentMethod()).Check(arguments);
            return arguments.AsEnumerable().Select(v => Convert.ChangeType(v, typeof(bool))).Any(v => v is true);
        }
    }
}
