
namespace DemoProject.Domain.Models.Users;
public sealed record User(
    Guid Id,
    string FirstName,
    string LastName,
    string Email
);