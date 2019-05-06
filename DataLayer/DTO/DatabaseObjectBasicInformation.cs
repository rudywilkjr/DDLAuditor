﻿using System;

namespace DataLayer.DTO
{
    public class DatabaseObjectBasicInformation
    {
        public string DatabaseName { get; set; }
        public string ObjectSchema { get; set; }
        public string ObjectName { get; set; }
        public DateTime LastDdlChange { get; set; }
    }
}
