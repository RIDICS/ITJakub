using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using System.Xml;
using ITJakub.Web.Hub.Models.Plugins.Keyboard;
using Newtonsoft.Json;

namespace ITJakub.Web.Hub.Controllers.Plugins.Keyboard
{
    public class KeyboardController : BaseController
    {
        private readonly string m_contentLayouts = "~/Content/KeyboardLayouts";
        private readonly Lazy<Dictionary<string, LayoutListItem>> m_layoutKeys;
        private readonly Lazy<Dictionary<string, string>> m_layouts;

        public KeyboardController()
        {
            m_layoutKeys = new Lazy<Dictionary<string, LayoutListItem>>(LoadLayoutKeys);
            m_layouts = new Lazy<Dictionary<string, string>>(LoadLayouts);
        }


        public Dictionary<string, LayoutListItem> LoadLayoutKeys()
        {
            var xml = new XmlDocument();
            var result = new Dictionary<string, LayoutListItem>();

            foreach (var layout in Directory.GetFiles(Server.MapPath(m_contentLayouts), "*.xml"))
            {
                xml.Load(layout);

                var layoutTitle = xml.DocumentElement?.SelectSingleNode("/Keyboard/Title");
                if (layoutTitle == null) continue;

                var layoutId = xml.DocumentElement?.SelectSingleNode("/Keyboard/Id");
                if (layoutId == null) continue;

                result.Add(layoutTitle.InnerText, new LayoutListItem
                {
                    Name = layoutTitle.InnerText,
                    Id = layoutId.InnerText,
                    Url = Url.Action("GetLayout", "Keyboard", new {layoutId = layoutId.InnerText}, Request.Url?.Scheme)
                });
            }

            return result;
        }

        private Dictionary<string, string> LoadLayouts()
        {
            var xml = new XmlDocument();
            var result = new Dictionary<string, string>();

            foreach (var layout in Directory.GetFiles(Server.MapPath(m_contentLayouts), "*.xml"))
            {
                xml.Load(layout);

                var layoutId = xml.DocumentElement?.SelectSingleNode("/Keyboard/Id");
                if (layoutId == null) continue;

                result.Add(layoutId.InnerText, JsonConvert.SerializeXmlNode(xml));
            }

            return result;
        }

        /// <summary>
        ///     Requested return format: [{name: "Čeština", id: "csCS", url: "localhost/layout.xml"}, {...},...]
        /// </summary>
        /// <returns></returns>
        public ActionResult GetLayoutList()
        {
            return Json(JsonConvert.SerializeObject(m_layoutKeys.Value.Values), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLayout(string layoutId)
        {
            return Json(m_layouts.Value[layoutId], JsonRequestBehavior.AllowGet);
        }
    }
}