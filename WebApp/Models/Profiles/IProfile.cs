namespace WebApp.Models.Profiles
{
    public interface IProfile
    {
        string Id { get; set; }

        string FullName { get; set; }

        string Email { get; set; }

        int Age { get; set; }

        string Address { get; set; }
    }
}
