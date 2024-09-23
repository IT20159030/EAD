using System;
using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson.Serialization.Attributes;
namespace Backend.Models;
public class Customer : MongoIdentityUser<Guid>
{
  public string Name { get; set; } = string.Empty;

  public CustomerAccountStatus Status { get; set; } = CustomerAccountStatus.Unapproved;

  [BsonElement("createdAt")]
  // [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
  public DateTime CreatedAt { get; set; }

  [BsonElement("updatedAt")]
  // [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
  public DateTime UpdatedAt { get; set; }
}

