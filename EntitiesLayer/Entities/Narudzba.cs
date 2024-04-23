namespace EntitiesLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Narudzba")]
    public partial class Narudzba
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Narudzba()
        {
            Stavka_narudzbe = new HashSet<Stavka_narudzbe>();
        }

        [Key]
        public int id_narudzba { get; set; }

        public DateTime? datum_vrijeme { get; set; }

        [StringLength(1000)]
        public string racun { get; set; }

        [StringLength(100)]
        public string status { get; set; }

        public int Korisnik_id_korisnik { get; set; }

        public virtual Korisnik Korisnik { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Stavka_narudzbe> Stavka_narudzbe { get; set; }
    }
}
