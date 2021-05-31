using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using TemplateEngine.Extensions;


namespace TemplateEngine.Models
{
    public class ArgumentModel
    {
        protected internal ArgumentModel(ParameterInfo param, Type[] supportedTypes = null, bool byKeyword = false, bool allowMultiple = false, string key = null)
        {
            if (param.IsOut)
                throw new ArgumentException($"Argument '{param.Name}' cannot be output");

            var paramType = param.ParameterType;
            bool isOptional = param.DefaultValue == DBNull.Value;
            object defaultValue = isOptional ? param.DefaultValue : null;
            if (byKeyword && (Key is null))
                throw new ArgumentException($"Keyword argument '{param.Name}' should have key");
            if (byKeyword && !isOptional)
                throw new ArgumentException($"Keyword argument '{param.Name}' should have default value");
            allowMultiple = allowMultiple || param.GetCustomAttributes(typeof(ParamArrayAttribute), false).Length > 0;
            if (allowMultiple && byKeyword)
                throw new ArgumentException($"Argument '{param.Name}' cannot be both multiple and keyword");

            if (supportedTypes is null)
            {
                
                if (!allowMultiple)
                {
                    supportedTypes = new Type[] { paramType };
                }
                else 
                {
                    if (!paramType.IsArray)
                        throw new ArgumentException($"Argument '{param.Name}' have to be an array type");
                    var elementType = paramType.GetElementType();
                    supportedTypes = new Type[] { elementType };
                }
            }
            else
            {
                if (!allowMultiple)
                {
                    if (!paramType.IsCastableFrom(supportedTypes))
                        throw new ArgumentException($"Argument '{param.Name}' has not supported type");
                }
                else
                {
                    var elementType = paramType.GetElementType();
                    if (!elementType.IsCastableFrom(supportedTypes))
                        throw new ArgumentException($"Argument '{param.Name}' has not supported type");
                }

            }

            Position = -1;
            Key = key ?? param.Name;
            Name = Properties.Resources.ResourceManager.GetString(Key);
            Debug.Assert(Name?.Length > 0, "Name of the function argument not found in resources");
            Description = Properties.Resources.ResourceManager.GetString(Key + "Description");
            ExpectedType = paramType;
            SupportedTypes = Array.AsReadOnly<Type>(supportedTypes);
            IsOptional = isOptional;
            AllowMultiple = allowMultiple;
            ByKeyword = byKeyword;
            DefaultValue = defaultValue;
        }

        public ArgumentModel(string key, string name, string description, Type expectedType, bool isOptional, bool allowMultiple, bool byKeyword, object defaultValue = null)
        {
            Position = -1;
            Key = key;
            Name = name;
            Description = description;
            ExpectedType = expectedType;
            SupportedTypes = Array.AsReadOnly(new Type[] { expectedType });
            IsOptional = isOptional;
            AllowMultiple = allowMultiple;
            ByKeyword = byKeyword;
            DefaultValue = defaultValue;
        }

        public int Position { get; protected internal set; }
        public string Key { get; }
        public string Name { get; }
        public string Description { get; }
        public Type ExpectedType { get; }
        public ReadOnlyCollection<Type> SupportedTypes { get; }
        public bool IsOptional { get; }
        public bool AllowMultiple { get; }
        public bool ByKeyword { get; }
        public object DefaultValue { get; }
    }


}
