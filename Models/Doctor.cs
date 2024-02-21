namespace HealthDbApp.Models{
    public partial class Doctor{
        public int DoctorId {get; set;}
        public string FirstName {get; set;}
        public string LastName {get; set;}

        public Doctor(){
            if(FirstName == null){
                FirstName = "";
            } 
            if(LastName == null){
                LastName = "";
            }
        }
    }
}