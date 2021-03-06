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

        public class AdopterRequirements
        {
            public const int FirstLastNameMinLength = 2;
            public const int FirstLastNameMaxLength = 30;
            public const int MinAge = 18;
            public const int MaxAge = 100;
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

        public class AnswerRequirements
        {
            public const int AnswerMinLength = 20;
            public const string AnswerLengthErrorMessage = "Your answer must be atleast {1} characters long";
        }
    }
}