using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using UPB.BusinessLogic.Managers.Exceptions;
using UPB.BusinessLogic.Models;

namespace UPB.BusinessLogic.Managers
{
    public class PatientManager
    {
        private List<Patient> _patients;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly string? _url;

        public PatientManager(IConfiguration configuration)
        {
            _patients = new List<Patient>();
            _configuration = configuration;

            _httpClient = new HttpClient();

            _url = _configuration.GetSection("HttpRequestsInfo").GetSection("baseAddress").Value;

            if(_url == null)
            {
                throw new JSONValueNotFoundException(["HttpRequestsInfo", "baseAddress"]);
            }

            ReadPatientsToList();
        }

        public async Task<Patient> CreatePatient(Patient patient)
        {
            if(CheckIfPatientExists(patient))
            {
                throw new PatientAlreadyExistsException();
            }

            await CreatePatientCode(patient);

            Task<string> result = GetPatientCodeByCI(patient);

            Patient createdPatient = new Patient()
            {
                Name = patient.Name,
                LastName = patient.LastName,
                CI = patient.CI,
                BloodType = GetRandomBloodType(),
                Code = result.Result
            };

            _patients.Add(createdPatient);

            WriteListToFile();

            return createdPatient;
        }

        public async Task<Patient> UpdatePatient(int ci, Patient UpdatedPatient)
        {
            Patient? patientToUpdate = _patients.Find(x => x.CI == ci);

            if(patientToUpdate == null)
            {
                throw new PatientNotFoundException("UpdatePatient");
            }

            patientToUpdate.Name = UpdatedPatient.Name;
            patientToUpdate.LastName = UpdatedPatient.LastName;

            await UpdatePatientCode(patientToUpdate);

            Task<string> result = GetPatientCodeByCI(patientToUpdate);

            patientToUpdate.Code = result.Result;

            WriteListToFile();

            return patientToUpdate;
        }

        public async Task<List<Patient>> Delete(int ci)
        {
            Patient? patientToDelete = _patients.Find(x => x.CI == ci);

            if (patientToDelete == null)
            {
                throw new PatientNotFoundException("Delete");
            }

            await DeletePatientCode(patientToDelete);

            _patients.Remove(patientToDelete);

            WriteListToFile();

            return _patients;
        }

        public List<Patient> GetPatients()
        {
            return _patients;
        }

        public Patient GetPatientsByCI(int ci)  
        {
            Patient? foundPatientByCI = _patients.Find(x => x.CI == ci);

            if (foundPatientByCI == null)
            {
                throw new PatientNotFoundException("GetPatientsByCI");
            }

            return foundPatientByCI;
        }

        private static string GetRandomBloodType()
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

        private async void ReadPatientsToList()
        {
            string? patientsFile = _configuration.GetSection("FilePaths").GetSection("PatientFile").Value;

            if(patientsFile == null)
            {
                throw new JSONValueNotFoundException(["FilePaths", "PatientFile"]);
            }

            StreamReader reader = new StreamReader(patientsFile);

            Log.Information("Loading Patients from file.");

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
                    BloodType = patientInfo[3],
                    Code = patientInfo[4]
                };

                await CreatePatientCode(newPatient);

                _patients.Add(newPatient);
            }
            reader.Close();
        }

        private void WriteListToFile()
        {
            string? patientsFile = _configuration.GetSection("FilePaths").GetSection("PatientFile").Value;

            if (patientsFile == null)
            {
                throw new JSONValueNotFoundException(["FilePaths", "PatientFile"]);
            }

            StreamWriter writer = new StreamWriter(patientsFile);

            Log.Information("Saving patients to the file.");

            foreach(var patient in _patients)
            {
                string[] patientInfo = { patient.Name, patient.LastName, $"{patient.CI}", patient.BloodType, patient.Code };
                writer.WriteLine(string.Join(",", patientInfo));
            }
            writer.Close();
        }

        private bool CheckIfPatientExists(Patient patient)
        {
            Patient? foundPatient = _patients.Find(x => x.CI == patient.CI);

            if(foundPatient == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private string GetCodeFromJsonAsString(string json)
        {
            string values;
            string codeValuePair;
            string codeValue;

            values = json.Split("{")[1].Split("}")[0];

            codeValuePair = values.Split(",")[3];

            codeValue = codeValuePair.Split(":")[1];

            return codeValue;
        }

        private async Task<string> GetPatientCodeByCI(Patient patient)
        {
            string? postEndpoint = _configuration.GetSection("HttpRequestsInfo").GetSection("GetEndpoint").Value;

            if (postEndpoint == null)
            {
                throw new JSONValueNotFoundException(["HttpRequestsInfo", "GetEndpoint"]);
            }

            var getResult = await _httpClient.GetAsync(_url + postEndpoint + $"/{patient.CI}");

            string result = await getResult.Content.ReadAsStringAsync();

            string code = GetCodeFromJsonAsString(result);

            Console.WriteLine($"The code is {code}");

            return code;
        }

        private async Task CreatePatientCode(Patient patient)
        {
            string? postEndpoint = _configuration.GetSection("HttpRequestsInfo").GetSection("PostEndpoint").Value;

            if(postEndpoint == null)
            {
                throw new JSONValueNotFoundException(["HttpRequestsInfo", "PostEndpoint"]);
            }

            var patientInfo = new {
                Name = patient.Name,
                LastName = patient.LastName,
                CI = patient.CI,
                Code = patient.Code
            };

            var postResult = await _httpClient.PostAsJsonAsync(_url + postEndpoint, patientInfo);
        }

        private async Task UpdatePatientCode(Patient patient)
        {
            string? postEndpoint = _configuration.GetSection("HttpRequestsInfo").GetSection("PutEndpoint").Value;

            if (postEndpoint == null)
            {
                throw new JSONValueNotFoundException(["HttpRequestsInfo", "PutEndpoint"]);
            }

            var patientInfo = new
            {
                Name = patient.Name,
                LastName = patient.LastName,
                CI = patient.CI,
                Code = patient.Code
            };

            var putResult = await _httpClient.PutAsJsonAsync(_url + postEndpoint + $"/{patient.CI}", patientInfo);
        }

        private async Task DeletePatientCode(Patient patient)
        {
            string? postEndpoint = _configuration.GetSection("HttpRequestsInfo").GetSection("DeleteEndpoint").Value;

            if (postEndpoint == null)
            {
                throw new JSONValueNotFoundException(["HttpRequestsInfo", "DeleteEndpoint"]);
            }

            var deleteResult = await _httpClient.DeleteAsync(_url + postEndpoint + $"/{patient.CI}");
        }
    }
}
