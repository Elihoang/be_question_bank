using BEQuestionBank.Core.Configurations;
using BEQuestionBank.Core.Repositories;
using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Infrastructure.Repositories;

namespace BEQuestionBank.Application.Services;

    public class CauTraLoiRepository : GenericRepository<CauTraLoi>, ICauTraLoiRepository
    {
        private readonly AppDbContext _context;

        public CauTraLoiRepository(AppDbContext context) 
            : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }
