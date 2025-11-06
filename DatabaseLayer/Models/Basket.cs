using System;
using System.Collections.Generic;

namespace PerfumeryBackend.DatabaseLayer.Models;

public partial class Basket
{
    public int Id { get; set; }

    public int? CustomerId { get; set; }

    public string? Date { get; set; }

    public string? Status { get; set; }

    public double? TotalPrice { get; set; }

    public virtual ICollection<BasketItem> BasketItems { get; set; } = new List<BasketItem>();

    public virtual Customer? Customer { get; set; }
}
