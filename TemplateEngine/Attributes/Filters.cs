using System;
using System.Collections.ObjectModel;

namespace TemplateEngine.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    class FilterAttribute : Attribute
    {
        public FilterAttribute(string name, string description, bool options = false)
        {
            Name = name;
            Description = description;
            Options = options;
        }
        public string Name { get; }
        public string Description { get; }
        public bool Options { get; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    class InputAttribute : Attribute
    {
        public InputAttribute(params Type[] types)
        {
            Types = new ReadOnlyCollection<Type>(types);
        }

        public ReadOnlyCollection<Type> Types { get; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    class OutputAttribute : Attribute
    {
        public OutputAttribute(Type type = null)
        {
            Type = type;
        }
        public Type Type { get; }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    class ParameterAttribute : Attribute
    {
        public ParameterAttribute(string name, string description, Type type = null, object defaultValue = null, bool optional = false, bool multiple = false)
        {
            Name = name;
            Description = description;
            Type = type;
            Default = defaultValue;
            Optional = optional;
            Multiple = multiple;
        }

        public string Name { get; }
        public string Description { get; }
        public Type Type { get; }
        public object Default { get; }
        public bool Optional { get; }

        public bool Multiple { get; }
    }

}
