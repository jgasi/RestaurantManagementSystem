namespace EntitiesLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Rezervacija")]
    public partial class Rezervacija
    {
        [Key]
        public int id_rezervacija { get; set; }

        public DateTime? datum_vrijeme { get; set; }

        public int Korisnik_id_korisnik { get; set; }

        public int Stol_id_stol { get; set; }

        public virtual Korisnik Korisnik { get; set; }

        public virtual Stol Stol { get; set; }
    }
}
