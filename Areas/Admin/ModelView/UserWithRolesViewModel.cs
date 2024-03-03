namespace Data_Trans_Job.Areas.Admin.ModelView
{
    public class UserWithRolesViewModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<string> Roles { get; set; }
    }
}
