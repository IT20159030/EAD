using System;
using AspNetCore.Identity.MongoDbCore.Models;
namespace Backend.Models;
public class WebApplicationUser : MongoIdentityUser<Guid>
{
  public string Name { get; set; } = string.Empty;
}

