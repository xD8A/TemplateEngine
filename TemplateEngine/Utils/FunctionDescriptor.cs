using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HandlebarsDotNet;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.PathStructure;
using TemplateEngine.Extensions;
using TemplateEngine.Models;


namespace TemplateEngine.Utils
{
    internal class FunctionDescriptor : IHelperDescriptor<HelperOptions>
    {
        public FunctionDescriptor(FunctionModel model)
        {
            _name = model.Key;
            Model = model;
        }

        private object[] GetParameters(Arguments arguments)
        {
            var funcArgs = Model.Arguments;
            var argHash = arguments.Hash;
            var hasKeyword = argHash?.Count > 0;

            var posParams = hasKeyword ? arguments.SkipLast(1).ToArray() : arguments.ToArray();
            var parameters = new List<object>();

            var i = 0;
            for (; i < Model.PosArgumentCount; i++)
            {
                var funcArg = funcArgs[i];
                if (i >= posParams.Length)
                {
                    if (i < Model.ReqArgumentCount)
                        throw new HandlebarsException($"{{{{{Name}}}}} required argument (# {i + 1}) not passed");
                    break;
                }

                if (!funcArg.AllowMultiple)
                {
                    var param = funcArg.ExpectedType.CastFrom(posParams[i]);
                    parameters.Add(param);
                }
                else
                {
                    var elemType = funcArg.ExpectedType.GetElementType();
                    var param = elemType.CastFrom(posParams[i]);
                    var multParam = new List<object>() { param };
                    for (var j = i + 1; j < posParams.Length; j++)
                    {
                        param = elemType.CastFrom(posParams[i]);
                        multParam.Add(param);
                    }
                    var paramArr = funcArg.ExpectedType.CastFrom(multParam.ToArray());
                    parameters.Add(paramArr);
                }
            }
            for (; i < Model.PosArgumentCount; i++)
            {
                var funcArg = funcArgs[i];
                parameters.Add(funcArg.DefaultValue);
            }
            if (hasKeyword)
            {
                foreach (var kv in argHash)
                {
                    var key = kv.Key;
                    var value = kv.Value;
                    i = Model.KeywordIndices[key];
                    var funcArg = funcArgs[i];
                    while (parameters.Count < i)
                        parameters.Add(funcArg.DefaultValue);
                    var param = funcArg.ExpectedType.CastFrom(value);
                    if (i < parameters.Count)
                        parameters[i] = param;
                    else
                        parameters.Add(param);
                }
            }
            for (i = parameters.Count; i < Model.Arguments.Count; i++)
            {
                var funcArg = funcArgs[i];
                parameters.Add(funcArg.DefaultValue);
            }
            return parameters.ToArray();
        }

        public object Invoke(in HelperOptions options, in Context context, in Arguments arguments)
        {
            var parameters = GetParameters(arguments);
            var result = Model.Info.Invoke(null, parameters);
            return result;
        }

        public void Invoke(in EncodedTextWriter output, in HelperOptions options, in Context context, in Arguments arguments)
        {
            output.WriteSafeString(Invoke(options, context, arguments));
        }

        private PathInfo _name;
        public PathInfo Name => _name;
        public FunctionModel Model { get; }
    }

}
