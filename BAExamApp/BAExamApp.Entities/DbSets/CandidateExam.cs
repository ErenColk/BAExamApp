using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Entities.DbSets;
public class CandidateExam: AuditableEntity
{
    public string Name { get; set; } = null!;
}
