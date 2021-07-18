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
    }
}
