using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Entities;

public partial class Table
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int BookId { get; set; }

    public int ReaderId { get; set; }

    public virtual BookMaster Book { get; set; } = null!;

    public virtual ReaderInfo Reader { get; set; } = null!;
}
public class ENtryDetails
{
    public int BookId { get; set; }

    public int ReaderId { get; set; }


}
