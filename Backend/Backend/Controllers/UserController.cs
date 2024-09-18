/**
* UserController.cs
* This file contains the controller for the User model.
* It contains the CRUD operations for the User model.
*/

using Backend.Models;
using Backend.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend.Controllers;

[Authorize(Roles = "admin")]
[ApiController]
[Route("api/v1/[controller]")]
public class UserController : ControllerBase
{
  private readonly ILogger<UserController> _logger;
  private readonly IMongoCollection<User> _users;
  public UserController(ILogger<UserController> logger, MongoDBService mongoDBService)
  {
    _logger = logger;
    _users = mongoDBService.Database.GetCollection<User>("TEST_USERS");
  }

  [HttpGet(Name = "GetUsers")]
  public async Task<IEnumerable<User>> Get()
  {
    return await _users.Find(new BsonDocument()).ToListAsync();
  }

  [HttpPost(Name = "CreateUser")]
  public async Task<IActionResult> Post([FromBody] User user)
  {



    await _users.InsertOneAsync(user);
    return CreatedAtAction(nameof(Get), new { id = user.Id }, user);


  }

  [HttpPut(Name = "UpdateUser")]
  public async Task<IActionResult> Put(String id, [FromBody] User user)
  {
    await _users.ReplaceOneAsync(u => u.Id == id, user);
    return NoContent();
  }

  [HttpDelete(Name = "DeleteUser")]
  public async Task<IActionResult> Delete(String id)
  {
    await _users.DeleteOneAsync(u => u.Id == id);
    return NoContent();
  }
}