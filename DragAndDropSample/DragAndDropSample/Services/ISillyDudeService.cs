using System.Collections.Generic;
using System.Threading.Tasks;

using Sharpnado.HorizontalListView.Services;

namespace DragAndDropSample.Services
{
    public interface ISillyDudeService
    {
        Task<IReadOnlyCollection<SillyDude>> GetSillyPeople();

        Task<PageResult<SillyDude>> GetSillyPeoplePage(int pageNumber, int pageSize);

        Task<SillyDude> GetSilly(int id);

        Task<SillyDude> GetRandomSilly(int waitTime = 2);
    }
}