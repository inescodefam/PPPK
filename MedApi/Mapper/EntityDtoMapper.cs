using AutoMapper;
using MedApi.DTOs;
using MedCore.Entities;

namespace MedApi.Mapper
{
    public class EntityDtoMapper : Profile
    {
        public EntityDtoMapper()
        {
            CreateMap<Patient, PatientDto>();
            CreateMap<PatientDto, Patient>();

            CreateMap<Doctor, DoctorDto>();
            CreateMap<DoctorDto, Doctor>();

            CreateMap<Examination, ExaminationDto>();
            CreateMap<ExaminationDto, Examination>();


            CreateMap<Therapy, TherapyDto>();
            CreateMap<TherapyDto, Therapy>();

            CreateMap<Prescription, PrescriptionDto>();
            CreateMap<PrescriptionDto, Prescription>();

            CreateMap<Medication, MedicationDto>();
            CreateMap<MedicationDto, Medication>();

            CreateMap<MedicalHistory, MedicalHistoryDto>();
            CreateMap<MedicalHistoryDto, MedicalHistory>();




        }

    }
}
