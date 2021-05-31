using System;
using TemplateEngine.Attributes;

namespace TemplateEngine.Functions
{
    public static class ObjectFunctions
    {
        [Function("Eq")]
        public static bool Eq(object value, object other)
        {
            return (dynamic)value == (dynamic)other;
        }

        [Function("Ne")]
        public static bool Ne(object value, object other)
        {
            return (dynamic)value != (dynamic)other;
        }

        [Function("Lt")]
        [Argument(new Type[] {
            typeof(sbyte), typeof(short), typeof(int), typeof(long), typeof(byte), typeof(ushort), typeof(uint), typeof(ulong),
            typeof(float), typeof(double),
            typeof(DateTime),
            typeof(string)
        })]
        [Argument()]
        public static bool Lt(object value, object other)
        {
            return (dynamic)value < (dynamic)other;
        }

        [Function("Gt")]
        [Argument(new Type[] {
            typeof(sbyte), typeof(short), typeof(int), typeof(long), typeof(byte), typeof(ushort), typeof(uint), typeof(ulong),
            typeof(float), typeof(double),
            typeof(DateTime),
            typeof(string)
        })]
        [Argument()]
        public static bool Gt(object value, object other)
        {
            return (dynamic)value > (dynamic)other;
        }

        [Function("Le")]
        [Argument(new Type[] {
            typeof(sbyte), typeof(short), typeof(int), typeof(long), typeof(byte), typeof(ushort), typeof(uint), typeof(ulong),
            typeof(float), typeof(double),
            typeof(DateTime),
            typeof(string)
        })]
        [Argument()]
        public static bool Le(object value, object other)
        {
            return (dynamic)value <= (dynamic)other;
        }

        [Function("Ge")]
        [Argument(new Type[] {
            typeof(sbyte), typeof(short), typeof(int), typeof(long), typeof(byte), typeof(ushort), typeof(uint), typeof(ulong),
            typeof(float), typeof(double),
            typeof(DateTime),
            typeof(string)
        })]
        [Argument()]
        public static bool Ge(object value, object other)
        {
            return (dynamic)value >= (dynamic)other;
        }

        [Function("Between")]
        [Argument(new Type[] {
            typeof(sbyte), typeof(short), typeof(int), typeof(long), typeof(byte), typeof(ushort), typeof(uint), typeof(ulong),
            typeof(float), typeof(double),
            typeof(DateTime),
            typeof(string)
        })]
        [Argument()]
        [Argument()]
        public static bool Between(object value, object lower, object upper)
        {
            var input = (dynamic)value;
            return (dynamic)lower <= input && input <= (dynamic)upper;
        }

    }
}
