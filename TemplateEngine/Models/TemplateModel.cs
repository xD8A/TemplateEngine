using System;
using System.Collections.Generic;
using System.Text;

namespace TemplateEngine.Models
{
    public class TemplateModel : ExpressionModel
    {
        protected TemplateModel(string key, string name, string description, IEnumerable<ArgumentModel> args) : base(key, name, description, args)
        { }
    }
}
