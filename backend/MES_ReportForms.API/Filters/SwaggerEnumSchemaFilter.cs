using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Xml.Linq;

namespace MES_ReportForms.API.Filters
{
    /// <summary>
    /// 
    /// </summary>
    public class SwaggerEnumSchemaFilter : ISchemaFilter
    {
        private readonly Dictionary<string, XDocument> xmlCacher = new Dictionary<string, XDocument>();

        private static object loadLocker = new object();

        private XDocument GetAssemblyComment(Type type)
        {
            string typeAssemblyCommentXml = type.Assembly.GetName().Name + ".xml";
            if (xmlCacher.ContainsKey(typeAssemblyCommentXml))
                return xmlCacher[typeAssemblyCommentXml];

            lock (loadLocker)
            {
                string xdocPath = Path.Combine(AppContext.BaseDirectory, typeAssemblyCommentXml);
                if (!File.Exists(xdocPath))
                {
                    xmlCacher.Add(typeAssemblyCommentXml, null);
                    return null;
                }
                else
                {
                    var xdoc = XDocument.Load(xdocPath);
                    xmlCacher.Add(typeAssemblyCommentXml, xdoc);
                    return xdoc;
                }
            }
        }

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema.Enum != null && schema.Enum.Count > 0 &&
                context.Type != null)
            {

                schema.Description += " (";
                var enumType = context.Type.IsGenericType ? context.Type.GetGenericArguments()[0] : context.Type;
                var enumTypeFullName = enumType.FullName;
                var _xmlComments = GetAssemblyComment(enumType);

                foreach (var enumMemberValue in schema.Enum.OfType<OpenApiInteger>().
                         Select(v => v.Value))
                {
                    string desc = null;
                    var enumMemberName = Enum.GetName(enumType, enumMemberValue);

                    if (_xmlComments != null)
                    {
                        var fullEnumMemberName = $"F:{enumTypeFullName}.{enumMemberName}";

                        var enumMemberComments = _xmlComments.Descendants("member")
                            .FirstOrDefault(m => m.Attribute("name").Value.Equals
                            (fullEnumMemberName, StringComparison.OrdinalIgnoreCase));

                        if (enumMemberComments != null)
                        {
                            var summary = enumMemberComments.Descendants("summary").FirstOrDefault();
                            if (summary != null)
                                desc = summary.Value.Trim();
                        }
                    }

                    if (string.IsNullOrWhiteSpace(desc))
                    {
                        desc = enumMemberName;
                    }

                    schema.Description += $"{enumMemberValue}:{desc}; ";
                }
                schema.Description = schema.Description.Trim().TrimEnd(';');
                schema.Description += ")";
            }
        }
    }
}
