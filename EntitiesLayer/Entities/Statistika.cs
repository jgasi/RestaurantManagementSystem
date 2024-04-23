namespace EntitiesLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Statistika")]
    public partial class Statistika
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Statistika()
        {
            Stavka_narudzbe = new HashSet<Stavka_narudzbe>();
        }

        [Key]
        public int id_statistika { get; set; }

        public DateTime? datum_vrijeme { get; set; }

        [StringLength(200)]
        public string broj_prodanih_jedinica { get; set; }

        [StringLength(200)]
        public string ukupni_prihod { get; set; }

        [StringLength(50)]
        public string prosjecna_ocjena { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Stavka_narudzbe> Stavka_narudzbe { get; set; }
    }
}
