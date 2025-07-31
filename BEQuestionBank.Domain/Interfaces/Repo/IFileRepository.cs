using System.Collections.Generic;
using System.Threading.Tasks;
using BEQuestionBank.Domain.Models;
using File = BEQuestionBank.Domain.Models.File;

namespace BEQuestionBank.Domain.Interfaces.Repo
{
    public interface IFileRepository : IRepository<File>
    {
    }
}