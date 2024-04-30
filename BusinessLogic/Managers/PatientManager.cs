using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPB.BusinessLogic.Models;

namespace UPB.BusinessLogic.Managers
{
    internal class PatientManager
    {
        public PatientManager() { }

        public Patient CreatePatient(string name, string lastname, int CI)
        {
            Patient createdPatient = new Patient()
            {
                Name = name,
                LastName = lastname,
                CI = CI,
                BloodType = GetRandomBloodType()
            };

            return createdPatient;
        }

        public string GetRandomBloodType()
        {
            Random rand = new Random();
            int random_value = rand.Next(0, 81);

            if(random_value < 10)
            {
                return "A+";
            }

            if(random_value < 20)
            {
                return "A-";
            }

            if (random_value < 30)
            {
                return "B+";
            }

            if (random_value < 40)
            {
                return "B-";
            }

            if (random_value < 50)
            {
                return "AB+";
            }

            if (random_value < 60)
            {
                return "AB-";
            }

            if (random_value < 70)
            {
                return "O+";
            }

            return "O-";
        }
    }
}
