using System;
using System.Collections.Generic;

namespace PerfumeryBackend.DatabaseLayer.Models;

public partial class Product
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? BrandId { get; set; }

    public int? CountryId { get; set; }

    public string? Description { get; set; }

    public string? Image { get; set; }

    public string? Gender { get; set; }

    public int? ManufactureYear { get; set; }

    public string? ExpirationDate { get; set; }

    public virtual ICollection<BasketItem> BasketItems { get; set; } = new List<BasketItem>();

    public virtual Brand? Brand { get; set; }

    public virtual Country? Country { get; set; }

    public virtual ICollection<ProductVariation> ProductVariations { get; set; } = new List<ProductVariation>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
