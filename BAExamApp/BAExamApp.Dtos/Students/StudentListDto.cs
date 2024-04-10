﻿namespace BAExamApp.Dtos.Students;

public class StudentListDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string? Image { get; set; }
    public string PhoneNumber { get; set; }
}