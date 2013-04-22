using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ujc.Naki.DataLayer
{
    public static class EnumUtils
    {
        public static DocumentView? GetDocumentView(string value)
        {
            value = value.ToUpper();
            DocumentView? docView = null;
            if (Enum.IsDefined(typeof(DocumentView), value))
            {
                docView = (DocumentView)Enum.Parse(typeof(DocumentView), value, true);
            }

            return docView;
        }

        public static DocumentKind? GetDocumentKind(string value)
        {
            value = value.ToUpper();
            DocumentKind? docKind = null;
            if (Enum.IsDefined(typeof(DocumentKind), value))
            {
                docKind = (DocumentKind)Enum.Parse(typeof(DocumentKind), value, true);
            }

            return docKind;
        }

        public static DocumentGenre? GetDocumentGenre(string value)
        {
            value = value.ToUpper();
            DocumentGenre? docGenre = null;
            if (Enum.IsDefined(typeof(DocumentView), value))
            {
                docGenre = (DocumentGenre)Enum.Parse(typeof(DocumentGenre), value, true);
            }

            return docGenre;
        }

        public static DocumentOriginal? GetDocumentOriginal(string value)
        {
            value = value.ToUpper();
            DocumentOriginal? docOriginal = null;
            if (Enum.IsDefined(typeof(DocumentView), value))
            {
                docOriginal = (DocumentOriginal)Enum.Parse(typeof(DocumentOriginal), value, true);
            }

            return docOriginal;
        }
    }
}
