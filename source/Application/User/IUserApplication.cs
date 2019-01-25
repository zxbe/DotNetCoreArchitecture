using DotNetCore.Objects;
using DotNetCoreArchitecture.Model.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetCoreArchitecture.Application
{
    public interface IUserApplication
    {
        Task<IResult<long>> AddAsync(AddUserModel addUserModel);

        Task<IResult<string>> DeleteAsync(long userId);

        Task<PagedList<UserModel>> ListAsync(PagedListParameters parameters);

        Task<IEnumerable<UserModel>> ListAsync();

        Task<UserModel> SelectAsync(long userId);

        Task<IResult<string>> UpdateAsync(UpdateUserModel updateUserModel);
    }
}
