namespace AdoptMe.Data
{
    public class DataConstants
    {
        public class PetRequirements
        {
            public const int NameMinLength = 2;
            public const int NameMaxLength = 20;
            public const int StoryMinLength = 50;
            public const int StoryMaxLength = 5000;
            public const int AgeMinValue = 0;
            public const int AgeMaxValue = 18;
        }

        public class ShelterRequirements
        {
            public const int NameMinLength = 3;
            public const int NameMaxLength = 30;
            public const int PhoneMinLength = 6;
            public const int PhoneMaxLength = 25;
        }

        public class User
        {
            public const int UserNameMinLength = 3;
            public const int UserNameMaxLength = 30;
            public const int UserPasswordMinLength = 6;
            public const int UserPasswordMaxLength = 30;
            public const string UserNameErrorMessage = "The {0} must be at least {2} and at max {1} characters long.";
            public const string UserPasswordErrorMessage = "The {0} must be at least {2} and at max {1} characters long.";
            public const string ConfirmPasswordErrorMessage = "The password and confirmation password do not match.";
        }

        public class Roles
        {
            public const string AdminRoleName = "Administrator";
            public const string ShelterRoleName = "Shelter";
        }

        public class Administrator
        {
            public const string adminEmail = "admin@pets.com";
            public const string adminPassword = "petadmin";
            public const string adminUsername = "admin";
        }
    }
}
