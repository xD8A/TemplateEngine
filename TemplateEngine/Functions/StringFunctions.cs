using TemplateEngine.Attributes;

namespace TemplateEngine.Functions
{
    public static class StringFunctions
    {

        [Function("ToUpper")]
        public static string ToUpper(string value)
        {
            return value.ToUpper();
        }

        [Function("Split")]
        public static string[] Split(string value, string separator = null)
        {
            if (separator is null)
                return value.Split();
            return value.Split(separator);
        }

    }
}
