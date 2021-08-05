namespace AdoptMe.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class City
    {
        public int Id { get; init; }

        [Required]
        public string Name { get; set; }

        public ICollection<Address> Addresses { get; init; } = new HashSet<Address>();
    }
}
