using Framework.Common;
using Framework.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Mvc.ViewEngines.Templates
{
    public class TemplateProvider : ITemplateProvider
    {
        #region Fields

        private readonly IList<TemplateConfiguration> _templateConfigurations = new List<TemplateConfiguration>();
        private readonly string _basePath = string.Empty;

        #endregion

        #region Constructors

        public TemplateProvider(AppConfig appConfig, IWebHelper webHelper)
        {
            _basePath = webHelper.MapPath(appConfig.ThemeBasePath);
            LoadConfigurations();
        }

        #endregion
    }
}
