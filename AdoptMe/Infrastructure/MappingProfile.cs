namespace AdoptMe.Infrastructure
{
    using AutoMapper;
    using AdoptMe.Models.Pets;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<PetDetailsViewModel, PetFormModel>();
        }
    }
}
