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

namespace TemplateEngine
{
    public class Renderer
    {
        public Renderer()
        {
            var config = new HandlebarsConfiguration()
            {
                NoEscape = true,
            };
            _handlebars = Handlebars.Create(config);
            _handlebars.RegisterHelper("context", (HandlebarsBlockHelper)ContextBlockHelper);
            _handlebars.RegisterHelper("set", (HandlebarsHelperWithOptions)SetHelper);
            foreach (var filter in filters)
            {
                if (!(filter.Helper is null))
                {
                    _handlebars.RegisterHelper(filter.Name, filter.Helper);
                }
                else
                {
                    _handlebars.RegisterHelper(filter.Name, filter.OptionsHelper);
                }
            }
            foreach (var (partialName, templateText) in GetPartialTemplateTexts())
            {
                _handlebars.RegisterTemplate(partialName, templateText);
            }
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

            var dict = new VariableContext(arguments[0]);
            var frame = options.CreateFrame(dict);
            options.Template(output, frame);
        }

        static private IEnumerable<Tuple<string, BindingContext>> WalkVarFrame(string name, BindingContext frame)
        {
            while (name.StartsWith("../"))
            {
                yield return new Tuple<string, BindingContext>(name, frame);
                name = name.Substring(3);
                frame = frame.GetParent();
            }
            yield return new Tuple<string, BindingContext>(name, frame);
        }

        static void SetHelper(in EncodedTextWriter output, in HelperOptions options, in Context context, in Arguments arguments)
        {
            if (arguments.Length != 2)
            {
                throw new HandlebarsException("{{set}} helper must have two arguments");
            }

            var (alias, frame) = WalkVarFrame((string)arguments[0], options.Frame).Last();

            var varDict = frame.Value as VariableContext;
            if (varDict is null)
            {
                throw new HandlebarsException("{{set}} helper should be used inside {{context}} block");
            }
            varDict[alias] = arguments[1];
        }

        static IEnumerable<Filter> GetFilters()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var ns = typeof(Renderer).Namespace + ".Filters";

            foreach (var extensionType in assembly.GetTypes()
                .Where(t => string.Equals(t.Namespace, ns)))
            {
                foreach (var methodInfo in extensionType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Where(m => m.Name.EndsWith("Filter")))
                {
                    yield return new Filter(methodInfo);
                }
            }
        }

        static IEnumerable<Tuple<Type, Filter[]>> BuildTypeFilters(IEnumerable<Filter> filters)
        {
            var dict = new Dictionary<Type, List<Filter>>();

            foreach (var filter in filters)
            {
                foreach (var inputType in filter.InputTypes)
                {
                    if (!dict.TryGetValue(inputType, out List<Filter> typeFilters))
                    {
                        typeFilters = new List<Filter>();
                        dict[inputType] = typeFilters;
                    }
                    typeFilters.Add(new Filter(filter, inputType));
                }
            }
            var types = dict.Keys.ToArray();
            int n = types.Length;
            for (var i = 0; i < n - 1; i++)
            {
                var leftType = types[i];
                var leftFilters = dict[leftType];
                for (var j = i + 1; j < n; j++)
                {
                    var rightType = types[j];
                    var rightFilters = dict[rightType];
                    if (leftType.IsSubclassOf(rightType))
                    {
                        leftFilters.AddRange(rightFilters);
                    }
                    if (rightType.IsSubclassOf(leftType))
                    {
                        rightFilters.AddRange(leftFilters);
                    }
                }
            }
            foreach (KeyValuePair<Type, List<Filter>> entry in dict)
            {
                yield return new Tuple<Type, Filter[]>(entry.Key, entry.Value.ToArray());
            }
        }

        protected virtual IEnumerable<Tuple<string, string>> GetPartialTemplateTexts()
        {
            return Enumerable.Empty<Tuple<string, string>>();
        }


        public static readonly ReadOnlyCollection<Filter> filters = GetFilters().ToList().AsReadOnly();
        public static readonly ReadOnlyDictionary<Type, ReadOnlyCollection<Filter>> typeFilters = new ReadOnlyDictionary<Type, ReadOnlyCollection<Filter>>(
            BuildTypeFilters(filters).ToDictionary(x => x.Item1, x => x.Item2.ToList().AsReadOnly()));

        public IHandlebars _handlebars;
    }
}
