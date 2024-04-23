namespace EntitiesLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Inventar")]
    public partial class Inventar
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Inventar()
        {
            Jelo = new HashSet<Jelo>();
            Pice = new HashSet<Pice>();
        }

        [Key]
        public int id_inventar { get; set; }

        [StringLength(100)]
        public string kolicina_na_zalihi { get; set; }

        [StringLength(100)]
        public string minimalna_kolicina_narudzbe { get; set; }

        public DateTime? datum_nabave { get; set; }

        [StringLength(200)]
        public string dostavljac { get; set; }

        [StringLength(100)]
        public string cijena { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Jelo> Jelo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Pice> Pice { get; set; }
    }
}
