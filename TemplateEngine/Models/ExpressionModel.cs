using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace TemplateEngine.Models
{
    public abstract class ExpressionModel
    {
        protected ExpressionModel(string key, string name, string description, IEnumerable<ArgumentModel> args)
        {
            Debug.Assert(key?.Length > 0, "Key should be set");
            Debug.Assert(name?.Length > 0, "Name should be set");

            int positionalCount = 0;
            int requiredCount = 0;
            bool eofPositional = false;
            bool eofRequired = false;

            var arguments = new List<ArgumentModel>();
            var kwIndices = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var i = 0;
            foreach (var arg in args)
            {
                var byKeyword = arg.ByKeyword;
                var isOptional = arg.IsOptional;

                if (eofPositional && !byKeyword)
                    throw new ArgumentException($"Argument '{key}' cannot be positional");

                if (!eofPositional && byKeyword)
                    eofPositional = true;

                if (!eofRequired && isOptional)
                    eofRequired = true;

                if (!eofRequired) ++requiredCount;
                if (!eofPositional) ++positionalCount;

                arguments.Add(arg);
                arg.Position = i++;
                if (byKeyword)
                    kwIndices[arg.Key] = arg.Position;
            }

            Key = key;
            Name = name;
            Description = description;
            PosArgumentCount = positionalCount;
            ReqArgumentCount = requiredCount;
            Arguments = arguments.AsReadOnly();
            KeywordIndices = new ReadOnlyDictionary<string, int>(kwIndices);
        }
        public string Key { get; }
        public string Name { get; }
        public string Description { get; }
        public int PosArgumentCount { get; protected set; }
        public int ReqArgumentCount { get; protected set; }
        public ReadOnlyCollection<ArgumentModel> Arguments { get; }
        public ReadOnlyDictionary<string, int> KeywordIndices { get; }
    }
}
