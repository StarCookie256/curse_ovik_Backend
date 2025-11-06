using System;
using System.Collections.Generic;

namespace PerfumeryBackend.DatabaseLayer.Models;

public partial class BasketItem
{
    public int Id { get; set; }

    public int? BasketId { get; set; }

    public int? ProductId { get; set; }

    public int? Quantity { get; set; }

    public double? Price { get; set; }

    public int? Volume { get; set; }

    public virtual Basket? Basket { get; set; }

    public virtual Product? Product { get; set; }
}
