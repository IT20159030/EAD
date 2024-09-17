using System;
using System.ComponentModel.DataAnnotations;


namespace Backend.Dtos;


public class RegisterResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
}