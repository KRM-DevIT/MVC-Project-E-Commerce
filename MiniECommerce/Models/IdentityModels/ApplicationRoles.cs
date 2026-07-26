namespace MiniECommerce.Models.IdentityModels
{
    public static class ApplicationRoles
    {
        public const string Admin = "Admin";
        public const string Customer = "Customer";
        public const string DemoAdmin = "DemoAdmin";
        public const string AdminOrDemoAdmin = Admin + "," + DemoAdmin;

        public const string DemoAdminEmail = "demo.admin@shophub.com";
        public const string DemoAdminPassword = "DemoAdmin123!";
    }
}
