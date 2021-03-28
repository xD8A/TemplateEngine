using System;
using System.Linq;
using System.Reflection;
using HandlebarsDotNet;
using TemplateEngine.Attributes;
using TemplateEngine.Models;

namespace TemplateEngine.Filters
{
    static class NumberFilters
    {
        [Filter("add", "Returns the result of an addition")]
        [Input(typeof(sbyte), typeof(short), typeof(int), typeof(long),
    typeof(byte), typeof(ushort), typeof(uint), typeof(ulong),
    typeof(float), typeof(double))]
        [Parameter(null, "Other values", null, null, false, true)]
        public static object AddFilter(Context context, Arguments arguments)
        {
            Filter.GetCached(MethodBase.GetCurrentMethod()).Check(arguments);

            var tp = arguments.AsEnumerable().Any(v => v.GetType() == typeof(double) || v.GetType() == typeof(double))
                ? typeof(double) 
                : typeof(long);

            return arguments.AsEnumerable()
                .Aggregate(Convert.ChangeType(0, tp), (acc, v) => (dynamic)acc + (dynamic)Convert.ChangeType(v, tp));
        }

        [Filter("mul", "Returns the result of a multiplication")]
        [Input(typeof(sbyte), typeof(short), typeof(int), typeof(long),
            typeof(byte), typeof(ushort), typeof(uint), typeof(ulong),
            typeof(float), typeof(double))]
        [Parameter(null, "Other values", null, null, false, true)]
        public static object MulFilter(Context context, Arguments arguments)
        {
            Filter.GetCached(MethodBase.GetCurrentMethod()).Check(arguments);

            var tp = arguments.AsEnumerable().Any(v => v.GetType() == typeof(double) || v.GetType() == typeof(double))
                ? typeof(double)
                : typeof(long);

            return arguments.AsEnumerable()
                .Aggregate(Convert.ChangeType(1, tp), (acc, v) => (dynamic)acc * (dynamic)Convert.ChangeType(v, tp));
        }

        [Filter("sub", "Returns the result of a substraction")]
        [Input(typeof(sbyte), typeof(short), typeof(int), typeof(long),
            typeof(byte), typeof(ushort), typeof(uint), typeof(ulong),
            typeof(float), typeof(double))]
        [Parameter(null, "Other value")]
        public static object SubFilter(Context context, Arguments arguments)
        {
            Filter.GetCached(MethodBase.GetCurrentMethod()).Check(arguments);

            var input = (dynamic)arguments[0];
            var other = (dynamic)arguments[1];

            return input - other;
        }

        [Filter("div", "Returns the result of a division")]
        [Input(typeof(sbyte), typeof(short), typeof(int), typeof(long),
    typeof(byte), typeof(ushort), typeof(uint), typeof(ulong),
    typeof(float), typeof(double))]
        [Parameter(null, "Other value")]
        public static object DivFilter(Context context, Arguments arguments)
        {
            Filter.GetCached(MethodBase.GetCurrentMethod()).Check(arguments);

            var input = (dynamic)arguments[0];
            var other = (dynamic)arguments[1];

            return input - other;
        }


        [Filter("mod", "Returns the module of a divison")]
        [Input(typeof(sbyte), typeof(short), typeof(int), typeof(long),
            typeof(byte), typeof(ushort), typeof(uint), typeof(ulong))]
        [Parameter(null, "Other values", typeof(uint), null, false, true)]
        public static object ModFilter(Context context, Arguments arguments)
        {
            Filter.GetCached(MethodBase.GetCurrentMethod()).Check(arguments);

            var input = (dynamic)arguments[0];
            var other = Convert.ChangeType(arguments[1], typeof(uint));

            return input % other;
        }

    }
}
