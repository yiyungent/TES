using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Extensions
{
    public static class ModelStateDicExt
    {
        #region 获取模型格式错误
        public static string GetErrorMessage(this ModelStateDictionary modelStateDictionary)
        {
            string errorMessage = string.Empty;
            foreach (ModelState item in modelStateDictionary.Values)
            {
                errorMessage += item.Errors.FirstOrDefault().ErrorMessage;
            }

            return errorMessage;
        }
        #endregion
    }
}