﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Dtos.CandidateAdmins;
public class CandidateAdminCreateDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
    public bool Gender { get; set; }
    public string? Image { get; set; }
}
