namespace AdoptMe.Infrastructure
{
    using AutoMapper;
    using AdoptMe.Models.Pets;
    using AdoptMe.Data.Models;
    using AdoptMe.Models.Adoptions;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<PetDetailsViewModel, PetFormModel>();
            this.CreateMap<Pet, PetDetailsViewModel>()
                .ForMember(x => x.UserId, cfg => cfg.MapFrom(x => x.Shelter.UserId));
            this.CreateMap<Species, PetSpeciesModel>();
            this.CreateMap<AdoptionApplication, AdoptionDetailsViewModel>()
                .ForMember(x => x.AdopterFullName, cfg => cfg.MapFrom(x => x.Adopter.FirstName + " " + x.Adopter.LastName));
        }
    }
}
