namespace EntitiesLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Stavka_narudzbe
    {
        [Key]
        public int id_stavka_narudzbe { get; set; }

        [StringLength(100)]
        public string kolicina { get; set; }

        [StringLength(1000)]
        public string prilagodbe { get; set; }

        public int Narudzba_id_narudzba { get; set; }

        public int? Statistika_id_statistika { get; set; }

        public int? Jelo_id_jelo { get; set; }

        public int? Pice_id_pice { get; set; }

        public virtual Jelo Jelo { get; set; }

        public virtual Narudzba Narudzba { get; set; }

        public virtual Pice Pice { get; set; }

        public virtual Statistika Statistika { get; set; }
    }
}
