using DotNetCore.Objects;
using DotNetCore.Security;
using DotNetCoreArchitecture.Database;
using DotNetCoreArchitecture.Model.Enums;
using DotNetCoreArchitecture.Model.Models;
using System.Threading.Tasks;

namespace DotNetCoreArchitecture.Domain
{
    public sealed class AuthenticationDomain : IAuthenticationDomain
    {
        public AuthenticationDomain
        (
            IJsonWebToken jsonWebToken,
            IUserLogDomain userLogDomain,
            IUserRepository userRepository
        )
        {
            JsonWebToken = jsonWebToken;
            UserLogDomain = userLogDomain;
            UserRepository = userRepository;
        }

        private IJsonWebToken JsonWebToken { get; }

        private IUserLogDomain UserLogDomain { get; }

        private IUserRepository UserRepository { get; }

        public Task<IResult<string>> SignInAsync(SignInModel signInModel)
        {
            var validation = new SignInModelValidator().Valid(signInModel);

            if (!validation.Success)
            {
                return Task.FromResult(validation);
            }

            var signedInModel = UserRepository.SignInAsync(signInModel).Result;

            validation = new SignedInModelValidator().Valid(signedInModel);

            if (!validation.Success)
            {
                return Task.FromResult(validation);
            }

            var userLogModel = new UserLogModel(signedInModel.UserId, LogType.Login);

            UserLogDomain.AddAsync(userLogModel);

            var jwt = CreateJwt(signedInModel);

            return new SuccessResult<string>(jwt).ToTask();
        }

        public Task SignOutAsync(SignOutModel signOutModel)
        {
            var userLogModel = new UserLogModel(signOutModel.UserId, LogType.Logout);

            return UserLogDomain.AddAsync(userLogModel);
        }

        private string CreateJwt(SignedInModel signedInModel)
        {
            var sub = signedInModel.UserId.ToString();

            var roles = signedInModel.Roles.ToString().Split(", ");

            return JsonWebToken.Encode(sub, roles);
        }
    }
}
