using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using TemplateEngine.Attributes;


namespace TemplateEngine.Models
{
    public class FunctionModel : ExpressionModel
    {
        private static IEnumerable<ArgumentModel> CreateArguments(MethodInfo info)
        {
            var funcAttr = info.GetCustomAttribute<FunctionAttribute>();
            if (funcAttr is null)
                throw new ArgumentException($"Function '{info.Name}' should have attribute");
            var parameters = info.GetParameters().ToArray();
            var paramAttrs = info.GetCustomAttributes<ArgumentAttribute>().ToArray();

            ArgumentAttribute prevParamAttr = null;

            for (var i = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];
                var paramAttr = paramAttrs.Length > 0 ? paramAttrs[i] : null;
                Type[] supportedTypes = null;
                var byKeyword = false;
                var allowMultiple = false;
                string key = null;
                if (paramAttr != null)
                {
                    supportedTypes = paramAttr.SupportedTypes?.ToArray();
                    byKeyword = paramAttr.ByKeyword;
                    allowMultiple = paramAttr.AllowMultiple;
                    key = paramAttr.Key;
                    if (paramAttr.AsPrevious)
                    {
                        if (prevParamAttr is null)
                            throw new ArgumentException($"Argument {param.Name} of function {info.Name} no previous");
                        if (prevParamAttr.SupportedTypes is null)
                            throw new ArgumentException($"Argument {param.Name} of function {info.Name} has no previous supported types ");
                        supportedTypes = prevParamAttr.SupportedTypes.ToArray();
                        paramAttr = prevParamAttr;
                    }
                }
                prevParamAttr = paramAttr;

                yield return new ArgumentModel(param, supportedTypes, byKeyword, allowMultiple, key);
            }


        }
        public FunctionModel(MethodInfo info) : base(
            info.GetCustomAttribute<FunctionAttribute>().Key ?? info.Name,
            Properties.Resources.ResourceManager.GetString(info.GetCustomAttribute<FunctionAttribute>().Key ?? info.Name),
            Properties.Resources.ResourceManager.GetString(info.GetCustomAttribute<FunctionAttribute>().Key ?? info.Name + "Description"),
            CreateArguments(info)
        )
        {
            var funcAttr = info.GetCustomAttribute<FunctionAttribute>();
            ResultTypeByInput = funcAttr.ResultTypeByInput;
            Info = info;
        }

        public Type GetResultTypeByInput(object[] parameters)
        {
            if (ResultTypeByInput >= 0)
                return parameters[ResultTypeByInput].GetType();
            return ResultType;
        }

        public int ResultTypeByInput { get; }
        public Type ResultType { get => Info.ReturnType; }
        public MethodInfo Info { get; }
    }
}
