using System;
using System.Globalization;
using System.Linq;
using TemplateEngine.Attributes;


namespace TemplateEngine.Functions
{
    public static class NumberFunctions
    {
        [Function("Add", 0)]
        [Argument(new Type[] {
            typeof(sbyte), typeof(short), typeof(int), typeof(long), typeof(byte), typeof(ushort), typeof(uint), typeof(ulong),
            typeof(float), typeof(double) 
        })]
        [Argument()]
        public static object Add(object value, params object[] addendums)
        {
            var values = addendums.AsEnumerable().Select(
                v => v.GetType() == typeof(string)
                ? double.Parse(v.ToString(), CultureInfo.InvariantCulture)
                : v
            ).ToArray();

            var tp = value.GetType();
            tp = (tp == typeof(double) || tp == typeof(float)) ? typeof(double) : typeof(long);

            var addendum = values.AsEnumerable().Aggregate(Convert.ChangeType(0, tp), (acc, v) => (dynamic)acc + (dynamic)Convert.ChangeType(v, tp));
            return (dynamic)value + (dynamic)addendum;
        }

        [Function("Mul", 0)]
        [Argument(new Type[] {
            typeof(sbyte), typeof(short), typeof(int), typeof(long), typeof(byte), typeof(ushort), typeof(uint), typeof(ulong),
            typeof(float), typeof(double)
        })]
        [Argument()]
        public static object Mul(object value, params object[] multipliers)
        {
            var values = multipliers.AsEnumerable().Select(
                v => v.GetType() == typeof(string)
                ? double.Parse(v.ToString(), CultureInfo.InvariantCulture)
                : v
            ).ToArray();

            var tp = value.GetType();
            tp = (tp == typeof(double) || tp == typeof(float)) ? typeof(double) : typeof(long);

            var mult = values.AsEnumerable().Aggregate(Convert.ChangeType(1, tp), (acc, v) => (dynamic)acc * (dynamic)Convert.ChangeType(v, tp));
            return (dynamic)value * (dynamic)mult;
        }

        [Function("Inc", 0)]
        [Argument(new Type[] {
            typeof(sbyte), typeof(short), typeof(int), typeof(long), typeof(byte), typeof(ushort), typeof(uint), typeof(ulong),
            typeof(float), typeof(double)
        })]
        [Argument()]
        public static object Inc(object value, int increment = 1)
        {
            return (dynamic)value + increment;
        }

        [Function("Dec", 0)]
        [Argument(new Type[] {
            typeof(sbyte), typeof(short), typeof(int), typeof(long), typeof(byte), typeof(ushort), typeof(uint), typeof(ulong),
            typeof(float), typeof(double)
        })]
        [Argument()]
        public static object Dec(object value, int decrement = 1)
        {
            return (dynamic)value - decrement;
        }

        [Function("Sub", 0)]
        [Argument(new Type[] {
            typeof(sbyte), typeof(short), typeof(int), typeof(long), typeof(byte), typeof(ushort), typeof(uint), typeof(ulong),
            typeof(float), typeof(double)
        })]
        [Argument()]
        public static object Sub(object value, object subtrahend)
        {
            return (dynamic)value - (dynamic)subtrahend;
        }

        [Function("Div", 0)]
        [Argument(new Type[] {
            typeof(sbyte), typeof(short), typeof(int), typeof(long), typeof(byte), typeof(ushort), typeof(uint), typeof(ulong),
            typeof(float), typeof(double)
        })]
        [Argument()]
        public static object Div(object value, object divisor)
        {
            return (dynamic)value / (dynamic)divisor;
        }


        [Function("Mod", 0)]
        [Argument(new Type[] {
            typeof(byte), typeof(ushort), typeof(uint), typeof(ulong)
        })]
        [Argument()]
        public static object Mod(object value, object divisor)
        {

            return (dynamic)value % (dynamic)divisor;
        }


    }
}
