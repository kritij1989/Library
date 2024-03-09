using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entities;

public partial class ReaderInfo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string ReaderName { get; set; } = null!;

    public string ReaderAddress { get; set; } = null!;

    public virtual ICollection<Table> Tables { get; set; } = new List<Table>();
}
