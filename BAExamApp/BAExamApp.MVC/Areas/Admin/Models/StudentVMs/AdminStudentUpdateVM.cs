﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BAExamApp.MVC.Areas.Admin.Models.StudentVMs;

public class AdminStudentUpdateVM
{
    public Guid Id { get; set; }
    [Display(Name = "First_Name")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz.")]
    public string FirstName { get; set; }

    [Display(Name = "Last_Name")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz.")]
    public string LastName { get; set; }
    public string? Image { get; set; }

    [Display(Name = "Profile_Image")]
    public IFormFile? NewImage { get; set; }

    [Display(Name = "Date_Of_Birth")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz.")]
    [DataType(DataType.Date, ErrorMessage = "Lütfen geçerli bir tarih giriniz.")]
    public DateTime DateOfBirth { get; set; }

    [Display(Name = "Gender")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz.")]
    public bool Gender { get; set; }
    [Display(Name = "City")]
    public Guid CityId { get; set; }
    public SelectList? Cities { get; set; }

    [Display(Name = "Email")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz.")]
    public string Email { get; set; }

    [Display(Name = "OtherEmails")]
    public List<string>? OtherEmails { get; set; }
}