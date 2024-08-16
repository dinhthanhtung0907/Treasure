using Treasure.Models.Treasuare;

namespace Treasure.Services
{
    public interface ITreasureService
    {
        Task<double> FindMinimumFuel(MatrixRequest request);
    }
}
