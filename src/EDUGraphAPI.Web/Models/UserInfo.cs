namespace EDUGraphAPI.Web.Models
{
    public class UserInfo
    {
        public string Id { get; set; }

        public string GivenName { get; set; }

        public string Surname { get; set; }

        public string Mail { get; set; }

        public string UserPrincipalName { get; set; }

        public string[] Roles { get; set; }
    }
}