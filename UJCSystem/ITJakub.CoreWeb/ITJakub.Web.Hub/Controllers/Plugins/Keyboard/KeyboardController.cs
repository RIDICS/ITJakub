using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using ITJakub.Web.Hub.Models.Plugins.Keyboard;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ITJakub.Web.Hub.Controllers.Plugins.Keyboard
{
    public class KeyboardController : BaseController
    {
        private readonly string m_contentLayouts;
        private readonly Lazy<Dictionary<string, LayoutListItem>> m_layoutKeys;
        private readonly Lazy<Dictionary<string, string>> m_layouts;

        public KeyboardController(IHostingEnvironment environment)
        {
            m_layoutKeys = new Lazy<Dictionary<string, LayoutListItem>>(LoadLayoutKeys);
            m_layouts = new Lazy<Dictionary<string, string>>(LoadLayouts);
            m_contentLayouts = Path.Combine(environment.WebRootPath, "Content/KeyboardLayouts");
        }


        public Dictionary<string, LayoutListItem> LoadLayoutKeys()
        {
            var xml = new XmlDocument();
            var result = new Dictionary<string, LayoutListItem>();

            foreach (var layout in Directory.GetFiles(m_contentLayouts, "*.xml"))
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
                    Url = Url.Action("GetLayout", "Keyboard", new {layoutId = layoutId.InnerText}, Request.Scheme)
                });
            }

            return result;
        }

        private Dictionary<string, string> LoadLayouts()
        {
            var xml = new XmlDocument();
            var result = new Dictionary<string, string>();

            foreach (var layout in Directory.GetFiles(m_contentLayouts, "*.xml"))
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
        ///[OutputCache(Duration = 60, VaryByParam = "none")]
        public ActionResult GetLayoutList()
        {
            return Json(JsonConvert.SerializeObject(m_layoutKeys.Value.Values));
        }

        ///[OutputCache(Duration = 60, VaryByParam = "layoutId")]
        public ActionResult GetLayout(string layoutId)
        {
            return Json(m_layouts.Value[layoutId]);
        }
    }
}