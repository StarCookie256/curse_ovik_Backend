using System;
using System.Collections.Generic;

namespace PerfumeryBackend.DatabaseLayer.Models;

public partial class ProductVariation
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public int? CategoryId { get; set; }

    public double? Price { get; set; }

    public double? Volume { get; set; }

    public int? Stock { get; set; }

    public virtual Category? Category { get; set; }

    public virtual Product? Product { get; set; }
}
