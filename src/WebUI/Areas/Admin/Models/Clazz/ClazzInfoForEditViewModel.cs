﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Domain;

namespace WebUI.Areas.Admin.Models.Clazz
{
    public class ClazzInfoForEditViewModel
    {
        public int ID { get; set; }

        [Required]
        public string InputClazzCode { get; set; }

        public static explicit operator ClazzInfoForEditViewModel(ClazzInfo clazzInfo)
        {
            ClazzInfoForEditViewModel model = new ClazzInfoForEditViewModel
            {
                ID = clazzInfo.ID,
                InputClazzCode = clazzInfo.ClazzCode
            };

            return model;
        }
    }
}