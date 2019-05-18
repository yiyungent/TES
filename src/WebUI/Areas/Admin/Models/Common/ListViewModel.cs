using Core;
using Domain;
using Domain.Base;
using Framework.HtmlHelpers;
using Service;
using Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Areas.Admin.Models.Common
{
    public class ListViewModel<T> : ListViewModel
    //where T : BaseEntity<T>
    {
        public IList<T> List { get; set; }

        public override IList<dynamic> DyList
        {
            get
            {
                IList<dynamic> rtn = new List<dynamic>();
                foreach (T item in this.List)
                {
                    rtn.Add(item);
                }

                return rtn;
            }
        }

        public ListViewModel(int pageIndex, int pageSize)
        {
            dynamic tempAllList = null;
            switch (typeof(T).ToString())
            {
                case "Domain.ClazzInfo":
                    tempAllList = Container.Instance.Resolve<ClazzInfoService>().GetAll();
                    break;
                case "Domain.CourseTable":
                    tempAllList = Container.Instance.Resolve<CourseTableService>().GetAll();
                    break;
                case "Domain.UserInfo":
                    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                    stopwatch.Start();
                    tempAllList = Container.Instance.Resolve<UserInfoService>().GetAll();
                    stopwatch.Stop();
                    TimeSpan t1 = stopwatch.Elapsed; // 1s
                    break;
                case "Domain.RoleInfo":
                    tempAllList = Container.Instance.Resolve<RoleInfoService>().GetAll();
                    break;
                case "Domain.CourseInfo":
                    tempAllList = Container.Instance.Resolve<CourseInfoService>().GetAll();
                    break;
                case "Domain.StudentInfo":
                    tempAllList = Container.Instance.Resolve<StudentInfoService>().GetAll();
                    break;
                case "Domain.EmployeeInfo":
                    tempAllList = Container.Instance.Resolve<EmployeeInfoService>().GetAll();
                    break;
            }
            IList<dynamic> allList = tempAllList;
            // 当前页号超过总页数，则显示最后一页
            int lastPageIndex = (int)Math.Ceiling((double)allList.Count / pageSize);
            pageIndex = pageIndex <= lastPageIndex ? pageIndex : lastPageIndex;
            IEnumerable<dynamic> data = (from m in allList
                                         orderby m.ID descending
                                         select m).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            IList<T> tempList = new List<T>();
            foreach (dynamic item in data)
            {
                tempList.Add((T)item);
            }

            #region PS
            //Unable to cast object of type 'System.Collections.Generic.List`1[System.Object]' to type 'System.Collections.Generic.IList`1[Domain.RoleInfo]'.
            //this.List = (IList<T>)data.ToList(); 
            #endregion

            this.List = tempList;
            this.PageInfo = new PageInfo
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalRecordCount = allList.Count,
                MaxLinkCount = 10
            };
        }
    }

    public class ListViewModel
    {
        public virtual PageInfo PageInfo { get; set; }

        public virtual IList<dynamic> DyList { get; }
    }
}