using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPB.BusinessLogic.Managers.Exceptions;
using UPB.BusinessLogic.Models;

namespace UPB.BusinessLogic.Managers
{
    public class PatientManager
    {
        private List<Patient> _patients;

        public PatientManager()
        {
            _patients = new List<Patient>();

            readPatientsToList();
        }

        public Patient CreatePatient(Patient patient)
        {
            Patient createdPatient = new Patient()
            {
                Name = patient.Name,
                LastName = patient.LastName,
                CI = patient.CI,
                BloodType = GetRandomBloodType()
            };

            _patients.Add(createdPatient);

            writeListToFile();

            return createdPatient;
        }

        public Patient UpdatePatient(int ci, Patient UpdatedPatient)
        {
            Patient? patientToUpdate = _patients.Find(x => x.CI == ci);

            if(patientToUpdate == null)
            {
                throw new PatientNotFoundException();
            }

            patientToUpdate.Name = UpdatedPatient.Name;
            patientToUpdate.LastName = UpdatedPatient.LastName;
            patientToUpdate.BloodType = UpdatedPatient.BloodType;

            writeListToFile();

            return patientToUpdate;
        }

        public List<Patient> Delete(int ci)
        {
            Patient? patientToDelete = _patients.Find(x => x.CI == ci);

            if (patientToDelete == null)
            {
                throw new PatientNotFoundException();
            }

            _patients.Remove(patientToDelete);

            writeListToFile();

            return _patients;
        }

        public List<Patient> GetPatients()
        {
            return _patients;
        }

        public Patient GetPatientsBiCI(int ci)  
        {
            Patient? foundPatientByCI = _patients.Find(x => x.CI == ci);

            if (foundPatientByCI == null)
            {
                throw new PatientNotFoundException();
            }

            return foundPatientByCI;
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

        private void readPatientsToList()
        {
            StreamReader reader = new StreamReader("D:\\Cert I\\SP\\Practice2\\Patients.txt");

            _patients.Clear();

            while(!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] patientInfo = line.Split(",");

                Patient newPatient = new Patient()
                {
                    Name = patientInfo[0],
                    LastName = patientInfo[1],
                    CI = int.Parse(patientInfo[2]),
                    BloodType = patientInfo[3]
                };
                _patients.Add(newPatient);
            }
            reader.Close();
        }

        private void writeListToFile()
        {
            StreamWriter writer = new StreamWriter("D:\\Cert I\\SP\\Practice2\\Patients.txt");
            foreach(var patient in _patients)
            {
                string[] patientInfo = { patient.Name, patient.LastName, $"{patient.CI}", patient.BloodType};
                writer.WriteLine(string.Join(",", patientInfo));
            }
            writer.Close();
        }
    }
}
