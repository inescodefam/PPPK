using MedApi.DTOs;
using MedCore.Entities;
using MedCore.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorRepository _doctorRepository;

        public DoctorsController(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        // GET: api/doctors
        [HttpGet]
        public ActionResult<List<DoctorDto>> GetAllDoctors()
        {
            var doctors = _doctorRepository.GetAll();
            return Ok(doctors);
        }

        // GET: api/doctors/5
        [HttpGet("{id}")]
        public ActionResult<DoctorDto> GetDoctor(int id)
        {
            var doctor = _doctorRepository.GetById(id);

            if (doctor == null)
                return NotFound($"Doctor with ID {id} not found.");

            return Ok(doctor);
        }

        // POST: api/doctors
        [HttpPost]
        public ActionResult<DoctorDto> CreateDoctor([FromBody] DoctorBaseDto doctorDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Doctor doctor = new Doctor
            {
                FirstName = doctorDto.FirstName,
                LastName = doctorDto.LastName,
                Specialization = doctorDto.Specialization
            };

            _doctorRepository.Add(doctor);
            _doctorRepository.Save();

            return CreatedAtAction(nameof(GetDoctor), new { id = doctor.Id }, doctor);
        }

        // PUT: api/doctors/5
        [HttpPut("{id}")]
        public ActionResult UpdateDoctor(int id, [FromBody] DoctorBaseDto doctorBase)
        {
            var existing = _doctorRepository.GetById(id);
            if (existing == null)
                return NotFound($"Doctor with ID {id} not found.");

            var doctor = new Doctor
            {
                Id = id,
                FirstName = doctorBase.FirstName,
                LastName = doctorBase.LastName,
                Specialization = doctorBase.Specialization
            };

            _doctorRepository.Update(doctor);
            _doctorRepository.Save();

            return NoContent();
        }

        // DELETE: api/doctors/5
        [HttpDelete("{id}")]
        public ActionResult DeleteDoctor(int id)
        {
            var doctor = _doctorRepository.GetById(id);

            if (doctor == null)
                return NotFound($"Doctor with ID {id} not found.");

            _doctorRepository.Delete(doctor);
            _doctorRepository.Save();

            return NoContent();
        }
    }
}
