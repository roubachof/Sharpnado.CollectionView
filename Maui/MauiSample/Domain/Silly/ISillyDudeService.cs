using Sharpnado.CollectionView.Services;

namespace MauiSample.Domain.Silly
{
    public interface ISillyDudeService
    {
        Task<IReadOnlyCollection<SillyDude>> GetSillyPeople();

        Task<PageResult<SillyDude>> GetSillyPeoplePage(int pageNumber, int pageSize);

        Task<SillyDude> GetSilly(int id);

        Task<SillyDude> GetRandomSilly(int waitTime = 2);
    }
}
