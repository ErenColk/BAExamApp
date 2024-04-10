using BAExamApp.MVC.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BAExamApp.MVC.Areas.CandidateAdmin.Controllers;

[Area("CandidateAdmin")]
[Authorize(Roles = "CandidateAdmin")]
public class CandidateAdminBaseController : BaseController
{
}
