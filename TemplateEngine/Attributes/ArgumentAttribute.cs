using System;
using System.Collections.ObjectModel;


namespace TemplateEngine.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class ArgumentAttribute : Attribute
    {
        public ArgumentAttribute(Type[] supportedTypes, bool byKeyword = false, bool allowMultiple = false, string key = null)
        {
            AsPrevious = false;
            SupportedTypes = supportedTypes != null ? Array.AsReadOnly<Type>(supportedTypes) : null;
            ByKeyword = byKeyword;
            AllowMultiple = allowMultiple;
            Key = key;
        }

        public ArgumentAttribute(bool allowMultiple = false, string key = null)
        {
            AsPrevious = true;
            SupportedTypes = null;
            ByKeyword = false;
            AllowMultiple = allowMultiple;
            Key = key;
        }

        public bool AsPrevious { get; }
        public string Key { get; }
        public ReadOnlyCollection<Type> SupportedTypes { get; }
        public bool AllowMultiple { get; }
        public bool ByKeyword { get; }
    }

}
