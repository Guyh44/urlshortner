

namespace urlshortner.Models
{
    //this class generates a random string using Random()
    public class RandomString
    {
        /*
        this func generate the Random string  
        the func gets an int from the user "length" = (length of the random string)
        the func return a random string at the wanted length
        */
        public static string GenerateRandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            string newString = "";

            for (int i = 0; i < length; i++)
            {
                newString += chars[random.Next(chars.Length)];
            }
            
            return newString;
        }
    }
}
