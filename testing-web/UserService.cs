namespace testing_web;

public class UserService(IUserRepository userRepository)
{
    public readonly IUserRepository _userRepository = userRepository;
    public bool SaveUser(string userName, string pass)
    {
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(pass))
            return false;

        return _userRepository.SaveUserDetail(userName, pass);
    }

    public async Task<User> GetUserById(int id)
    {
        if (id < 0)
            return new();

        return await _userRepository.GetUserByid(id);
    }
}
