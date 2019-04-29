﻿using Core;
using Domain;
using Framework.Infrastructure.Abstract;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Infrastructure.Concrete
{
    public class UserInfo_Guest
    {
        private static UserInfo _instance;

        private static IDBAccessProvider _dBAccessProvider;

        public static UserInfo Instance
        {
            get
            {
                if (_instance == null)
                {
                    _dBAccessProvider = new DBAccessProvider();

                    UserInfo guest = new UserInfo
                    {
                        Name = "游客(未登录)",
                        Avatar = "/images/default-avatar.jpg",
                        RoleInfoList = new List<RoleInfo>
                        {
                            //Container.Instance.Resolve<RoleInfoService>().GetEntity(2)
                            _dBAccessProvider.GetGuestRoleInfo()
                        }
                    };

                    _instance = guest;
                }

                return _instance;
            }
        }
    }
}
