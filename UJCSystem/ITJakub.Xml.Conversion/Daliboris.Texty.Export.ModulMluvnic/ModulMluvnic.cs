using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daliboris.Texty.Evidence.Rozhrani;
using Daliboris.Texty.Export.Rozhrani;
using ITJakub.Shared.Contracts.Resources;
using Ujc.Ovj.Tools.Xml.XsltTransformation;

namespace Daliboris.Texty.Export
{
    public class ModulMluvnic : ExportBase
    {
        #region Overrides of ExportBase

        public ModulMluvnic(IExportNastaveni nastaveni) : base(nastaveni)
        {
        }
        
        protected override void Exportuj(IPrepis prpPrepis, IList<string> xmlOutputFiles)
        {
            throw new NotImplementedException();
        }

        public override void Exportuj(IPrepis prpPrepis, IList<string> xmlOutputFiles, Dictionary<ResourceType, string[]> uploadedFiles)
        {
            ExportujImpl(prpPrepis, xmlOutputFiles, uploadedFiles);
        }

        private void ExportujImpl(IPrepis prepis, IList<string> xmlOutputFiles, Dictionary<ResourceType, string[]> uploadedFiles)
        {


            /*
            IList<IXsltTransformer> body = XsltTransformerFactory.GetXsltTransformers(Nastaveni.SouborTransformaci, "body", Nastaveni.SlozkaXslt);
            string konecnyVystup = null;
            
            const string csPriponaXml = ".xml";

            DateTime casExportu = Nastaveni.CasExportu == DateTime.MinValue ? DateTime.Now : Nastaveni.CasExportu;
            string souborBezPripony = prepis.Soubor.NazevBezPripony;

            konecnyVystup = Path.Combine(Nastaveni.VystupniSlozka, prepis.Soubor.NazevBezPripony + csPriponaXml);

            string headerFile = Path.Combine(Nastaveni.DocasnaSlozka, String.Format("{0}_{1}.xml", souborBezPripony, "header"));
            NameValueCollection parameters = new NameValueCollection();
            ApplyTransformations(Nastaveni.SouborMetadat, headerFile, body, Nastaveni.DocasnaSlozka, parameters);*/
        }

        #endregion
    }
}
