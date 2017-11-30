using System;

namespace Vokabular.MainService.Core.Utils
{
    public static class PagingHelper
    {
        public static int GetStart(int? start)
        {
            return start ?? DefaultValues.Start;
        }

        public static int GetCount(int? count)
        {
            return count != null
                ? Math.Min(count.Value, DefaultValues.MaxCount)
                : DefaultValues.Count;
        }

        public static int GetCountForProject(int? count)
        {
            return count != null
                ? Math.Min(count.Value, DefaultValues.MaxProjectCount)
                : DefaultValues.ProjectCount;
        }
    }
}