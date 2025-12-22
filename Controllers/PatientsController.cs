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
        public ActionResult<List<Patient>> GetAllPatients()
        {
            var patients = _patientRepository.GetAll();
            return Ok(patients);
        }

        // GET: api/patients/5
        [HttpGet("{id}")]
        public ActionResult<Patient> GetPatient(int id)
        {
            var patient = _patientRepository.GetById(id);

            if (patient == null)
                return NotFound($"Patient with ID {id} not found.");

            return Ok(patient);
        }

        // GET: api/patients/oib/12345678901
        [HttpGet("oib/{oib}")]
        public ActionResult<Patient> GetPatientByOIB(string oib)
        {
            var patient = _patientRepository.GetByOIB(oib);

            if (patient == null)
                return NotFound($"Patient with OIB {oib} not found.");

            return Ok(patient);
        }

        // GET: api/patients/search?lastName=Horvat
        [HttpGet("search")]
        public ActionResult<List<Patient>> SearchPatients([FromQuery] string lastName)
        {
            var patients = _patientRepository.SearchByLastName(lastName);
            return Ok(patients);
        }

        // GET: api/patients/5/details
        [HttpGet("{id}/details")]
        public ActionResult<Patient> GetPatientWithDetails(int id)
        {
            var patient = _patientRepository.GetPatientWithDetails(id);

            if (patient == null)
                return NotFound($"Patient with ID {id} not found.");

            return Ok(patient);
        }

        // POST: api/patients
        [HttpPost]
        public ActionResult<Patient> CreatePatient([FromBody] Patient patient)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validacija OIB-a (mora biti 11 znamenki)
            if (patient.OIB.Length != 11)
                return BadRequest("OIB must be exactly 11 digits.");

            // Provjeri postoji li već pacijent s tim OIB-om
            var existing = _patientRepository.GetByOIB(patient.OIB);
            if (existing != null)
                return Conflict($"Patient with OIB {patient.OIB} already exists.");

            _patientRepository.Add(patient);
            _patientRepository.Save();

            return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
        }

        // PUT: api/patients/5
        [HttpPut("{id}")]
        public ActionResult UpdatePatient(int id, [FromBody] Patient patient)
        {
            if (id != patient.Id)
                return BadRequest("ID mismatch.");

            var existing = _patientRepository.GetById(id);
            if (existing == null)
                return NotFound($"Patient with ID {id} not found.");

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
