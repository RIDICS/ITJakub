using System.Net;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts;

namespace Vokabular.MainService.Core.Utils
{
    public class OwnershipHelper
    {
        public static void CheckItemOwnership(long itemOwnerUserId, int userId)
        {
            if (userId != itemOwnerUserId)
            {
                throw new MainServiceException(
                    MainServiceErrorCode.UserHasNotPermissionToManipulate,
                    $"Current user ID=({userId}) doesn't have permission manipulate with specified item owned by user with ID={itemOwnerUserId}",
                    HttpStatusCode.Forbidden
                );
            }
        }

        private void CheckItemOwnership(User itemOwnerUser, User user)
        {
            if (user.Id != itemOwnerUser.Id)
            {
                throw new MainServiceException(
                    MainServiceErrorCode.UserHasNotPermissionToManipulate,
                    $"Current user with id '{user.Id}' (external id '{user.ExternalId}') doesn't have permission manipulate with specified item owned by user with ID={itemOwnerUser.Id}",
                    HttpStatusCode.Forbidden
                );
            }
        }
    }
}