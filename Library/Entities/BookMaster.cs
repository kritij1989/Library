using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entities;

public partial class BookMaster
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
   
    public string BookName { get; set; } = null!;

    [Required]
    public string Authorname { get; set; } = null!;

    [Required]
    public int Quantity { get; set; }

   
    public int RemainingQty { get; set; }
   
    public virtual ICollection<Table> Tables { get; set; } = new List<Table>();
    
}
