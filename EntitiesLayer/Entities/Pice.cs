namespace EntitiesLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Pice")]
    public partial class Pice
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Pice()
        {
            Recenzija = new HashSet<Recenzija>();
            Stavka_narudzbe = new HashSet<Stavka_narudzbe>();
        }

        [Key]
        public int id_pice { get; set; }

        [StringLength(100)]
        public string naziv { get; set; }

        [StringLength(100)]
        public string cijena { get; set; }

        [StringLength(1000)]
        public string nutrivne_informacije { get; set; }

        [StringLength(1000)]
        public string alergeni { get; set; }

        public byte[] slika { get; set; }

        public int Inventar_id_inventar { get; set; }

        public virtual Inventar Inventar { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Recenzija> Recenzija { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Stavka_narudzbe> Stavka_narudzbe { get; set; }
    }
}
