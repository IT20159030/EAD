using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson.Serialization.Attributes;
namespace Backend.Models;
public class User : MongoIdentityUser<Guid>
{
  public string Name { get; set; } = string.Empty;

  public AccountStatus Status { get; set; } = AccountStatus.Unapproved;

  [BsonElement("createdAt")]
  public DateTime CreatedAt { get; set; }

  [BsonElement("updatedAt")]
  public DateTime UpdatedAt { get; set; }
}

