/**
* UserController.cs
* This file contains the controller for the User model.
* It contains the CRUD operations for the User model.
*/

using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend.Controllers;


[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
  private readonly ILogger<UserController> _logger;
  private readonly IMongoCollection<User> _users;
  public UserController(ILogger<UserController> logger, MongoDBService mongoDbService)
  {
    _logger = logger;
    _users = mongoDbService.Database?.GetCollection<User>("TEST_USERS") ?? throw new InvalidOperationException("Cannot read MongoDB connection settings");
  }

  [HttpGet(Name = "GetUsers")]
  public async Task<IEnumerable<User>> Get()
  {
    _logger.LogInformation("Getting users");
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