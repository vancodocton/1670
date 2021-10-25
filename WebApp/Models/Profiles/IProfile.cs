namespace WebApp.Models.Profiles
{
    public interface IProfile
    {
        string UserId { get; set; }
        ApplicationUser User { get; set; }
    }
}
