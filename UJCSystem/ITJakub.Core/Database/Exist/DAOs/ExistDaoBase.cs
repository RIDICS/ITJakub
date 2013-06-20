using System.Collections.Generic;
using System.Text;
using Castle.MicroKernel;

namespace ITJakub.Core.Database.Exist.DAOs
{
    public abstract class ExistDaoBase
    {
        protected readonly TeiP5Descriptor Descriptor;
        protected readonly ExistDao ExistDao;

        protected ExistDaoBase( IKernel container)
        {
            ExistDao = container.Resolve<ExistDao>();
            Descriptor = container.Resolve<TeiP5Descriptor>();
        }

        protected void AddNamespacesAndCollation(StringBuilder builder)
        {
            builder.AppendLine(string.Format("declare default collation \"{0}\";", Descriptor.GetCollation()));
            foreach (KeyValuePair<string, string> allNamespace in Descriptor.GetAllNamespaces())
            {
                builder.AppendLine(string.Format("declare namespace {0} = \"{1}\";", allNamespace.Key, allNamespace.Value));
            }
        }
    }
}