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
            InitClazz();
            InitCourse();
            InitStudent();
            InitEmployee();
            InitCourseTable();
            InitNormType();
            InitNormTarget();
            InitOptions();
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

                Container.Instance.Resolve<Sys_MenuService>().Create(new Sys_Menu()
                {
                    Name = "测试一级菜单",
                    SortCode = 50,
                    AreaName = "Admin",
                    ControllerName = "UserInfo",
                    ActionName = "Index",
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
                    ControllerName = "SysMenu",
                    ActionName = "Index",
                    AreaName = "Admin",
                    ParentMenu = parentMenu,
                    SortCode = 40,
                });
                Container.Instance.Resolve<Sys_MenuService>().Create(new Sys_Menu()
                {
                    Name = "主题模板",
                    ControllerName = "ThemeTemplate",
                    ActionName = "Index",
                    AreaName = "Admin",
                    ParentMenu = parentMenu,
                    SortCode = 50,
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
                    ControllerName = "ClazzInfo",
                    ActionName = "Index",
                    AreaName = "Admin",
                    ParentMenu = parentMenu,
                    SortCode = 30,
                });
                Container.Instance.Resolve<Sys_MenuService>().Create(new Sys_Menu()
                {
                    Name = "学生管理",
                    ControllerName = "StudentInfo",
                    ActionName = "Index",
                    AreaName = "Admin",
                    ParentMenu = parentMenu,
                    SortCode = 40,
                });
                Container.Instance.Resolve<Sys_MenuService>().Create(new Sys_Menu()
                {
                    Name = "课程管理",
                    ControllerName = "CourseInfo",
                    ActionName = "Index",
                    AreaName = "Admin",
                    ParentMenu = parentMenu,
                    SortCode = 50,
                });
                Container.Instance.Resolve<Sys_MenuService>().Create(new Sys_Menu()
                {
                    Name = "课程表管理",
                    ControllerName = "CourseTable",
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

                #region 三级菜单
                parentMenu = Container.Instance.Resolve<Sys_MenuService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "仪表盘-1")
                }).FirstOrDefault();

                Container.Instance.Resolve<Sys_MenuService>().Create(new Sys_Menu()
                {
                    Name = "仪表盘-1 - 测试三级菜单",
                    AreaName = "Admin",
                    ControllerName = "RoleInfo",
                    ActionName = "Index",
                    ParentMenu = parentMenu,
                    SortCode = 10,
                });
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
                // ID: 1
                // 特殊抽象操作---决定是否能进入管理中心
                // 只要拥有系统菜单下的任一操作权限 --> 就会拥有此对应系统菜单项 --> 就会拥有进入管理中心，即拥有此抽象的特殊操作权限(Admin.Home.Index  (后台)管理中心(框架))
                Container.Instance.Resolve<FunctionInfoService>().Create(new FunctionInfo
                {
                    AuthKey = "Admin.Home.Index",
                    Name = "(后台)管理中心(框架)"
                });

                IList<ICriterion> qryWhere = new List<ICriterion>();

                // 7-18
                qryWhere.Add(Expression.Ge("ID", 7));
                qryWhere.Add(Expression.Le("ID", 18));
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
                // 班级管理菜单 增加 调课操作
                Sys_Menu clazzInfo_Sys_Menu = Container.Instance.Resolve<Sys_MenuService>().Query(new List<ICriterion> { Expression.Eq("ControllerName", "ClazzInfo") }).FirstOrDefault();
                Container.Instance.Resolve<FunctionInfoService>().Create(new FunctionInfo
                {
                    AuthKey = "Admin.ClazzInfo.AssignCourse",
                    Name = "班级管理-调课",
                    Sys_Menu = clazzInfo_Sys_Menu
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
                    UserName = "admin",
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

        #region 初始化班级
        private void InitClazz()
        {
            try
            {
                ShowMessage("开始初始化班级表");

                for (int i = 0; i < 50; i++)
                {
                    Container.Instance.Resolve<ClazzInfoService>().Create(new ClazzInfo
                    {
                        ClazzCode = "17001" + i.ToString("00")
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

        #region 初始化课程
        private void InitCourse()
        {
            try
            {
                ShowMessage("开始初始化课程");

                string[] names = { "高等数学", "大学英语", "C#程序设计", "素质教育", "框架设计" };

                for (int i = 0; i < names.Length; i++)
                {
                    Container.Instance.Resolve<CourseInfoService>().Create(new CourseInfo
                    {
                        CourseCode = "10010" + i.ToString("00"),
                        Name = names[i]
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

        #region 初始化学生表
        private void InitStudent()
        {
            try
            {
                ShowMessage("开始初始化学生表");

                IList<RoleInfo> allRole = Container.Instance.Resolve<RoleInfoService>().GetAll();
                IList<ClazzInfo> allClazz = Container.Instance.Resolve<ClazzInfoService>().GetAll();

                for (int i = 0; i < 100; i++)
                {
                    string name = "学生" + (i + 1);
                    string studentCode = "170010" + i.ToString("000");
                    // 创建其绑定用户
                    Container.Instance.Resolve<UserInfoService>().Create(new UserInfo()
                    {
                        Name = name,
                        Avatar = "/images/default-avatar.jpg",
                        UserName = studentCode,
                        Password = EncryptHelper.MD5Encrypt32("12345"),
                        RoleInfoList = allRole.Where(m => m.Name == "学生").ToList()
                    });
                    // 创建学生
                    int randomNum = new Random().Next(0, 50);
                    Container.Instance.Resolve<StudentInfoService>().Create(new StudentInfo()
                    {
                        Name = name,
                        StudentCode = studentCode,
                        ClazzInfo = (from m in allClazz where m.ClazzCode == "17001" + randomNum.ToString("00") select m).FirstOrDefault(),
                        UID = 2 + i
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

        #region 初始化员工表
        private void InitEmployee()
        {
            try
            {
                ShowMessage("开始初始化员工表");

                var allRole = Container.Instance.Resolve<RoleInfoService>().GetAll();

                for (int i = 0; i < 100; i++)
                {
                    string name = "教师" + (i + 1);
                    string employeeCode = "120010" + i.ToString("000");
                    // 创建其绑定用户
                    Container.Instance.Resolve<UserInfoService>().Create(new UserInfo()
                    {
                        Name = name,
                        Avatar = "/images/default-avatar.jpg",
                        UserName = employeeCode,
                        Password = EncryptHelper.MD5Encrypt32("12345"),
                        RoleInfoList = allRole.Where(m => m.Name == "教师").ToList()
                    });
                    // 创建教师
                    Container.Instance.Resolve<EmployeeInfoService>().Create(new EmployeeInfo()
                    {
                        Name = name,
                        EmployeeCode = employeeCode,
                        CourseTableList = new List<CourseTable>(),
                        UID = 102 + i
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

        #region 初始化课程表
        private void InitCourseTable()
        {
            try
            {
                ShowMessage("开始初始化课程表");

                IList<ClazzInfo> allClazz = Container.Instance.Resolve<ClazzInfoService>().GetAll();
                IList<CourseInfo> allCourse = Container.Instance.Resolve<CourseInfoService>().GetAll();
                IList<EmployeeInfo> allEmployee = Container.Instance.Resolve<EmployeeInfoService>().GetAll();

                Random random = new Random();
                for (int i = 0; i < allClazz.Count; i++)
                {
                    for (int j = 0; j < allCourse.Count; j++)
                    {
                        Container.Instance.Resolve<CourseTableService>().Create(new CourseTable
                        {
                            Clazz = allClazz[i],
                            Teacher = allEmployee[random.Next(0, allEmployee.Count)],
                            Course = allCourse[j]
                        });
                    }
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

        #region 初始化指标
        private void InitNormTarget()
        {
            try
            {
                ShowMessage("开始初始化指标");
                #region 一级指标
                string[] TargetNames = new string[] { "学生方面", "系部方面", "教研室方面", "同行方面", "教师个人方面" };
                decimal[] TargetWeight = new decimal[] { 0.30m, 0.25m, 0.20m, 0.15m, 0.10m };
                for (int i = 0; i < TargetNames.Length; i++)
                {
                    Container.Instance.Resolve<NormTargetService>().Create(new NormTarget()
                    {
                        Name = TargetNames[i],
                        SortCode = (i + 1) * 10,
                        Weight = TargetWeight[i],
                    });
                }
                #endregion
                NormTarget ParentTarget = null;
                #region 学生方面的二级指标
                ParentTarget = Container.Instance.Resolve<NormTargetService>().GetEntity(1);
                string[] STargetName = new string[] { "概念的讲解", "重点和难点", "逻辑性和条理性", "趣味性和生动性", "板书", "辅导（阅读指导）", "作业与批改", "能力培养", "教书育人", "为人师表" };
                decimal[] STargetweight = new decimal[] { 0.15m, 0.15m, 0.10m, 0.10m, 0.05m, 0.08m, 0.10m, 0.10m, 0.10m, 0.07m };
                for (int i = 0; i < STargetName.Length; i++)
                {
                    Container.Instance.Resolve<NormTargetService>().Create(new NormTarget()
                    {
                        Name = STargetName[i],
                        SortCode = (i + 1) * 10,
                        Weight = STargetweight[i],
                        ParentTarget = ParentTarget,
                    });
                }
                #endregion
                #region 系部的二级指标
                ParentTarget = Container.Instance.Resolve<NormTargetService>().GetEntity(2);
                string[] DTargetName = new string[] { "量考核", "质考核" };
                decimal[] DTargetweight = new decimal[] { 0.30m, 0.70m };
                for (int i = 0; i < DTargetName.Length; i++)
                {
                    Container.Instance.Resolve<NormTargetService>().Create(new NormTarget()
                    {
                        Name = DTargetName[i],
                        SortCode = (i + 1) * 10,
                        Weight = DTargetweight[i],
                        ParentTarget = ParentTarget,
                    });
                }
                #endregion
                #region 教研室的二级指标
                ParentTarget = Container.Instance.Resolve<NormTargetService>().GetEntity(3);
                string[] TargetName = new string[] { "教学环节", "接受任务的态度", "汲取新技术", "学术与研究水平", "参加教研活动" };
                decimal[] Targetweight = new decimal[] { 0.60m, 0.05m, 0.05m, 0.10m, 0.20m };
                for (int i = 0; i < TargetName.Length; i++)
                {
                    Container.Instance.Resolve<NormTargetService>().Create(new NormTarget()
                    {
                        Name = TargetName[i],
                        SortCode = (i + 1) * 10,
                        Weight = Targetweight[i],
                        ParentTarget = ParentTarget,
                    });
                }
                #endregion
                #region 同行方面的二级指标
                ParentTarget = Container.Instance.Resolve<NormTargetService>().GetEntity(4);
                string[] TTargetName = new string[] { "组织教学", "教学内容与教学要求", "概念讲解", "重点和难点", "趣味性与生动性", "直观教学与板书", "智力能力的培养", "理论联系实际", "教材处理" };
                decimal[] TTargetweight = new decimal[] { 0.15m, 0.15m, 0.10m, 0.10m, 0.08m, 0.07m, 0.10m, 0.10m, 0.15m };
                for (int i = 0; i < TTargetName.Length; i++)
                {
                    Container.Instance.Resolve<NormTargetService>().Create(new NormTarget()
                    {
                        Name = TTargetName[i],
                        SortCode = (i + 1) * 10,
                        Weight = TTargetweight[i],
                        ParentTarget = ParentTarget,
                    });
                }
                #endregion
                #region 教师个人方面的二级指标
                ParentTarget = Container.Instance.Resolve<NormTargetService>().GetEntity(5);
                string[] ETargetName = new string[] { "自我评价", "自我评价的工作" };
                decimal[] ETargetweight = new decimal[] { 0.5m, 0.5m };
                for (int i = 0; i < ETargetName.Length; i++)
                {
                    Container.Instance.Resolve<NormTargetService>().Create(new NormTarget()
                    {
                        Name = ETargetName[i],
                        SortCode = (i + 1) * 10,
                        Weight = ETargetweight[i],
                        ParentTarget = ParentTarget,
                    });
                }
                #endregion
                #region 教学环节的三级指标
                ParentTarget = Container.Instance.Resolve<NormTargetService>().GetEntity(18);//18
                string[] name = new string[] { "概念的讲解", "重点和难点", "逻辑性、条理性", "趣味性、生动性", "板书", "能力培养", "理论联系实际", "辅导（阅读指导）", "作业与批改" };
                decimal[] weight = new decimal[] { 0.15m, 0.15m, 0.10m, 0.10m, 0.05m, 0.15m, 0.10m, 0.10m, 0.10m };
                for (int i = 0; i < name.Length; i++)
                {
                    Container.Instance.Resolve<NormTargetService>().Create(new NormTarget()
                    {
                        Name = name[i],
                        SortCode = (i + 1) * 10,
                        Weight = weight[i],
                        ParentTarget = ParentTarget,
                    });
                }
                #endregion
                #region 量考核的三级指标
                ParentTarget = Container.Instance.Resolve<NormTargetService>().GetEntity(16);//18
                string[] Lname = new string[] { "教学工作量", "社会工作量", "任课班级" };
                decimal[] Lweight = new decimal[] { 0.75m, 0.15m, 0.10m };
                for (int i = 0; i < Lname.Length; i++)
                {
                    Container.Instance.Resolve<NormTargetService>().Create(new NormTarget()
                    {
                        Name = Lname[i],
                        SortCode = (i + 1) * 10,
                        Weight = Lweight[i],
                        ParentTarget = ParentTarget,
                    });
                }
                #endregion
                #region 质考核的三级指标
                ParentTarget = Container.Instance.Resolve<NormTargetService>().GetEntity(17);//18
                string[] Zname = new string[] { "工作态度", "学术与研究水平", "完成任务情况", "教学水平变化", "教学反映", "能力培养", "汲取新信息新技术", "考试命题" };
                decimal[] Zweight = new decimal[] { 0.40m, 0.15m, 0.05m, 0.05m, 0.15m, 0.10m, 0.05m, 0.05m };
                for (int i = 0; i < Zname.Length; i++)
                {
                    Container.Instance.Resolve<NormTargetService>().Create(new NormTarget()
                    {
                        Name = Zname[i],
                        SortCode = (i + 1) * 10,
                        Weight = Zweight[i],
                        ParentTarget = ParentTarget,
                    });
                }
                #endregion
                #region 工作态度的四级指标
                ParentTarget = Container.Instance.Resolve<NormTargetService>().GetEntity(46);
                string[] Gname = new string[] { "接受任务态度", "教学常规" };
                decimal[] Gweight = new decimal[] { 0.10m, 0.90m };
                for (int i = 0; i < Gname.Length; i++)
                {
                    Container.Instance.Resolve<NormTargetService>().Create(new NormTarget()
                    {
                        Name = Gname[i],
                        SortCode = (i + 1) * 10,
                        Weight = Gweight[i],
                        ParentTarget = ParentTarget,
                    });
                }
                #endregion
                #region 学术与研究水平的四级指标
                ParentTarget = Container.Instance.Resolve<NormTargetService>().GetEntity(47);
                string[] XGname = new string[] { "职称", "运用新知识、新技术能力", "论文撰写、教材编写能力" };
                decimal[] XGweight = new decimal[] { 0.10m, 0.40m, 0.50m };
                for (int i = 0; i < Gname.Length; i++)
                {
                    Container.Instance.Resolve<NormTargetService>().Create(new NormTarget()
                    {
                        Name = XGname[i],
                        SortCode = (i + 1) * 10,
                        Weight = XGweight[i],
                        ParentTarget = ParentTarget,
                    });
                }
                #endregion
                #region 教学常规的五级指标
                ParentTarget = Container.Instance.Resolve<NormTargetService>().GetEntity(55);
                string[] Jname = new string[] { "授课计划的制定", "教案首页、备课余量", "教学日志手册的填写", "教学表格的填写", "辅导", "教学秩序的掌握" };
                decimal[] Jweight = new decimal[] { 0.10m, 0.20m, 0.10m, 0.10m, 0.10m, 0.20m, 0.20m };
                for (int i = 0; i < Jname.Length; i++)
                {
                    Container.Instance.Resolve<NormTargetService>().Create(new NormTarget()
                    {
                        Name = Jname[i],
                        SortCode = (i + 1) * 10,
                        Weight = Jweight[i],
                        ParentTarget = ParentTarget,
                    });
                }
                #endregion
                ShowMessage("成功");
            }
            catch (Exception)
            {
                ShowMessage("失败");
            }
        }
        #endregion

        #region 初始化指标选项
        private void InitOptions()
        {
            try
            {
                ShowMessage("开始初始化指标选项");
                string[] contents = { };
                decimal[] scores = { };
                NormTarget normTarget = null;
                #region 概念的讲解的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "概念的讲解")
                }).FirstOrDefault();
                contents = new string[] { "语言精练，深入浅出，讲解准确", "讲解清晰，容易接受", "讲解基本准确，但不易接受", "概念紊乱，时有差错" };
                scores = new decimal[] { 1m, 0.85m, 0.65m, 0.45m };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        Score = scores[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 重点和难点的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "重点和难点")
                }).FirstOrDefault();
                contents = new string[] { "重点突出，讲清难点，举一反三", "能把握重点、难点，但讲解不够明确", "重点不明显，难点讲不透", "重点一言而过，难点草率了事" };
                scores = new decimal[] { 1m, 0.85m, 0.65m, 0.45m };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        Score = scores[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 逻辑性和条理性的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "逻辑性和条理性")
                }).FirstOrDefault();
                contents = new string[] { "层次分明，融会贯通", "条目较清楚，有分析归纳", "平淡叙述，缺乏连贯性", "杂乱无章，前后矛盾" };
                scores = new decimal[] { 1m, 0.85m, 0.65m, 0.45m };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        Score = scores[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 趣味性和生动性的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "趣味性和生动性")
                }).FirstOrDefault();
                contents = new string[] { "讲解方法新颖，举例生动，有吸引力", "讲解较熟练，语言通俗", "讲解平淡，语言单调", "讲解生疏，远离课题，语言枯燥" };
                scores = new decimal[] { 1m, 0.85m, 0.65m, 0.45m };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        Score = scores[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 板书的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "板书")
                }).FirstOrDefault();
                contents = new string[] { "简繁适度，清楚醒目", "条目明白，书写整洁", "布局较差，详略失当", "次序凌乱，书写潦草" };
                scores = new decimal[] { 1m, 0.85m, 0.65m, 0.45m };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        Score = scores[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 辅导（阅读指导）的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "辅导（阅读指导）")
                }).FirstOrDefault();
                contents = new string[] { "辅导及时、并指导课外阅读", "定期辅导，并布置课外阅读", "辅导较少", "没有辅导" };
                scores = new decimal[] { 1m, 0.85m, 0.65m, 0.45m };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        Score = scores[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 作业与批改的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "作业与批改")
                }).FirstOrDefault();
                contents = new string[] { "选题得当，批改及时，注意讲评", "作业适量，批改及时", "作业量时轻时重，批改不够及时", "选题随便，批改马虎" };
                scores = new decimal[] { 1m, 0.85m, 0.65m, 0.45m };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        Score = scores[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 能力培养的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "能力培养")
                }).FirstOrDefault();
                contents = new string[] { "思路开阔，鼓励创新，注意能力培养、效果明显", "注意学生能力培养，并在教学中有所体现", "能提出能力培养的要求，但缺乏具体的办法", "忽视能力培养，单纯灌输书本知识" };
                scores = new decimal[] { 1m, 0.85m, 0.65m, 0.45m };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        Score = scores[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 教书育人的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "教书育人")
                }).FirstOrDefault();
                contents = new string[] { "全面关心学生，经常接触学生，亲切、严格", "关心学生的学业，引导学生学好本门课程", "单纯完成上课任务，与同学接触较少", "对学生漠不关心，放任自流" };
                scores = new decimal[] { 1m, 0.85m, 0.65m, 0.45m };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        Score = scores[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 为人师表的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "为人师表")
                }).FirstOrDefault();
                contents = new string[] { "严于律己，以身作则，堪称楷模", "举止文明，待人热情", "注意礼貌，待人和气", "要求不严，言谈失当" };
                scores = new decimal[] { 1m, 0.85m, 0.65m, 0.45m };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        Score = scores[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 理论联系实际的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "理论联系实际")
                }).FirstOrDefault();
                contents = new string[] { "理论紧密联系当前实际", "理论能联系具体事例", "联系实际较勉强", "只有理论没有实际" };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 接受任务的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "接受任务")
                }).FirstOrDefault();
                contents = new string[] { "勇挑重担", "主动承担", "一般", "较差" };
                scores = new decimal[] { };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 汲取新信息技术情况的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "汲取新信息技术情况")
                }).FirstOrDefault();
                contents = new string[] { "及时在教学中体现", "教学中注意联系新信息新技术", "教学中联系新信息新技术不够", "教学中根本不联系新信息新技术" };
                scores = new decimal[] { };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 学术与研究水平的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "学术与研究水平")
                }).FirstOrDefault();
                contents = new string[] { "开出有一定水平的选修课、讲座、院级公开课或指导兴趣小组有成效", "开出选修课、讲座、系内公开课或指导兴趣小组活动", "承担室内研究课、协助开出讲座或配合指导学生课外活动", "无" };
                scores = new decimal[] { };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 参加教研活动的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "参加教研活动")
                }).FirstOrDefault();
                contents = new string[] { "出主意、提建议、协助室主任搞好教研活动", "积极参加讨论", "能按时参加活动", "参加活动不正常" };
                scores = new decimal[] { };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 教学工作量的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "教学工作量")
                }).FirstOrDefault();
                contents = new string[] { "超工作量", "满工作量", "接近完成（70%）", "差距较大" };
                scores = new decimal[] { };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 社会工作量的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "社会工作量")
                }).FirstOrDefault();
                contents = new string[] { "担任教研室主任", "担任办公室、工作室主任", "担任专业班主任（辅导员）等其他工作", "未承担" };
                scores = new decimal[] { };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 任课班级的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "任课班级")
                }).FirstOrDefault();
                contents = new string[] { "任4个班级以上，或双进度", "任3个班级", "任2个班级", "任1个班级" };
                scores = new decimal[] { };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 接受任务态度的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "接受任务态度")
                }).FirstOrDefault();
                contents = new string[] { "勇挑重担", "主动承担", "一般", "较差" };
                scores = new decimal[] { };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 授课计划制定的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "授课计划制定")
                }).FirstOrDefault();
                contents = new string[] { "清晰", "完整", "一般", "潦草" };
                scores = new decimal[] { };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 教案首页的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "教案首页")
                }).FirstOrDefault();
                contents = new string[] { "完整", "缺一项", "缺二项", "缺二项以上" };
                scores = new decimal[] { };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 备课余量的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "备课余量")
                }).FirstOrDefault();
                contents = new string[] { "一周以上", "一周", "接近一周", "没有余量" };
                scores = new decimal[] { };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 教学日志的填写的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "教学日志的填写")
                }).FirstOrDefault();
                contents = new string[] { "清楚、准确", "正确、及时", "填写缺项", "填写马虎" };
                scores = new decimal[] { };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 教学表格的填写的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "教学表格的填写")
                }).FirstOrDefault();
                contents = new string[] { "认真且有见解", "详尽、整洁", "正确", "潦草、拖拉" };
                scores = new decimal[] { };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 辅导、作业的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "辅导、作业")
                }).FirstOrDefault();
                contents = new string[] { "每周有辅导", "辅导较经常", "辅导较少", "不辅导" };
                scores = new decimal[] { };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 教学秩序的掌握的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "教学秩序的掌握")
                }).FirstOrDefault();
                contents = new string[] { "严格", "较严格", "一般", "出现教学事故" };
                scores = new decimal[] { };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 职称的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "教学秩序的掌握")
                }).FirstOrDefault();
                contents = new string[] { "副教授", "讲师", "助教", "未评职称" };
                scores = new decimal[] { };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 运用新知识、新技术能力的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "运用新知识、新技术能力")
                }).FirstOrDefault();
                contents = new string[] { "开出有一定水平的选修课、讲座、院级公开课或指导兴趣小组有成效", "开出选修课、讲座、科内公开课或指导兴趣小组活动", "承担室内研究课、协助开出讲座或配合指导学生课外活动", "无" };
                scores = new decimal[] { };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 论文撰写、教材编写能力的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "论文撰写、教材编写能力")
                }).FirstOrDefault();
                contents = new string[] { "在核心刊物上发表、教材正式出版（三年内）", "在公开刊物上发表，教材兄弟院校使用（二年内）", "在内部刊上发表，教材在校内使用（一年内）", "无" };
                scores = new decimal[] { };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 完成任务情况的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "完成任务情况")
                }).FirstOrDefault();
                contents = new string[] { "高质量完成", "及时完成", "基本完成", "完不成" };
                scores = new decimal[] { };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 教学水平变化的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "教学水平变化")
                }).FirstOrDefault();
                contents = new string[] { "显著提高", "有所提高", "变化很小", "下降" };
                scores = new decimal[] { };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 教学（效果）反映的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "教学（效果）反映")
                }).FirstOrDefault();
                contents = new string[] { "优秀", "良好", "一般", "较差" };
                scores = new decimal[] { };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 汲取新信息技术情况的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "汲取新信息技术情况")
                }).FirstOrDefault();
                contents = new string[] { "及时在教学中体现", "教学中注意联系新信息新技术", "教学中联系新信息新技术不够", "教学中根本不联系新信息新技术" };
                scores = new decimal[] { };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 考试命题的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "考试命题")
                }).FirstOrDefault();
                contents = new string[] { "试题的水平、题型、题量、分布、覆盖面符合教学目标的要求；难度适中，区分度适当；表述准确、严密、简洁。", "有2项不符合要求", "有3项不符合要求", "有3项以上（不含3项）不符合要求" };
                scores = new decimal[] { };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 组织教学的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "考试命题")
                }).FirstOrDefault();
                contents = new string[] { "教学组织安排得当，气氛活跃，纪律良好", "注意学生动态，教学有条不紊", "忽视教学步骤，师生双边活动较差", "只顾自己讲，不管学生情况" };
                scores = new decimal[] { };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 教学要求、教学内容的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "教学要求、教学内容")
                }).FirstOrDefault();
                contents = new string[] { "切合教学大纲要求与实际，内容组织科学严密", "符合教学大纲要求，内容正确", "基本达到教学大纲要求，内容偶有差错", "降低教学标准，内容时有差错" };
                scores = new decimal[] { };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 概念讲解的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "概念讲解")
                }).FirstOrDefault();
                contents = new string[] { "语言精练，深入浅出，讲解准确", "讲解清晰，容易接受", "讲解基本准确，但不易接受", "概念紊乱，时有差错" };
                scores = new decimal[] { };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 重点和难点的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "重点和难点")
                }).FirstOrDefault();
                contents = new string[] { "重点突出，讲清难点，举一反三", "能把握重点、难点，但讲解不够明确", "重点不明显，难点讲不透", "重点一言而过，难点草率了事" };
                scores = new decimal[] { };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 直观教学与板书的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "直观教学与板书")
                }).FirstOrDefault();
                contents = new string[] { "教具使用合理，板书清晰，示教形象、直观", "注意直观教学，板书条目明白、整洁", "教具使用失当，板书布局较差", "忽视直观教学，板书凌乱" };
                scores = new decimal[] { };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 智力能力的培养的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "智力能力的培养")
                }).FirstOrDefault();
                contents = new string[] { "运用各种方法，调动学生积极思维，注重能力培养", "注意调动学生思维和能力培养，方法和效果欠佳", "缺乏启发式方法和能力培养手段", "照本宣科，不搞启发式教学" };
                scores = new decimal[] { };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 理论联系实际的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "理论联系实际")
                }).FirstOrDefault();
                contents = new string[] { "理论与实例、实验、实际密切结合", "理论能结合实际进行教学", "理论与实际结合不理想", "理论与实际严重脱节" };
                scores = new decimal[] { };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                #region 教材处理的选项
                normTarget = Container.Instance.Resolve<NormTargetService>().Query(new List<ICriterion>
                {
                    Expression.Eq("Name", "教材处理")
                }).FirstOrDefault();
                contents = new string[] { "科学的处理教材，繁简增删适当，收事半功倍之效", "对教材的处理，有助于学生理解和掌握内在联系", "基本按照教材讲课，没有给学生什么新东西", "对教材毫无处理，完全重复课本内容" };
                scores = new decimal[] { };
                for (int i = 0; i < contents.Length; i++)
                {
                    Container.Instance.Resolve<OptionsService>().Create(new Options
                    {
                        Content = contents[i],
                        SortCode = 10 * (i + 1),
                        NormTarget = normTarget
                    });
                }
                #endregion
                ShowMessage("成功");
            }
            catch (Exception ex)
            {
                ShowMessage("失败");
            }
        }
        #endregion

        #region 初始化指标体系类型
        private void InitNormType()
        {
            try
            {
                ShowMessage("开始初始化指标体系类型");

                string[] names = { "教师工作评价指标体系（学生用）", "教师工作评价指标体系（教研室主任用）", "教师工作评价指标体系（系主任用）", "教师工作评价指标体系（领导、督导、同行用）" };

                for (int i = 0; i < names.Length; i++)
                {
                    Container.Instance.Resolve<NormTypeService>().Create(new NormType
                    {
                        Name = names[i],
                        SortCode = 10 * (i + 1)
                    });
                }

                ShowMessage("成功");
            }
            catch (Exception ex)
            {
                ShowMessage("失败");
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