using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Areas.Admin.Models.EvaResultVM
{
    public class EvaResultViewModel
    {
        public EvaTask EvaTask { get; set; }

        public EmployeeInfo EmployeeInfo { get; set; }

        public IList<EvaResult> EvaResultList { get; set; }

        public EvaResultViewModel()
        {
            EvaResultList = new List<EvaResult>();
        }
    }
}