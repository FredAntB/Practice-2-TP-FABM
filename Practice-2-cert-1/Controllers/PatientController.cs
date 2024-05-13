using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UPB.BusinessLogic.Models;
using UPB.BusinessLogic.Managers;

namespace UPB.Practice_2_cert_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly PatientManager _patientManager;

        public PatientController(PatientManager patientManager)
        {
            _patientManager = patientManager;
        }

        [HttpGet]
        public List<Patient> Get()
        {
            return _patientManager.GetPatients();
        }

        [HttpGet]
        [Route("{ci}")]
        public Patient Get(int ci)
        {
            return _patientManager.GetPatientsByCI(ci);
        }

        [HttpPost]
        public async void Post([FromBody] Patient value)
        {
            await _patientManager.CreatePatient(value);
        }

        [HttpPut("{ci}")]
        public async void Put(int ci, [FromBody] Patient value)
        {
            await _patientManager.UpdatePatient(ci, value);
        }

        [HttpDelete("{ci}")]
        public async void Delete(int ci)
        {
            await _patientManager.Delete(ci);
        }
    }
}
