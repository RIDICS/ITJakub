using System;
using System.Collections;

namespace Vokabular.RestClient
{
    public abstract class BuilderBase<T>
    {
        protected abstract void AppendParameter(string name, string value);

        public virtual BuilderBase<T> AddParameter(string name, string value)
        {
            if (value == null)
                return this;

            AppendParameter(name, value);
            return this;
        }

        public virtual BuilderBase<T> AddParameter(string name, Enum value)
        {
            if (value == null)
                return this;

            AppendParameter(name, value.ToString());
            return this;
        }

        public virtual BuilderBase<T> AddParameterList(string name, IEnumerable list)
        {
            if (list == null)
                return this;

            foreach (var item in list)
            {
                AppendParameter(name, item.ToString());
            }
            return this;
        }

        public virtual BuilderBase<T> AddParameter(string name, object value)
        {
            if (value == null)
                return this;

            AddParameter(name, value.ToString());
            return this;
        }

        public virtual BuilderBase<T> AddParameter(string name, long value)
        {
            AppendParameter(name, value.ToString());
            return this;
        }

        public abstract T ToResult();
    }
}