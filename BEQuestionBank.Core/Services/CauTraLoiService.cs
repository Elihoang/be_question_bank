using System.Linq.Expressions;
using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Models;

namespace BEQuestionBank.Core.Services;

public class CauTraLoiService : ICauTraLoiService
{

    private readonly ICauTraLoiRepository _repository;

    public CauTraLoiService(ICauTraLoiRepository repository) {
        _repository = repository;
    }

    public async Task AddAsync(CauTraLoi model)
    {
        await _repository.AddAsync(model);
    }

    public async Task DeleteAsync(CauTraLoi model)
    {
        await _repository.DeleteAsync(model);
    }

    public async Task<bool> ExistsAsync(Expression<Func<CauTraLoi, bool>> predicate)
    {
        return await _repository.ExistsAsync(predicate);
    }

    public Task<IEnumerable<CauTraLoi>> FindAsync(Expression<Func<CauTraLoi, bool>> predicate)
    {
        return _repository.FindAsync(predicate);
    }

    public Task<CauTraLoi> FirstOrDefaultAsync(Expression<Func<CauTraLoi, bool>> predicate)
    {
        return _repository.FirstOrDefaultAsync(predicate);
    }

    public Task<IEnumerable<CauTraLoi>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    public async Task<CauTraLoi> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<CauTraLoi>> GetByMaCauHoi(Guid maCauHoi)
    {
        return await _repository.GetByMaCauHoi(maCauHoi);
    }

    public async Task UpdateAsync(CauTraLoi model)
    {
        await _repository.UpdateAsync(model);
    }
    public async Task<bool> CheckSingleCorrectAnswerAsync(Guid maCauHoi)
    {
        var answers = await _repository.GetByMaCauHoi(maCauHoi);
        return answers.Count(a => a.LaDapAn) == 1;
    }
}