using System.Data;
using NHibernate;
using NHibernate.Dialect;
using NHibernate.Dialect.Function;

namespace Vokabular.ProjectImport.Test.Mock
{
    public class CustomSQLiteDialect : SQLiteDialect
    {
        protected override void RegisterColumnTypes()
        {
            base.RegisterColumnTypes();
            RegisterColumnType(DbType.DateTime2, "DATETIME2");
        }

        protected override void RegisterFunctions()
        {
            base.RegisterFunctions();
            RegisterFunction("current_timestamp", new NoArgSQLFunction("TEXT", NHibernateUtil.DateTime2, true));

        }

        protected override void RegisterKeywords()
        {
            base.RegisterKeywords();
            RegisterKeyword("datetime2");

        }

        protected override void RegisterDefaultProperties()
        {
            base.RegisterDefaultProperties();

        }
    }
}