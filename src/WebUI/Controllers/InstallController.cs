using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Castle.ActiveRecord;
using Core;
using Domain;
using Service;
using NHibernate.Criterion;
using Framework.Common;

namespace WebUI.Controllers
{
    public class InstallController : Controller
    {
        #region 安装首页
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region 开始安装
        public ViewResult StartInstall()
        {
            CreateDB();

            return View("Index");
        }
        #endregion

        #region 创建数据库
        private void CreateDB()
        {
            CreateSchema();
            InitSys_Menu();
            InitFunction();
            InitRole();
            InitUser();
            InitStudent();
            InitTeacher();
        }
        #endregion

        #region 创建数据库表结构
        private void CreateSchema()
        {
            try
            {
                ShowMessage("开始创建数据库表结构");
                ActiveRecordStarter.CreateSchema();
                ShowMessage("成功");
            }
            catch (Exception ex)
            {
                ShowMessage("失败");
                ShowMessage(ex.Message);
            }
        }
        #endregion

        #region 初始化系统菜单表
        private void InitSys_Menu()
        {
            try
            {
                ShowMessage("开始初始化系统菜单表");

                #region 一级菜单
                Container.Instance.Resolve<Sys_MenuService>().Create(new Sys_Menu()
                {
                    Name = "系统管理",
                    SortCode = 10,
                });
                Container.Instance.Resolve<Sys_MenuService>().Create(new Sys_Menu()
                {
                    Name = "业务管理",
                    SortCode = 20,
                });
                Container.Instance.Resolve<Sys_MenuService>().Create(new Sys_Menu()
                {
                    Name = "评价管理",
                    SortCode = 30,
                });
                Container.Instance.Resolve<Sys_MenuService>().Create(new Sys_Menu()
                {
                    Name = "仪表盘",
                    SortCode = 40,
                });
                #endregion

                #region 二级菜单
                // 一级菜单项---二级菜单的父菜单项
                Sys_Menu parentMenu = null;


                #region 系统管理的二级菜单
                parentMenu = Container.Instance.Resolve<Sys_MenuService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "系统管理")
                }).FirstOrDefault();

                Container.Instance.Resolve<Sys_MenuService>().Create(new Sys_Menu()
                {
                    Name = "站点信息",
                    ControllerName = "BaseInfo",
                    ActionName = "Index",
                    AreaName = "Admin",
                    ParentMenu = parentMenu,
                    SortCode = 10,
                });
                Container.Instance.Resolve<Sys_MenuService>().Create(new Sys_Menu()
                {
                    Name = "用户管理",
                    ControllerName = "UserInfo",
                    ActionName = "Index",
                    AreaName = "Admin",
                    ParentMenu = parentMenu,
                    SortCode = 20,
                });
                Container.Instance.Resolve<Sys_MenuService>().Create(new Sys_Menu()
                {
                    Name = "角色管理",
                    ControllerName = "RoleInfo",
                    ActionName = "Index",
                    AreaName = "Admin",
                    ParentMenu = parentMenu,
                    SortCode = 30,
                });
                Container.Instance.Resolve<Sys_MenuService>().Create(new Sys_Menu()
                {
                    Name = "菜单管理",
                    ControllerName = "Menu",
                    ActionName = "Index",
                    AreaName = "Admin",
                    ParentMenu = parentMenu,
                    SortCode = 10,
                });
                #endregion

