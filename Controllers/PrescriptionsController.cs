using MedApi.DTOs;
using MedCore.Entities;
using MedCore.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrescriptionsController : ControllerBase
    {
        private readonly IPrescriptionRepository _prescriptionRepository;

        public PrescriptionsController(IPrescriptionRepository prescriptionRepository)
        {
            _prescriptionRepository = prescriptionRepository;
        }

        // GET: api/prescriptions
        [HttpGet]
        public ActionResult<List<PrescriptionDto>> GetAllPrescriptions()
        {
            var prescriptions = _prescriptionRepository.GetAll();
            return Ok(prescriptions);
        }

        // GET: api/prescriptions/5
        [HttpGet("{id}")]
        public ActionResult<PrescriptionDto> GetPrescription(int id)
        {
            var prescription = _prescriptionRepository.GetById(id);

            if (prescription == null)
                return NotFound($"Prescription with ID {id} not found.");

            return Ok(prescription);
        }

        // GET: api/prescriptions/therapy/5
        [HttpGet("therapy/{therapyId}")]
        public ActionResult<List<PrescriptionDto>> GetByTherapy(int therapyId)
        {
            var prescriptions = _prescriptionRepository.GetByTherapyId(therapyId);
            return Ok(prescriptions);
        }

        // GET: api/prescriptions/medication/5
        [HttpGet("medication/{medicationId}")]
        public ActionResult<List<PrescriptionDto>> GetByMedication(int medicationId)
        {
            var prescriptions = _prescriptionRepository.GetByMedicationId(medicationId);
            return Ok(prescriptions);
        }

        // GET: api/prescriptions/doctor/5
        [HttpGet("doctor/{doctorId}")]
        public ActionResult<List<PrescriptionDto>> GetByDoctor(int doctorId)
        {
            var prescriptions = _prescriptionRepository.GetByDoctorId(doctorId);
            return Ok(prescriptions);
        }

        // GET: api/prescriptions/5/details
        [HttpGet("{id}/details")]
        public ActionResult<PrescriptionDto> GetPrescriptionWithDetails(int id)
        {
            var prescription = _prescriptionRepository.GetPrescriptionWithDetails(id);

            if (prescription == null)
                return NotFound($"Prescription with ID {id} not found.");

            return Ok(prescription);
        }

        // POST: api/prescriptions
        [HttpPost]
        public ActionResult<PrescriptionDto> CreatePrescription([FromBody] PrescriptionDto prescriptionDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Prescription prescription = new Prescription
            {
                TherapyId = prescriptionDto.TherapyId,
                MedicationId = prescriptionDto.MedicationId,
                DoctorId = prescriptionDto.DoctorId,
                Dosage = prescriptionDto.Dosage,
                Frequency = prescriptionDto.Frequency,
                PrescribedDate = prescriptionDto.PrescribedDate,
                EndDate = prescriptionDto.EndDate,
                Instructions = prescriptionDto.Instructions
            };

            try
            {
                _prescriptionRepository.Add(prescription);
                _prescriptionRepository.Save();
            }
            catch (Exception e)
            {
                return StatusCode(500, "An error occurred while creating the prescription.\n " + e.Message);
            }

            return CreatedAtAction(nameof(GetPrescription), new { id = prescription.Id }, prescription);
        }

        // PUT: api/prescriptions/5
        [HttpPut("{id}")]
        public ActionResult UpdatePrescription(int id, [FromBody] PrescriptionDto prescriptionDto)
        {
            var existing = _prescriptionRepository.GetById(id);
            if (existing == null)
                return NotFound($"Prescription with ID {id} not found.");

            Prescription prescription = new Prescription
            {
                Id = id,
                TherapyId = prescriptionDto.TherapyId,
                MedicationId = prescriptionDto.MedicationId,
                DoctorId = prescriptionDto.DoctorId,
                Dosage = prescriptionDto.Dosage,
                Frequency = prescriptionDto.Frequency,
                PrescribedDate = prescriptionDto.PrescribedDate,
                EndDate = prescriptionDto.EndDate,
                Instructions = prescriptionDto.Instructions
            };

            _prescriptionRepository.Update(prescription);
            _prescriptionRepository.Save();

            return NoContent();
        }

        // DELETE: api/prescriptions/5
        [HttpDelete("{id}")]
        public ActionResult DeletePrescription(int id)
        {
            var prescription = _prescriptionRepository.GetById(id);

            if (prescription == null)
                return NotFound($"Prescription with ID {id} not found.");

            _prescriptionRepository.Delete(prescription);
            _prescriptionRepository.Save();

            return NoContent();
        }
    }
}
