using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using HandlebarsDotNet;

namespace TemplateEngine.Utils
{

    class VariableContext : Dictionary<string, object>
    {
        public VariableContext(object source = null) : base()
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
                foreach (var (key, value) in dict)
                {
                    this[key] = value;
                }
            }
            else
            {
                var type = other.GetType();
                if (!type.IsPrimitive && !type.IsValueType && type != typeof(string))
                {
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
