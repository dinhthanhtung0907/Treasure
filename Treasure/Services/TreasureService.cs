using AutoMapper;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Text.Json;
using Treasure.Data;
using Treasure.Data.Entitites;
using Treasure.Services;

namespace Treasure.Models.Treasuare
{
    public class TreasureService : ITreasureService 
    {
        private  AppDBContext _context;
        private readonly IDistributedCache _cache;
        private readonly IMapper _mapper;

        public TreasureService(AppDBContext context, IDistributedCache cache, IMapper mapper)
        {
            _context = context;
            _cache = cache;
            _mapper = mapper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<double> FindMinimumFuel(MatrixRequest request)
        {
            try
            {
                //check if have requestId
                if (request.Id != null && request.Id != 0)
                {
                    string redisKey = $"fuel_{request.Id}";
                    string redisValue = await _cache.GetStringAsync(redisKey);
                    if (!string.IsNullOrEmpty(redisValue))
                    {
                        var redisValueObject = JsonConvert.DeserializeObject<MatrixRequest>(redisValue);
                        if (redisValueObject == null)
                        {
                            Console.WriteLine($"[ERROR] - Invalid value of REDIS Key: {redisKey}");
                            throw new Exception($"[ERROR] - Invalid value of REDIS Key: {redisKey}");
                        }
                        request.Matrix = redisValueObject.Matrix;
                        request.N = redisValueObject.N;
                        request.M = redisValueObject.M;
                        request.P = redisValueObject.P;
                    }
                    else
                    {
                        var requestDB = _context.MatrixInputHistories.Find(request.Id);
                        if (requestDB == null)
                        {
                            Console.WriteLine($"[ERROR] - requestId not exist");
                            throw new Exception("[ERROR] - requestId not exist");
                        }
                        // add data to redis
                        await _cache.SetStringAsync(redisKey, JsonConvert.SerializeObject(request));
                        request = _mapper.Map(requestDB, request);
                        request.Matrix = JsonConvert.DeserializeObject<int[][]>(requestDB.DataMatrix);
                    }
                }
                else
                {
                    //save db and add to redis data
                    var treasureRequest = new MatrixInputHistory();
                    treasureRequest = _mapper.Map<MatrixInputHistory>(request);
                    treasureRequest.DataMatrix = JsonConvert.SerializeObject(request.Matrix);
                    _context.MatrixInputHistories.Add(treasureRequest);
                    var numRow =  _context.SaveChanges();
                    if (numRow > 0)
                    {
                        string redisKey = $"fuel_{treasureRequest.Id}";
                        request.Id = treasureRequest.Id;
                        await _cache.SetStringAsync(redisKey, JsonConvert.SerializeObject(request));
                    }
                }


                int n = request.N;
                int m = request.M;
                int p = request.P;
                int[][] matrix = request.Matrix;

                // save location for each chest
                var positions = new Dictionary<int, List<(int x, int y)>>();

                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < m; j++)
                    {
                        int key = matrix[i][j];
                        if (!positions.ContainsKey(key))
                        {
                            positions[key] = new List<(int x, int y)>();
                        }
                        positions[key].Add((i, j));
                    }
                }
      
                var dp = new Dictionary<(int x, int y), double>();
                dp[(0, 0)] = 0;

                // caculate dp for each key from 1 to p
                for (int k = 1; k <= p; k++)
                {
                    var newDp = new Dictionary<(int x, int y), double>();

                    foreach (var (prevX, prevY) in dp.Keys)
                    {
                        double prevFuel = dp[(prevX, prevY)];

                        foreach (var (nextX, nextY) in positions[k])
                        {
                            double distance = Math.Sqrt(Math.Pow(prevX - nextX, 2) + Math.Pow(prevY - nextY, 2));
                            double newFuel = prevFuel + distance;

                            if (!newDp.ContainsKey((nextX, nextY)) || newFuel < newDp[(nextX, nextY)])
                            {
                                newDp[(nextX, nextY)] = newFuel;
                            }
                        }
                    }

                    dp = newDp; // update dp with new value for k's key 
                }

                // find min value in db for the last chest(p)
                double minFuel = double.MaxValue;
                foreach (var (finalX, finalY) in positions[p])
                {
                    if (dp.ContainsKey((finalX, finalY)))
                    {
                        minFuel = Math.Min(minFuel, dp[(finalX, finalY)]);
                    }
                }
                return minFuel;
            }
            catch (Exception e)
            {
                Console.WriteLine($"TreasureService:  FindMinimumFuel: Error {e.Message}");
                throw new Exception($"TreasureService:  FindMinimumFuel: Error {e.Message}");
            }
        }
    }

}
