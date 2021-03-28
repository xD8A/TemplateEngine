using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using HandlebarsDotNet;
using TemplateEngine.Attributes;


namespace TemplateEngine.Models
{
    public class FilterParameter
    {
        public FilterParameter(string name, string description, Type type, object defaultValue, bool optional = false, bool multiple = false)
        {
            Name = name;
            Description = description;
            Type = type;
            Default = defaultValue;
            Optional = optional;
            Multiple = multiple;
        }

        public override string ToString()
        {
            return (Type.ToString() + (Optional ? "?" : "") + " " + Name
                + (Default is null ? "" : " = " + Default.ToString()));
        }

        public string Name { get; }
        public string Description { get; }
        public Type Type { get; }

        public Type GetType(Type inputType)
        {
            if (Type is null)
                return inputType;
            return Type;
        }

        public object Default { get; }
        public bool Optional { get; }
        public bool Multiple { get; }
    }
    public class Filter
    {
        private const int argumentLimit = 255;
        private static Dictionary<MethodBase, Filter> cache = new Dictionary<MethodBase, Filter>();

        public static Filter GetCached(MethodBase method)
        {
            if (!cache.TryGetValue(method, out var filter))
            {
                filter = new Filter(method);
                cache[method] = filter;
            }
            return filter;
        }
        public Filter(Filter source, Type selectedType)
        {
            Info = source.Info;
            Helper = source.Helper;
            OptionsHelper = source.OptionsHelper;
            Name = source.Name;
            Description = source.Description;
            if (!source.InputTypes.Contains(selectedType))
                throw new ArgumentException("invalid type");
            InputTypes = new ReadOnlyCollection<Type>(new Type[] { selectedType });
            OutputType = source.OutputType is null ? selectedType : source.OutputType;
            Parameters = source.Parameters
                .Select(p => new FilterParameter(p.Name, p.Description, p.Type is null ? selectedType : p.Type, p.Default, p.Optional, p.Multiple))
                .ToList()
                .AsReadOnly();
            PositionalParameterCount = source.PositionalParameterCount;
            RequiredParameterCount = source.RequiredParameterCount;
            Keywords = source.Keywords;
        }

        public Filter(MethodBase method)
        {
            Info = method as MethodInfo;
            var filterInfo = (FilterAttribute)method.GetCustomAttribute(typeof(FilterAttribute), false);
            if (!filterInfo.Options)
            {
                Helper = (HandlebarsReturnHelper)Info.CreateDelegate(typeof(HandlebarsReturnHelper));
            }
            else
            {
                OptionsHelper = (HandlebarsReturnWithOptionsHelper)Info.CreateDelegate(typeof(HandlebarsReturnWithOptionsHelper));
            }
            Name = filterInfo.Name;
            Description = filterInfo.Description;
            var inputInfo = (InputAttribute)method.GetCustomAttribute(typeof(InputAttribute), false);
            InputTypes = inputInfo.Types;
            var outputInfo = (OutputAttribute)method.GetCustomAttribute(typeof(OutputAttribute), false);
            OutputType = outputInfo?.Type;

            var parameters = new List<FilterParameter>();
            bool expectRequiredEnd = false;
            int requiredCount = 0;
            bool expectPositionalEnd = false;
            int positionalCount = 0;
            bool expectEnd = false;
            var keywords = new List<string>();
            foreach (var paramInfo in method.GetCustomAttributes(typeof(ParameterAttribute), false).Select(p => (ParameterAttribute)p))
            {
                if (expectEnd)
                {
                    throw new ArgumentException("No argument should follow the a multiple declarated argument");
                }
                if (paramInfo.Multiple)
                {
                    if (!(paramInfo.Name is null))
                    {
                        throw new ArgumentException("Keyword argument cannot be multiple declarated");
                    }
                    expectEnd = true;
                }
                if (paramInfo.Name is null)
                {
                    if (expectPositionalEnd)
                    {
                        throw new ArgumentException("Positional argument should not follow for a keyword declarated argument");
                    }
                    positionalCount++;
                }
                else
                {
                    expectPositionalEnd = true;
                    keywords.Add(paramInfo.Name);
                }
                if (!paramInfo.Optional)
                {
                    if (!expectPositionalEnd && expectRequiredEnd)
                    {
                        throw new ArgumentException("Required argument should not follow for a optional declarated argument");
                    }
                    requiredCount++;
                }
                else
                {
                    expectRequiredEnd = true;
                }
                FilterParameter parameter = new FilterParameter(paramInfo.Name, paramInfo.Description, paramInfo.Type, paramInfo.Default, paramInfo.Optional, paramInfo.Multiple);
                parameters.Add(parameter);
            }
            Parameters = parameters.AsReadOnly();
            PositionalParameterCount = expectEnd ? argumentLimit : positionalCount;
            RequiredParameterCount = requiredCount;
            Keywords = keywords.AsReadOnly();
        }

        public void Check(Arguments arguments)
        {

            var filterName = "{{" + Name + "}}";
            if (arguments.Length < 1)
            {
                throw new HandlebarsException(filterName + " should have at least one argument as input");
            }

            var hash = arguments.Hash;
            var posCount = (arguments.Length - (hash.Count == 0 ? 0 : -1)) - 1;
            if (posCount < RequiredParameterCount)
            {
                throw new HandlebarsException(filterName + " should have at least " + RequiredParameterCount.ToString() + " positional parameters");
            }
            if (posCount > PositionalParameterCount)
            {
                throw new HandlebarsException(filterName + " should have no more than " + PositionalParameterCount.ToString() + " positional parameters");
            }

            if (hash.Count > 0)
            {
                var unexpectedKeys = hash.Keys.ToList().Except(Keywords).ToArray();
                if (unexpectedKeys.Length > 0)
                {
                    throw new HandlebarsException(filterName + " got unexpected keyword parameters: " + unexpectedKeys.ToString());
                }
            }

        }

        public override string ToString()
        {
            return (OutputType is null ? "void" : OutputType.ToString()
                + " " + Name + "(" + InputTypes[0].ToString() + " this, "
                + string.Join(", ", Parameters.Select(p => p.ToString())) + ")");
        }

        protected MethodInfo Info { get; }
        public HandlebarsReturnHelper Helper { get; } = null;

        public HandlebarsReturnWithOptionsHelper OptionsHelper { get; } = null;
        public string Name { get; }
        public string Description { get; }
        public ReadOnlyCollection<Type> InputTypes { get; }
        public Type OutputType { get; }
        public ReadOnlyCollection<FilterParameter> Parameters { get; }

        public int RequiredParameterCount { get; }
        public int PositionalParameterCount { get; }

        public ReadOnlyCollection<string> Keywords { get; }
    }
}
