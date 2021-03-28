using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System.Dynamic;
using YamlDotNet.Serialization;
using TemplateEngine;

namespace TemplateGenerator
{
    class Program
    {
        static void Main(string[] args)
        {            
            var deserializer = new DeserializerBuilder()
                .Build();
            var yamlText = File.ReadAllText("db.yaml", Encoding.UTF8);
            Model model = deserializer.Deserialize<Model>(yamlText);

            var renderer = new Renderer();
            var text = renderer.Render("{{#each Users}}{{add @index 1}}. {{Name}} <{{Email}}>\n{{/each}}", model);
            
            Console.WriteLine(text);
        }
    }
}
