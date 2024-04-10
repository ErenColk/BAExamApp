using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Entities.Enums;
public enum CandidateQuestionType
{
    [Display(Name = "Test")]
    Test = 1,
    [Display(Name = "Algorithm")]
    Algorithm = 2,
    [Display(Name = "Text_Answered_Question")]
    Classic = 3,
}
