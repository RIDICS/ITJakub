using FluentMigrator.Builders.Create.Table;

namespace Vokabular.Database.Migrations.Extensions
{
    public static class MigratorExtensions
    {
        /// <summary>
        /// Extension method for creating text column with maximal size
        /// </summary>
        /// <param name="createTableColumnAsTypeSyntax"></param>
        /// <returns></returns>
        public static ICreateTableColumnOptionOrWithColumnSyntax AsMaxString(this ICreateTableColumnAsTypeSyntax createTableColumnAsTypeSyntax)
        {
            return createTableColumnAsTypeSyntax.AsString(int.MaxValue);
        }
    }
}
