namespace HealthDbApp.Models{
    public partial class Prescription{
        public int PrescriptionId {get; set;}
        public int PatientId {get; set;}
        public int DoctorId {get; set;}
        public string DrugName {get; set;}
        public int Amount{get; set;}

        public Prescription(){
            if(DrugName == null){
                DrugName = "";
            }
        }
    }
}