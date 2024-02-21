using Microsoft.AspNetCore.Mvc;
using Dapper;
using HealthDbApp.Data;
using HealthDbApp.Models;
using System.Data;
using HealthDbApp.Helpers;

namespace HealthDbApp.Controllers{
    [ApiController]
    [Route("[controller]")]
    public class PatientController: ControllerBase{
        private readonly DataContextDapper _dapper;
        private readonly ReusableSQL _reusableSQL;

        public PatientController(IConfiguration config){
            _dapper = new DataContextDapper(config);
            _reusableSQL = new ReusableSQL(config);
        }

        [HttpGet("GetPatients/{patientId}")]
        public IEnumerable<Patient> GetPatients(int patientId = 0)
        {
            string sql = "EXEC HealthAppSchema.spPatients_Get";
            DynamicParameters sqlParameters = new DynamicParameters();

            if (patientId > 0 ){
                sqlParameters.Add("@PatientIdParam", patientId, DbType.Int32);
                sql += " @PatientId = @PatientIdParam";
            }
            IEnumerable<Patient> patients = _dapper.LoadDataWithParameters<Patient>(sql, sqlParameters);
            return patients;
        }

        [HttpPut("UpsertPatient")]
        public IActionResult UpsertPatient(Patient patient)
        {
            if(_reusableSQL.UpsertPatient(patient)){
                return Ok();
            }

            throw new Exception("Failed to Update Patient");
        }

        [HttpDelete("DeletePatient/{patientId}")]
        public IActionResult DeletePatient(int patientId){
            //Deletes from Patients, Appointments and Prescriptions via SP
            string sql = $"HealthAppSchema.spPatient_Delete @PatientId = @PatientIdParam";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@PatientIdParam", patientId, DbType.Int32);

            if(_dapper.ExecuteSqlWithParameters(sql, sqlParameters)){
                return Ok();
            }

            throw new Exception("Failed to Delete Patient");
        }

        [HttpPut("BatchUpsertPatients")]
        public void BatchUpsertPatients(IEnumerable<Patient> patients){
            foreach(Patient patient in patients){
                UpsertPatient(patient);
            }
        }
    }
}