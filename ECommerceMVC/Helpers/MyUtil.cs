using System.Text;

namespace ECommerceMVC.Helpers
{
    public class MyUtil
    {
        public static string UpLoadHinh(IFormFile Hinh, string folder)
        {
            try
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", folder, Hinh.FileName);
                using (var myfile = new FileStream(fullPath, FileMode.CreateNew))
                {
                    Hinh.CopyTo(myfile);
                }
                return Hinh.FileName;
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }
        public static string GenerateRandomKey(int lenghth = 5)
        {
            var pattern = @"gdjfasdhqywetroiucbvmsbhjgkasdhfAGEUFJAKHDJKHHDLKASHDEBBCNVOPR!@#$$%^&*";
            var sb = new StringBuilder();
            var rd = new Random();
            for (int i = 0; i < lenghth; i++)
            {
                sb.Append(pattern[rd.Next(0, pattern.Length)]);
            }
            return sb.ToString();
        }
    }
}
