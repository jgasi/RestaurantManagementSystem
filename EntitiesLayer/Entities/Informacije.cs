namespace EntitiesLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Informacije")]
    public partial class Informacije
    {
        [Key]
        public int id_informacije { get; set; }

        [StringLength(100)]
        public string naziv { get; set; }

        [StringLength(200)]
        public string adresa { get; set; }

        [StringLength(100)]
        public string grad { get; set; }

        [StringLength(200)]
        public string radno_vrijeme { get; set; }

        [StringLength(100)]
        public string kontakt_telefon { get; set; }

        [StringLength(200)]
        public string email { get; set; }

        public byte[] slika { get; set; }
    }
}
