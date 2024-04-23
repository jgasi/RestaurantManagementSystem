namespace EntitiesLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Recenzija")]
    public partial class Recenzija
    {
        [Key]
        public int id_recenzija { get; set; }

        [StringLength(50)]
        public string ocjena { get; set; }

        [StringLength(1000)]
        public string komentar { get; set; }

        public int Korisnik_id_korisnik { get; set; }

        public int? Jelo_id_jelo { get; set; }

        public int? Pice_id_pice { get; set; }

        public virtual Jelo Jelo { get; set; }

        public virtual Korisnik Korisnik { get; set; }

        public virtual Pice Pice { get; set; }
    }
}
