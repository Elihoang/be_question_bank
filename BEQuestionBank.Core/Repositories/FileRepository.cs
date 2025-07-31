using BEQuestionBank.Core.Configurations;
using BEQuestionBank.Core.Repositories;
using BEQuestionBank.Domain.Interfaces.Repo;
using File = BEQuestionBank.Domain.Models.File;

namespace BEQuestionBank.Infrastructure.Repositories
{
    public class FileRepository : GenericRepository<File>, IFileRepository
    {
        private readonly AppDbContext _context;

        public FileRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

    }
}