using Microsoft.AspNetCore.Mvc;
using Dapper;
using HealthDbApp.Data;
using HealthDbApp.Models;
using System.Data;
using HealthDbApp.Helpers;

namespace HealthDbApp.Controllers{
    [ApiController]
    [Route("[controller]")]
    public class PrescriptionController: ControllerBase{
        private readonly DataContextDapper _dapper;
        private readonly ReusableSQL _reusableSQL;

        public PrescriptionController(IConfiguration config){
            _dapper = new DataContextDapper(config);
            _reusableSQL = new ReusableSQL(config);
        }

        [HttpGet("GetPrescriptions/{prescriptionId}/{doctorId}/{patientId}")]
        public IEnumerable<Prescription> GetPrescriptions(int prescriptionId = 0, int doctorId = 0, int patientId = 0)
        {
            string sql = "EXEC HealthAppSchema.spPrescriptions_Get";
            string stringParameters = "";
            DynamicParameters sqlParameters = new DynamicParameters();

            if (prescriptionId > 0 ){
                sqlParameters.Add("@PrescriptionIdParam", prescriptionId, DbType.Int32);
                stringParameters += ", @PrescriptionId = @PrescriptionIdParam";
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
            IEnumerable<Prescription> Prescriptions = _dapper.LoadDataWithParameters<Prescription>(sql, sqlParameters);
            return Prescriptions;
        }

        [HttpPut("UpsertPrescription")]
        public IActionResult UpsertPrescription(Prescription prescription)
        {
            string sql = @$"EXEC HealthAppSchema.spPrescription_Upsert
                @DoctorId = @DoctorIdParam,
                @PatientId = @PatientIdParam,
                @DrugName = @DrugNameParam,
                @Amount  = @AmountParam";
                
            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@DoctorIdParam", prescription.DoctorId, DbType.Int32);
            sqlParameters.Add("@PatientIdParam", prescription.PatientId, DbType.Int32);
            sqlParameters.Add("@DrugNameParam", prescription.DrugName, DbType.String);
            sqlParameters.Add("@AmountParam", prescription.Amount, DbType.Int32);

            if(_reusableSQL.CheckExists(AppDataTypes.Patient, prescription.PatientId)){
                if(_reusableSQL.CheckExists(AppDataTypes.Doctor, prescription.DoctorId)){
                    if(_dapper.ExecuteSqlWithParameters(sql, sqlParameters)){
                        return Ok();
                    }
                    throw new Exception ("Failed to add Prescription");
                }
                throw new Exception ($"Attempted to add a Prescription with an invalid DoctorID: {prescription.DoctorId}");
            }
            throw new Exception ($"Attempted to add a Prescription with an invalid PatientID: {prescription.PatientId}");
        }

        [HttpDelete("DeletePrescription/{prescriptionId}")]
        public IActionResult DeleteUser(int prescriptionId){
            string sql = $"HealthAppSchema.spPrescription_Delete @PrescriptionId = @PrescriptionIdParam";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@PrescriptionIdParam", prescriptionId, DbType.Int32);

            if(_dapper.ExecuteSqlWithParameters(sql, sqlParameters)){
                return Ok();
            }

            throw new Exception("Failed to Delete Prescription");
        }

        [HttpPut("BatchUpsertPrescriptions")]
        public void BatchUpdatePrescriptions(IEnumerable<Prescription> prescriptions){
            foreach(Prescription prescription in prescriptions){
                UpsertPrescription(prescription);
            }
        }
    }
}