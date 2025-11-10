using PerfumeryBackend.ApplicationLayer.Entities;
using System;
using System.Collections.Generic;

namespace PerfumeryBackend.DatabaseLayer.Models;

public partial class Customer
{
    public int Id { get; set; }

    public string? Image { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string Password { get; set; } = null!;
    public RefreshToken RefreshToken { get; set; } = null!;

    //public string RefreshTokenToken { get; set; } = null!;

    //public string RefreshTokenExpires { get; set; } = null!;

    public virtual ICollection<Basket> Baskets { get; set; } = new List<Basket>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
