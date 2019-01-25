using DotNetCore.Mapping;
using DotNetCore.Objects;
using DotNetCoreArchitecture.Database;
using DotNetCoreArchitecture.Model.Entities;
using DotNetCoreArchitecture.Model.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetCoreArchitecture.Domain
{
    public sealed class UserDomain : IUserDomain
    {
        public UserDomain
        (
            IDatabaseUnitOfWork databaseUnitOfWork,
            IUserRepository userRepository
        )
        {
            DatabaseUnitOfWork = databaseUnitOfWork;
            UserRepository = userRepository;
        }

        private IDatabaseUnitOfWork DatabaseUnitOfWork { get; }

        private IUserRepository UserRepository { get; }

        public Task<IResult<long>> AddAsync(AddUserModel addUserModel)
        {
            var validationResult = new AddUserModelValidator().Valid(addUserModel);

            if (!validationResult.Success)
            {
                return new ErrorResult<long>(validationResult.Message).ToTask();
            }

            var signInModel = new SignInModel(addUserModel.Login, addUserModel.Password);

            addUserModel.Login = signInModel.LoginHash();

            addUserModel.Password = signInModel.PasswordHash();

            var userEntity = addUserModel.Map<UserEntity>();

            UserRepository.Add(userEntity);

            DatabaseUnitOfWork.SaveChanges();

            return new SuccessResult<long>(userEntity.UserId).ToTask();
        }

        public Task<IResult<string>> DeleteAsync(long userId)
        {
            UserRepository.Delete(userId);

            DatabaseUnitOfWork.SaveChanges();

            return new SuccessResult<string>().ToTask();
        }

        public Task<PagedList<UserModel>> ListAsync(PagedListParameters parameters)
        {
            return UserRepository.ListAsync<UserModel>(parameters);
        }

        public Task<IEnumerable<UserModel>> ListAsync()
        {
            return UserRepository.ListAsync<UserModel>();
        }

        public Task<UserModel> SelectAsync(long userId)
        {
            return UserRepository.SelectAsync<UserModel>(userId);
        }

        public Task<IResult<string>> UpdateAsync(UpdateUserModel updateUserModel)
        {
            var validationResult = new UpdateUserModelValidator().Valid(updateUserModel);

            if (!validationResult.Success)
            {
                return Task.FromResult(validationResult);
            }

            var userEntity = updateUserModel.Map<UserEntity>();

            var userEntityDatabase = UserRepository.Select(userEntity.UserId);

            userEntity.Login = userEntityDatabase.Login;

            userEntity.Password = userEntityDatabase.Password;

            UserRepository.Update(userEntity, userEntity.UserId);

            DatabaseUnitOfWork.SaveChanges();

            return new SuccessResult<string>().ToTask();
        }
    }
}
