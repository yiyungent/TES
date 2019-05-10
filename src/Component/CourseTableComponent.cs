﻿using Component.Base;
using Domain;
using Manager;
using Service;

namespace Component
{
    public class CourseTableComponent : BaseComponent<CourseTable, CourseTableManager>, CourseTableService
    {
    }
}
