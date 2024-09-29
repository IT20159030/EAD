using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace Backend.Models;

[CollectionName("roles")]
public class ApplicationRole : MongoIdentityRole<Guid>
{
}