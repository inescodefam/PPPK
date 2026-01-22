using MedApi.DTOs;
using MedCore.Entities;
using MedCore.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class PatientsController : ControllerBase
    {
        private readonly IPatientRepository _patientRepository;

        public PatientsController(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        // GET: api/patients
        [HttpGet]
        public ActionResult<List<PatientDto>> GetAllPatients()
        {
            var patients = _patientRepository.GetAll();
            return Ok(patients);
        }

        // GET: api/patients/5
        [HttpGet("{id}")]
        public ActionResult<PatientDto> GetPatient(int id)
        {
            var patient = _patientRepository.GetById(id);

            if (patient == null)
                return NotFound($"Patient with ID {id} not found.");

            return Ok(patient);
        }

        // GET: api/patients/oib/12345678901
        [HttpGet("oib/{oib}")]
        public ActionResult<PatientDto> GetPatientByOIB(string oib)
        {
            var patient = _patientRepository.GetByOIB(oib);

            if (patient == null)
                return NotFound($"Patient with OIB {oib} not found.");

            return Ok(patient);
        }

        // GET: api/patients/search?lastName=Horvat
        [HttpGet("search")]
        public ActionResult<List<PatientDto>> SearchPatients([FromQuery] string lastName)
        {
            var patients = _patientRepository.SearchByLastName(lastName);
            return Ok(patients);
        }

        // GET: api/patients/5/details
        [HttpGet("{id}/details")]
        public ActionResult<PatientDto> GetPatientWithDetails(int id)
        {
            var patient = _patientRepository.GetPatientWithDetails(id);

            if (patient == null)
                return NotFound($"Patient with ID {id} not found.");

            return Ok(patient);
        }

        // POST: api/patients
        [HttpPost]
        public ActionResult<PatientDto> CreatePatient([FromBody] PatientBaseDto patientDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (patientDto.OIB.Length != 11)
                return BadRequest("OIB must be exactly 11 digits.");

            var existing = _patientRepository.GetByOIB(patientDto.OIB);
            if (existing != null)
                return Conflict($"Patient with OIB {patientDto.OIB} already exists.");

            if(patientDto.OIB.Any(c => !char.IsDigit(c)) || patientDto.OIB.Length != 11)
                return BadRequest("OIB must contain 11 digits.");

            Patient patient = new Patient
            {
                FirstName = patientDto.FirstName,
                LastName = patientDto.LastName,
                OIB = patientDto.OIB,
                DateOfBirth = patientDto.DateOfBirth,
                Gender = patientDto.Gender,
                ResidenceAddress = patientDto.ResidenceAddress,

            };

            try
            {
                _patientRepository.Add(patient);
                _patientRepository.Save();
                
            }
            catch (Exception e)
            {
                return StatusCode(500, "An error occurred while creating or saving the patient.\n " + e.Message);
            }

            return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
        }

        // PUT: api/patients/5
        [HttpPut("{id}")]
        public ActionResult UpdatePatient(int id, [FromBody] PatientBaseDto patientDto)
        {
            var existing = _patientRepository.GetById(id);
            if (existing == null)
                return NotFound($"Patient with ID {id} not found.");

            Patient patient = new Patient
            {
                Id = id,
                FirstName = patientDto.FirstName,
                LastName = patientDto.LastName,
                OIB = patientDto.OIB,
                DateOfBirth = patientDto.DateOfBirth,
            };


            _patientRepository.Update(patient);
            _patientRepository.Save();

            return NoContent();
        }

        // DELETE: api/patients/5
        [HttpDelete("{id}")]
        public ActionResult DeletePatient(int id)
        {
            var patient = _patientRepository.GetById(id);

            if (patient == null)
                return NotFound($"Patient with ID {id} not found.");

            _patientRepository.Delete(patient);
            _patientRepository.Save();

            return NoContent();
        }
    }
}
