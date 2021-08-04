namespace AdoptMe.Infrastructure
{
    using AutoMapper;
    using AdoptMe.Models.Pets;
    using AdoptMe.Data.Models;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<PetDetailsViewModel, PetFormModel>();
        }
    }
}
