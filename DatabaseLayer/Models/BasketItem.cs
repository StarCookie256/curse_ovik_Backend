using System;
using System.Collections.Generic;

namespace PerfumeryBackend.DatabaseLayer.Models;

public partial class BasketItem
{
    public int Id { get; set; }

    public int BasketId { get; set; }

    public int ProductVariationId { get; set; }

    public int? Stock { get; set; }

    public virtual Basket Basket { get; set; } = null!;

    public virtual ProductVariation ProductVariation { get; set; } = null!;
}
