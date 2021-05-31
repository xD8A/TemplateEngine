using System;
using System.Collections.Generic;
using System.Reflection;


namespace TemplateEngine.Utils
{
    public class VariableContext : Dictionary<string, object>
    {

        public VariableContext(object source = null) : base(StringComparer.OrdinalIgnoreCase)
        {
            if (!(source is null))
            {
                Update(source);
            }
        }

        public void Update(object other)
        {
            if (other is null)
            {
                throw new ArgumentException("Object cannot be null");
            }
            var dict = other as IDictionary<string, object>;
            if (dict != null)
            {
                foreach (var pair in dict)
                {
                    this[pair.Key] = pair.Value;
                }
            }
            else
            {
                var type = other.GetType();
                if (!type.IsPrimitive && !type.IsValueType && type != typeof(string) && !type.IsSubclassOf(typeof(IEnumerable<>)))
                {
                    foreach (var fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
                    {
                        this[fieldInfo.Name] = fieldInfo.GetValue(other);
                    }
                    foreach (var propInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    {
                        this[propInfo.Name] = propInfo.GetValue(other);
                    }
                }
                else
                {
                    throw new ArgumentException("Should be compound type (use with instead)");
                }
            }
        }


    }
}
