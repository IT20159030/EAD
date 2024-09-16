using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Backend.Models;
public class User
{
  [BsonId]
  [BsonRepresentation(BsonType.ObjectId)]
  public string? Id { get; set; }

  [BsonElement("name")]
  public required string Name { get; set; }

  [BsonElement("email")]
  public required string Email { get; set; }

  [BsonElement("password")]
  public required string Password { get; set; }

  [BsonElement("role")]
  public required string Role { get; set; }

  [BsonElement("status")]
  public required string Status { get; set; }

}