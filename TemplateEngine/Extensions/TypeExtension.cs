using System;
using System.Collections.Generic;


namespace TemplateEngine.Extensions
{
    public static class TypeExtension
    {
        public static bool IsCastableTo(this Type type, Type descendantType, bool strict = false)
        {
            if (type == descendantType || type.IsSubclassOf(descendantType))
                return true;
            if (strict)
                return false;
            if (type.IsArray && descendantType.IsArray)
                return IsCastableTo(type.GetElementType(), descendantType.GetElementType(), strict);
            // TODO: generic types
            return false;
        }

        public static bool IsCastableFrom(this Type type, Type ancestorType, bool strict = false)
        {
            return IsCastableTo(ancestorType, type, strict);
        }

        public static bool IsCastableTo(this Type type, IEnumerable<Type> descendantTypes, bool strict = false)
        {
            foreach (var descendantType in descendantTypes)
            {
                if (IsCastableTo(type, descendantType, strict))
                    return true;
            }
            return false;
        }

        public static bool IsCastableFrom(this Type type, IEnumerable<Type> ancestorTypes, bool strict = false)
        {
            foreach (var ancestorType in ancestorTypes)
            {
                if (IsCastableFrom(type, ancestorType, strict))
                    return true;
            }
            return false;
        }

        public static object CastFrom(this Type resultType, object obj, bool strict = false)
        {
            var objType = obj.GetType();
            if (IsCastableFrom(resultType, objType, false))
                return obj;
            if (objType.IsArray && resultType.IsArray)
            {
                var dynObj = (dynamic)obj;
                int n = dynObj.Length;
                var resultArr = Array.CreateInstance(resultType, n);
                return Array.Copy(dynObj, resultArr, n);
            }
            return null;
        }

    }

}
