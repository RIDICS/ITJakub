namespace ITJakub.MobileApps.Client.Core.Manager.Converter
{
    public static class UserRoleConverter
    {
        public static UserRole ConvertToLocal(Service.UserRole serviceUserRole)
        {
            switch (serviceUserRole)
            {
                case Service.UserRole.Student:
                    return UserRole.Student;
                case Service.UserRole.Teacher:
                    return UserRole.Teacher;
                default:
                    return UserRole.Student;
            }
        }
    }
}