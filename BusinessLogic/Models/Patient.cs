﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPB.BusinessLogic.Models
{
    public class Patient
    {
        public string Name {  get; set; }
        public string LastName {  get; set; }

        public int CI {  get; set; }

        public string BloodType {  get; set; }

        public string Code { get; set; }

        public Patient() { }

        public Patient(string Name, string LastName, int CI, string BloodType, string code)
        {
            this.Name = Name;
            this.LastName = LastName;
            this.CI = CI;
            this.BloodType = BloodType;
            Code = code;
        }
    }
}
