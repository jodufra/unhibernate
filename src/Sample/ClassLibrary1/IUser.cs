using System;

namespace ClassLibrary1
{
    public interface IUser
    {
        int Id { get; set; }
        string Name { get; set; }
        DateTime DateCreated { get; set; }
        DateTime? DateUpdated { get; set; }
    }
}