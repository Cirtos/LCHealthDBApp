namespace HealthDbApp.Models{
    public partial class Appointment{
        public int AppointmentId {get; set;}
        public int PatientId {get; set;}
        public int DoctorId {get; set;}
        public DateTime AppointmentDateTime {get; set;}

        public Appointment(){
        }
    }
}