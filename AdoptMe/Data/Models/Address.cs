﻿namespace AdoptMe.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Address
    {
        public int Id { get; set; }

        [Required]
        public string StreetName { get; set; }

        public string StreetNumber { get; set; }

        public int CityId { get; set; }

        public City City { get; set; }
    }
}
