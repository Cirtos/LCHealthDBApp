using Microsoft.AspNetCore.Mvc;
using Dapper;
using HealthDbApp.Data;
using HealthDbApp.Models;
using System.Data;
using HealthDbApp.Helpers;

namespace HealthDbApp.Controllers{
    [ApiController]
    [Route("[controller]")]
    public class DoctorController: ControllerBase{
        private readonly DataContextDapper _dapper;

        public DoctorController(IConfiguration config){
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("GetDoctors/{doctorId}")]
        public IEnumerable<Doctor> GetDoctors(int doctorId = 0)
        {
            string sql = "EXEC HealthAppSchema.spDoctors_Get";
            DynamicParameters sqlParameters = new DynamicParameters();

            if (doctorId > 0 ){
                sqlParameters.Add("@DoctorIdParam", doctorId, DbType.Int32);
                sql += " @DoctorId = @DoctorIdParam";
            }
            IEnumerable<Doctor> Doctors = _dapper.LoadDataWithParameters<Doctor>(sql, sqlParameters);
            return Doctors;
        }

        [HttpPut("UpsertDoctor")]
        public bool UpsertDoctor(Doctor doctor)
        {
            string sql = @$"EXEC HealthAppSchema.spDoctor_Upsert
                @FirstName = @FirstNameParam,
                @LastName = @LastNameParam";
                
            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@FirstNameParam", doctor.FirstName, DbType.String);
            sqlParameters.Add("@LastNameParam", doctor.LastName, DbType.String);

            return _dapper.ExecuteSqlWithParameters(sql, sqlParameters);
        }

        [HttpPut("BatchUpsertDoctors")]
        public void BatchUpsertDoctor(IEnumerable<Doctor> doctors)
        {
            foreach(Doctor doctor in doctors){    
                UpsertDoctor(doctor);
            }
        }
    }
}