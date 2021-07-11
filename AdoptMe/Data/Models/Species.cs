namespace AdoptMe.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Species 
    {
        public int Id { get; init; }

        [Required]
        public string Name { get; set; }

        public IEnumerable<Pet> Pets { get; init; } = new List<Pet>();
    }
}
