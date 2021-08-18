namespace AdoptMe.Common
{
    public class GlobalConstants
    {
        public class Roles
        {
            public const string AdminRoleName = "Administrator";
            public const string ShelterRoleName = "Shelter";
            public const string AdopterRoleName = "Adopter";
        }

        public class Administrator
        {
            public const string AdminEmail = "admin@pets.com";
            public const string AdminPassword = "petadmin";
            public const string AdminUsername = "admin";
            public const string AdminAreaName = "Administration";
        }

        public class PageSizes
        {
            public const int AllPetsPageSize = 3;
            public const int AdminPanelPagesSize = 5;
            public const int MyPetsPageSize = 5;
            public const int AdoptionApplicationsPageSize = 10;
        }

        public class AdoptionApplicationQuestions
        {
            public const string FirstQuestion = "Why do you want to adopt a pet?";
            public const string SecondQuestion = "Do you live in a house with yard or in flat?";
            public const string ThirdQuestion = "Have you had a pet before?";
            public const string FourthQuestion = "Do you currently have any other pets?";
        }

        public class ShelterSeeder
        {
            public const string Email = "shelter@shelter.com";
            public const string Username = "shelter";
        }

        public class AdopterSeeder
        {
            public const string Email = "adopter@adopter.com";
            public const string Username = "adopter";
            public const string Email2 = "adopter2@adopter.com";
            public const string Username2 = "adopter2";
            public const string FirstName = "Jack";
            public const string LastName = "Sparrow";
            public const int Age = 35;
            public const string FirstName2 = "Tony";
            public const string LastName2 = "Stark";
            public const int Age2 = 30;
        }

        public class AddressSeeder
        {
            public const string CityName = "Plovdiv";
            public const string StreetName = "bul. 6-ti Septemvri";
            public const string StreetNumber = "155A";
        }
    }
}
