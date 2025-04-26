using System;
using DatingModels.Entities;
using Microsoft.EntityFrameworkCore;

namespace DatingDAL;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<AppUser> Users { get; set; }
}
