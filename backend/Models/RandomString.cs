
//add a way so every short code will have the same strat
namespace urlshortner.Models
{
    //this class generates a random string using Random()
    public class RandomStringGenerator
    {
        Random Random { set; get; }
        RandomStringGenerator(Random random)
        {
            Random = random;
        }
        /*
        this func generate the Random string  
        the func gets an int from the user "length" = (length of the random string)
        the func return a random string at the wanted length
        */
        public string GenerateRandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string newString = "";

            for (int i = 0; i < length; i++)
            {
                newString += chars[Random.Next(chars.Length)];
            }

            return newString;
        }
    }
}
