using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ITJakub.Contracts.Searching;
using log4net;

namespace ITJakub.Core.Searching.CriteriumCompilers
{
    public class CriteriumCompilerDirector
    {
        private readonly Dictionary<Type, CriteriumCompilerBase> m_compilers;

        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public CriteriumCompilerDirector(string[] assemblies)
        {
            m_compilers = new Dictionary<Type, CriteriumCompilerBase>();
            InitializeCompilers(assemblies);
        }

        private void InitializeCompilers(IEnumerable<string> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                var ass = Assembly.Load(assembly);

                foreach (Type type in ass.GetTypes())
                {
                    var cca =
                        type.GetCustomAttributes(typeof (CriteriumCompilerAttribute), false).FirstOrDefault() as
                        CriteriumCompilerAttribute;
                    if (cca != null)
                    {
                        var compiler = Activator.CreateInstance(type) as CriteriumCompilerBase;
                        if (compiler != null)
                            m_compilers.Add(cca.CompilerFor, compiler);
                        else
                            throw new InvalidOperationException(string.Format("Type {0} does not implement {1}",
                                                                              type.FullName,
                                                                              typeof (CriteriumCompilerBase).FullName));
                    }
                    else
                    {
                        if (m_log.IsDebugEnabled)
                            m_log.DebugFormat("Class skipped: {0}", type.FullName);
                    }
                }
            }
        }

        public void CompileSearchRequest(SearchRequest request)
        {
        }
    }
}