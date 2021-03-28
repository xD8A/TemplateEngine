using System.Reflection;
using HandlebarsDotNet;

namespace TemplateEngine.Extensions
{
    public static class BindingContextExtension
    {
        public static BindingContext GetRoot(this BindingContext frame)
        {
            var propInfo = frame.GetType().GetProperty("Root", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var root = (BindingContext)propInfo.GetValue(frame);
            return root;
        }

        public static BindingContext GetParent(this BindingContext frame)
        {
            var propInfo = frame.GetType().GetProperty("ParentContext", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var parent = (BindingContext)propInfo.GetValue(frame);
            return parent;
        }

    }
}
