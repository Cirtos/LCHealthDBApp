using Microsoft.AspNetCore.Mvc;
using Dapper;
using HealthDbApp.Data;
using HealthDbApp.Models;
using System.Data;
using HealthDbApp.Helpers;

namespace HealthDbApp.Controllers{
    [ApiController]
    [Route("[controller]")]
    public class AppointmentController: ControllerBase{
        private readonly DataContextDapper _dapper;
        private readonly ReusableSQL _reusableSQL;

        public AppointmentController(IConfiguration config){
            _dapper = new DataContextDapper(config);
            _reusableSQL = new ReusableSQL(config);
        }

        [HttpGet("GetAppointments/{appointmentId}/{doctorId}/{patientId}")]
        public IEnumerable<Appointment> GetAppointments(int appointmentId = 0, int doctorId = 0, int patientId = 0)
        {
            string sql = "EXEC HealthAppSchema.spAppointments_Get";
            string stringParameters = "";
            DynamicParameters sqlParameters = new DynamicParameters();

            if (appointmentId > 0 ){
                sqlParameters.Add("@AppointmentIdParam", appointmentId, DbType.Int32);
                stringParameters += ", @AppointmentId = @AppointmentIdParam";
            }
            if (doctorId > 0 ){
                sqlParameters.Add("@DoctorIdParam", doctorId, DbType.Int32);
                stringParameters += ", @DoctorId = @DoctorIdParam";
            }
            if (patientId > 0 ){
                sqlParameters.Add("@PatientIdParam", patientId, DbType.Int32);
                stringParameters += ", @PatientId = @PatientIdParam";
            }
            if(stringParameters.Length > 1){
                sql += stringParameters.Substring(1);
            }
            Console.WriteLine(sql);
            IEnumerable<Appointment> appointments = _dapper.LoadDataWithParameters<Appointment>(sql, sqlParameters);
            return appointments;
        }

        [HttpPut("UpsertAppointment")]
        public IActionResult UpsertAppointment(Appointment appointment)
        {
            string sql = @$"EXEC HealthAppSchema.spAppointment_Upsert
                @DoctorId = @DoctorIdParam,
                @PatientId = @PatientIdParam,
                @AppointmentDateTime = @DateParam";
                
            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@DoctorIdParam", appointment.DoctorId, DbType.Int32);
            sqlParameters.Add("@PatientIdParam", appointment.PatientId, DbType.Int32);
            sqlParameters.Add("@DateParam", appointment.AppointmentDateTime, DbType.DateTime2);
            if(_reusableSQL.CheckExists(AppDataTypes.Patient, appointment.PatientId)){
                if(_reusableSQL.CheckExists(AppDataTypes.Doctor, appointment.DoctorId)){
                    if(_dapper.ExecuteSqlWithParameters(sql, sqlParameters)){
                        return Ok();
                    }
                    throw new Exception ("Failed to add Appointment");
                }
                throw new Exception ($"Attempted to add a Appointment with an invalid DoctorID: {appointment.DoctorId}");
            }
            throw new Exception ($"Attempted to add a Appointment with an invalid PatientID: {appointment.PatientId}");
        }

        [HttpDelete("DeleteAppointment/{appointmentId}")]
        public IActionResult DeleteUser(int appointmentId){
            string sql = $"HealthAppSchema.spAppointment_Delete @AppointmentId = @AppointmentIdParam";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@AppointmentIdParam", appointmentId, DbType.Int32);

            if(_dapper.ExecuteSqlWithParameters(sql, sqlParameters)){
                return Ok();
            }

            throw new Exception("Failed to Delete Appointment");
        }

        [HttpPut("BatchUpsertAppointments")]
        public void BatchUpdateAppointments(IEnumerable<Appointment> appointments){
            foreach(Appointment appointment in appointments){
                UpsertAppointment(appointment);
            }
        }
    }
}