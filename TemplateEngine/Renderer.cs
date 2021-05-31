using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Reflection;
using HandlebarsDotNet;
using TemplateEngine.Models;
using TemplateEngine.Extensions;
using TemplateEngine.Utils;
using TemplateEngine.Attributes;

namespace TemplateEngine
{
    public class Renderer
    {
        static IEnumerable<FunctionModel> GetFunctions()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var ns = typeof(Renderer).Namespace + ".Functions";

            foreach (var extensionType in assembly.GetTypes()
                .Where(t => string.Equals(t.Namespace, ns)))
            {
                foreach (var methodInfo in extensionType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Where(m => m.GetCustomAttribute<FunctionAttribute>() != null))
                {
                    var func = new FunctionModel(methodInfo);
                    yield return func;
                }
            }
        }

        public static readonly ReadOnlyDictionary<string, FunctionModel> Functions
            = new ReadOnlyDictionary<string, FunctionModel>(
                new Dictionary<string, FunctionModel>(GetFunctions()
                    .Select(f => new KeyValuePair<string, FunctionModel>(f.Key, f)), StringComparer.OrdinalIgnoreCase));

        public Renderer()
        {
            var config = new HandlebarsConfiguration()
            {
                NoEscape = true,
            };
            _handlebars = Handlebars.Create(config);
            _handlebars.RegisterHelper("context", (HandlebarsBlockHelper)ContextBlockHelper);
            _handlebars.RegisterHelper("set", (HandlebarsHelperWithOptions)SetHelper);

            foreach (var func in Functions.Values)
            {
                var descriptor = new FunctionDescriptor(func);
                _handlebars.RegisterHelper(descriptor);
            }
        }

        public void RegisterTemplate(TemplateModel template)
        {
            _handlebars.RegisterTemplate(template.Key, template.Name);
        }

        public string Render(string templateText, object model)
        {
            var template = _handlebars.Compile(templateText);
            string text = template(model);
            return text;
        }

        public void RenderFile(string fromPath, string toPath, object model)
        {
            var templateText = File.ReadAllText(fromPath, Encoding.UTF8);
            var template = _handlebars.Compile(templateText);
            string text = template(model);
            File.WriteAllText(toPath, text, Encoding.UTF8);
        }

        static void ContextBlockHelper(EncodedTextWriter output, BlockHelperOptions options, Context context, Arguments arguments)
        {
            if (arguments.Length != 1)
            {
                throw new HandlebarsException("{{context}} block helper must have one argument");
            }

            if (arguments.Length != 1)
            {
                throw new HandlebarsException("{{context}} block helper must have one argument");
            }

            var source = arguments[0];

            if (source != null)
            {
                var type = source.GetType();

                if (type.IsPrimitive || type.IsValueType || type == typeof(string) || type.IsSubclassOf(typeof(IEnumerable<>)))
                {
                    throw new HandlebarsException("{{context}} context source should be compound type");
                }

                var dict = new VariableContext(source);
                var frame = options.CreateFrame(dict);
                options.Template(output, frame);
            }

            else
            {
                options.Inverse(output, context);
            }

        }

        private static IEnumerable<Tuple<string, BindingContext>> WalkVarFrame(string name, BindingContext frame)
        {
            while (name.StartsWith("../"))
            {
                yield return new Tuple<string, BindingContext>(name, frame);
                name = name.Substring(3);
                frame = frame.GetParent();
            }
            yield return new Tuple<string, BindingContext>(name, frame);
        }

        private static IEnumerable<Tuple<string, BindingContext>> emergeToFrame(string name, BindingContext frame)
        {
            while (name.StartsWith("../"))
            {
                yield return new Tuple<string, BindingContext>(name, frame);
                name = name.Substring(3);
                frame = frame.GetParent();
            }
            yield return new Tuple<string, BindingContext>(name, frame);
        }

        private static void SetHelper(in EncodedTextWriter output, in HelperOptions options, in Context context, in Arguments arguments)
        {
            if (arguments.Length != 2)
            {
                throw new HandlebarsException("{{set}} helper must have two arguments");
            }

            string name;
            try
            {
                name = (string)arguments[0];
            }
            catch (InvalidCastException)
            {
                throw new HandlebarsException("{{set}} helper first argument should be string");
            }

            var (alias, frame) = emergeToFrame(name, options.Frame).Last();

            var varDict = frame.Value as VariableContext;
            if (varDict is null)
            {
                throw new HandlebarsException("{{set}} helper should be used inside {{context}} block");
            }
            varDict[alias] = arguments[1];
        }

        public IHandlebars _handlebars;
    }
}
