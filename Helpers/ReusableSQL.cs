using System.Data;
using Dapper;
using HealthDbApp.Data;
using HealthDbApp.Models;

namespace HealthDbApp.Helpers{
    public class ReusableSQL{
        private readonly DataContextDapper _dapper;
        public ReusableSQL(IConfiguration config){
            _dapper = new DataContextDapper(config);
        }

        public bool UpsertPatient(Patient patient)
        {
            string sql = @$"EXEC HealthAppSchema.spPatient_Upsert
                @FirstName = @FirstNameParam,
                @LastName = @LastNameParam,
                @DateOfBirth = @DateOfBirthParam,
                @Allergies  = @AllergiesParam,
                @Email = @EmailParam,
                @PhoneNumber = @PhoneNumberParam";
                
            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@FirstNameParam", patient.FirstName, DbType.String);
            sqlParameters.Add("@LastNameParam", patient.LastName, DbType.String);
            sqlParameters.Add("@DateOfBirthParam", patient.DateOfBirth, DbType.DateTime2);
            sqlParameters.Add("@AllergiesParam", patient.Allergies, DbType.String);
            sqlParameters.Add("@EmailParam" , patient.Email, DbType.String);
            sqlParameters.Add("@PhoneNumberParam", patient.PhoneNumber, DbType.String);

            return _dapper.ExecuteSqlWithParameters(sql, sqlParameters);
        }

        //I tried doing this accepting a generic and switching on it's type, but couldn't get it to work
        public bool CheckExists(AppDataTypes checkType, int checkId){
            string sql = "";
            switch (checkType){
                case AppDataTypes.Patient:
                    sql = $"SELECT * FROM HealthAppSchema.Patients WHERE PatientId = {checkId}";
                    return _dapper.LoadDataSingle<Patient>(sql) != null;
                case AppDataTypes.Doctor:
                    sql = $"SELECT * FROM HealthAppSchema.Doctors WHERE DoctorId = {checkId}";
                    return _dapper.LoadDataSingle<Doctor>(sql) != null;
                case AppDataTypes.Appointment:
                    sql = $"SELECT * FROM HealthAppSchema.Appointment WHERE AppointmentId = {checkId}";
                    return _dapper.LoadDataSingle<Appointment>(sql) != null;
                case AppDataTypes.Prescription:
                    sql = $"SELECT * FROM HealthAppSchema.Prescription WHERE PrescriptionId = {checkId}";
                    return _dapper.LoadDataSingle<Prescription>(sql) != null;
                default:
                    return false;
            }
        }
    }
}

public enum AppDataTypes{
    Patient,
    Doctor,
    Appointment,
    Prescription
}