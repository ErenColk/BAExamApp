﻿namespace BAExamApp.Dtos.Admins;

public class AdminDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
    public bool Gender { get; set; }
    public string? Image { get; set; }
    public Guid CityId { get; set; }
    public string IdentityId { get; set; }
}