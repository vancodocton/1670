using System;

namespace WebApp.Models.Profiles
{
    public interface IProfileTrainee : IProfile
    {
        DateTime? BirthDate { get; set; }

        string Education { get; set; }
    }
}
