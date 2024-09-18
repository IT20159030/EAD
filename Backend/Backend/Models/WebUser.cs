using System;
using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson.Serialization.Attributes;
namespace Backend.Models;
public class WebUser : MongoIdentityUser<Guid>
{
  public string Name { get; set; } = string.Empty;

  [BsonElement("createdAt")]
  public DateTime CreatedAt { get; set; }

  [BsonElement("updatedAt")]
  public DateTime UpdatedAt { get; set; }
}

