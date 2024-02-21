namespace HealthDbApp.Models{
    public partial class Patient{
        public int PatientId {get; set;}
        public string FirstName {get; set;}
        public string LastName {get; set;}
        public DateTime DateOfBirth{get; set;}
        public string? Allergies {get; set;}
        public string Email {get; set;}
        public string PhoneNumber {get; set;}

        public Patient(){
            if(FirstName == null){
                FirstName = "";
            }
            if(LastName == null){
                LastName = "";
            }
            if(Allergies == null){
                Allergies = "";
            }
            if(Email == null){
                Email = "";
            }
            if(PhoneNumber == null){
                PhoneNumber = "";
            }
        }
    }
}