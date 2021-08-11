namespace AdoptMe.Common
{
    public class GlobalConstants
    {
        public class Roles
        {
            public const string AdminRoleName = "Administrator";
            public const string ShelterRoleName = "Shelter";
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
    }
}