                #region 业务管理的二级菜单
                parentMenu = Container.Instance.Resolve<Sys_MenuService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "业务管理")
                }).FirstOrDefault();

                Container.Instance.Resolve<Sys_MenuService>().Create(new Sys_Menu()
                {
                    Name = "操作管理",
                    ControllerName = "FunctionInfo",
                    ActionName = "Index",
                    AreaName = "Admin",
                    ParentMenu = parentMenu,
                    SortCode = 10,
                });
                Container.Instance.Resolve<Sys_MenuService>().Create(new Sys_Menu()
                {
                    Name = "部门管理",
                    ControllerName = "Department",
                    ActionName = "Index",
                    AreaName = "Admin",
                    ParentMenu = parentMenu,
                    SortCode = 20,
                });
                Container.Instance.Resolve<Sys_MenuService>().Create(new Sys_Menu()
                {
                    Name = "班级管理",
                    ControllerName = "Clazz",
                    ActionName = "Index",
                    AreaName = "Admin",
                    ParentMenu = parentMenu,
                    SortCode = 30,
                });
                Container.Instance.Resolve<Sys_MenuService>().Create(new Sys_Menu()
                {
                    Name = "学生管理",
                    ControllerName = "Student",
                    ActionName = "Index",
                    AreaName = "Admin",
                    ParentMenu = parentMenu,
                    SortCode = 40,
                });
                Container.Instance.Resolve<Sys_MenuService>().Create(new Sys_Menu()
                {
                    Name = "课程管理",
                    ControllerName = "Course",
                    ActionName = "Index",
                    AreaName = "Admin",
                    ParentMenu = parentMenu,
                    SortCode = 50,
                });
                Container.Instance.Resolve<Sys_MenuService>().Create(new Sys_Menu()
                {
                    Name = "课表管理",
                    ControllerName = "TimeTable",
                    ActionName = "Index",
                    AreaName = "Admin",
                    ParentMenu = parentMenu,
                    SortCode = 60,
                });
                #endregion

                #region 评价管理的二级菜单
                parentMenu = Container.Instance.Resolve<Sys_MenuService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "评价管理")
                }).FirstOrDefault(); ;

                Container.Instance.Resolve<Sys_MenuService>().Create(new Sys_Menu()
                {
                    Name = "评价指标",
                    ControllerName = "EvaluationIndex",
                    ActionName = "Index",
                    AreaName = "Admin",
                    ParentMenu = parentMenu,
                    SortCode = 10,
                });
                Container.Instance.Resolve<Sys_MenuService>().Create(new Sys_Menu()
                {
                    Name = "评价选项",
                    ControllerName = "EvaluationOperation",
                    ActionName = "Index",
                    AreaName = "Admin",
                    ParentMenu = parentMenu,
                    SortCode = 20,
                });
                Container.Instance.Resolve<Sys_MenuService>().Create(new Sys_Menu()
                {
                    Name = "评价任务",
                    ControllerName = "EvaluationTask",
                    ActionName = "Index",
                    AreaName = "Admin",
                    ParentMenu = parentMenu,
                    SortCode = 30,
                });
                #endregion

                #region 仪表盘的二级菜单
                parentMenu = Container.Instance.Resolve<Sys_MenuService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "仪表盘")
                }).FirstOrDefault();

                Container.Instance.Resolve<Sys_MenuService>().Create(new Sys_Menu()
                {
                    Name = "仪表盘-1",
                    ControllerName = "Dashboard",
                    ActionName = "One",
                    AreaName = "Admin",
                    ParentMenu = parentMenu,
                    SortCode = 10,
                });
                #endregion

                #endregion

                ShowMessage("成功");
            }
            catch (Exception ex)
            {
                ShowMessage("失败");
                ShowMessage(ex.Message);
            }
        }
        #endregion

        #region 初始化操作表
        /// <summary>
        /// 放入此表，即此action需要权限验证
        /// </summary>
        private void InitFunction()
        {
            try
            {
                ShowMessage("开始初始化操作表");

                Container.Instance.Resolve<FunctionInfoService>().Create(new FunctionInfo
                {
                    AuthKey = "Admin.Home.Index",
                    Name = "后台管理(框架)"
                });

                IList<ICriterion> qryWhere = new List<ICriterion>();

                // 5-16
                qryWhere.Add(Expression.Ge("ID", 5));
                qryWhere.Add(Expression.Le("ID", 16));
                IList<Sys_Menu> findMenuList = Container.Instance.Resolve<Sys_MenuService>().Query(qryWhere);
                string[] funcNames = { "新增", "修改", "删除", "查看" };
                string[] actionNames = { "Create", "Edit", "Delete", "Detail", "Index" };
                foreach (var menu in findMenuList)
                {
                    for (int i = 0; i < actionNames.Length; i++)
                    {
                        if (i == actionNames.Length - 1)
                        {
                            Container.Instance.Resolve<FunctionInfoService>().Create(new FunctionInfo()
                            {
                                Name = menu.Name + "-首页",
                                AuthKey = menu.AreaName + "." + menu.ControllerName + "." + actionNames[i],
                                Sys_Menu = menu
                            });
                        }
                        else
                        {
                            Container.Instance.Resolve<FunctionInfoService>().Create(new FunctionInfo()
                            {
                                Name = menu.Name + "-" + funcNames[i],
                                AuthKey = menu.AreaName + "." + menu.ControllerName + "." + actionNames[i],
                                Sys_Menu = menu
                            });
                        }
                    }
                }

                // 角色RoleInfo菜单 增加授权操作
                Sys_Menu roleInfo_Sys_Menu = Container.Instance.Resolve<Sys_MenuService>().Query(new List<ICriterion> { Expression.Eq("ControllerName", "RoleInfo") }).FirstOrDefault();
                Container.Instance.Resolve<FunctionInfoService>().Create(new FunctionInfo
                {
                    AuthKey = "Admin.RoleInfo.AssignPower",
                    Name = "角色管理-授权",
                    Sys_Menu = roleInfo_Sys_Menu
                });


                ShowMessage("成功");
            }
            catch (Exception ex)
            {
                ShowMessage("失败");
                ShowMessage(ex.Message);
            }
        }
        #endregion

        #region 初始化角色表
        private void InitRole()
        {
            try
            {
                ShowMessage("开始初始化角色表");

                var allMenu = Container.Instance.Resolve<Sys_MenuService>().GetAll();
                var allFunction = Container.Instance.Resolve<FunctionInfoService>().GetAll();

                Container.Instance.Resolve<RoleInfoService>().Create(new RoleInfo
                {
                    Name = "超级管理员",
                    Status = 0,
                    Sys_MenuList = allMenu,
                    FunctionInfoList = allFunction
                });

                Container.Instance.Resolve<RoleInfoService>().Create(new RoleInfo
                {
                    Name = "游客",
                    Status = 0,
                    Sys_MenuList = null,
                    FunctionInfoList = null
                });

                Container.Instance.Resolve<RoleInfoService>().Create(new RoleInfo
                {
                    Name = "学生",
                    Status = 0,
                    Sys_MenuList = null,
                    FunctionInfoList = null
                });

                Container.Instance.Resolve<RoleInfoService>().Create(new RoleInfo
                {
                    Name = "教师",
                    Status = 0,
                    Sys_MenuList = null,
                    FunctionInfoList = null
                });

                ShowMessage("成功");
            }
            catch (Exception ex)
            {
                ShowMessage("失败");
                ShowMessage(ex.Message);
            }
        }
        #endregion

        #region 初始化用户表
        private void InitUser()
        {
            try
            {
                ShowMessage("开始初始化用户表");

                var allRole = Container.Instance.Resolve<RoleInfoService>().GetAll();

                Container.Instance.Resolve<UserInfoService>().Create(new UserInfo()
                {
                    Name = "超级管理员admin",
                    LoginAccount = "admin",
                    Avatar = "/images/default-avatar.jpg",
                    Password = EncryptHelper.MD5Encrypt32("admin"),
                    Status = 0,
                    RoleInfoList = (from m in allRole where m.ID == 1 select m).ToList(),
                    RegTime = DateTime.Now
                });

                ShowMessage("成功");
            }
            catch (Exception ex)
            {
                ShowMessage("失败");
                ShowMessage(ex.Message);
            }
        }
        #endregion

        #region 初始化学生表
        private void InitStudent()
        {
            try
            {
                ShowMessage("开始初始化学生表");

                IList<RoleInfo> allRole = Container.Instance.Resolve<RoleInfoService>().GetAll();

                for (int i = 0; i < 100; i++)
                {
                    string name = "学生" + (i + 1);
                    string studentCode = "170010" + i.ToString("000");
                    // 创建其绑定用户
                    Container.Instance.Resolve<UserInfoService>().Create(new UserInfo()
                    {
                        Name = name,
                        Avatar = "/images/default-avatar.jpg",
                        LoginAccount = studentCode,
                        Password = EncryptHelper.MD5Encrypt32("12345"),
                        RoleInfoList = allRole.Where(m => m.Name == "学生").ToList()
                    });
                    // 创建学生
                    Container.Instance.Resolve<StudentInfoService>().Create(new StudentInfo()
                    {
                        Name = name,
                        StudentCode = studentCode,
                        UserInfo_Account = studentCode
                    });
                }

                ShowMessage("成功");
            }
            catch (Exception ex)
            {
                ShowMessage("失败");
                ShowMessage(ex.Message);
            }
        }
        #endregion

        #region 初始化教师表
        private void InitTeacher()
        {
            try
            {
                ShowMessage("开始初始化教师表");

                var allRole = Container.Instance.Resolve<RoleInfoService>().GetAll();

                for (int i = 0; i < 100; i++)
                {
                    string name = "教师" + (i + 1);
                    string teacherCode = "120010" + i.ToString("000");
                    // 创建其绑定用户
                    Container.Instance.Resolve<UserInfoService>().Create(new UserInfo()
                    {
                        Name = name,
                        Avatar = "/images/default-avatar.jpg",
                        LoginAccount = teacherCode,
                        Password = EncryptHelper.MD5Encrypt32("12345"),
                        RoleInfoList = allRole.Where(m => m.Name == "教师").ToList()
                    });
                    // 创建学生
                    Container.Instance.Resolve<TeacherInfoService>().Create(new TeacherInfo()
                    {
                        Name = name,
                        TeacherCode = teacherCode,
                        UserInfo_Account = teacherCode
                    });
                }

                ShowMessage("成功");
            }
            catch (Exception ex)
            {
                ShowMessage("失败");
                ShowMessage(ex.Message);
            }
        }
        #endregion

        #region 输出消息
        private void ShowMessage(string message)
        {
            Response.Write(message + "<br>");
            Response.Flush();
        }
        #endregion
    }
}