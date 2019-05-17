using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Mvc.ViewEngines.Templates
{
    public class TemplateConfiguration
    {
        public TemplateConfiguration(string templateName, string path, string jsonStr)
        {
            this.TemplateName = templateName;
            this.Path = path;

            Dictionary<string, object> jsonDic = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonStr);
            this.PreviewImageUrl = path + "/" + "preview.png";
            if (jsonDic.ContainsKey("description"))
            {
                this.Description = jsonDic["description"].ToString();
            }
            if (jsonDic.ContainsKey("title"))
            {
                this.TemplateTitle = jsonDic["title"].ToString();
            }
            if (jsonDic.ContainsKey("authors"))
            {
                JArray authorArr = (JArray)jsonDic["authors"];
                this.Authors = new List<string>();
                foreach (var author in authorArr)
                {
                    this.Authors.Add(author.ToString());
                }
            }
        }

        public string Path { get; protected set; }

        public string PreviewImageUrl { get; protected set; }

        public string Description { get; protected set; }

        public IList<string> Authors { get; protected set; }

        public string TemplateName { get; protected set; }

        public string TemplateTitle { get; protected set; }
    }
}
