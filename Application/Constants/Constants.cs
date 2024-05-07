namespace Application.Constants;

public class Constants
{
    public class Roles
    {
        public const string Admin = "Admin";
        public const string Blogger = "Blogger";
    }

    public class Passwords
    {
        public const string AdminPassword = "@ff!N1ty";
        public const string BloggerPassword = "@ff!N1ty";
    }
    
    public class FilePath
    {
        public static string UsersImagesFilePath => @"user-images\";

        public static string BlogsImagesFilePath => @"blog-images\";
    }
}