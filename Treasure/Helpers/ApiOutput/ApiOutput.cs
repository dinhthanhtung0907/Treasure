using System.Dynamic;

namespace Treasure.Helpers.ApiOutput
{
    public class ApiOutput
    {
        public static dynamic Success(string message)
        {
            dynamic success = new ExpandoObject();
            success.timestamp = ((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds();
            success.message = message;
            return success;
        }

        public static dynamic Success<T>(T data)
        {
            dynamic success = new ExpandoObject();
            success.timestamp = ((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds();
            success.data = data;
            return success;
        }
    }
}
