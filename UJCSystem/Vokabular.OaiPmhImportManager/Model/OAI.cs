﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
// 
// This source code was auto-generated by xsd, Version=4.0.30319.33440.
// 

using System;
using System.Xml.Serialization;

namespace Vokabular.OaiPmhImportManager.Model
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [Serializable()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(TypeName = "OAI-PMHtype", Namespace = "http://www.openarchives.org/OAI/2.0/")]
    [XmlRoot("OAI-PMH", Namespace = "http://www.openarchives.org/OAI/2.0/", IsNullable = false)]
    public partial class OAIPMHType
    {

        private System.DateTime responseDateField;

        private requestType requestField;

        private object[] itemsField;

        /// <remarks/>
        public System.DateTime responseDate
        {
            get
            {
                return this.responseDateField;
            }
            set
            {
                this.responseDateField = value;
            }
        }

        /// <remarks/>
        public requestType request
        {
            get
            {
                return this.requestField;
            }
            set
            {
                this.requestField = value;
            }
        }

        /// <remarks/>
        [XmlElement("GetRecord", typeof(GetRecordType))]
        [XmlElement("Identify", typeof(IdentifyType))]
        [XmlElement("ListIdentifiers", typeof(ListIdentifiersType))]
        [XmlElement("ListMetadataFormats", typeof(ListMetadataFormatsType))]
        [XmlElement("ListRecords", typeof(ListRecordsType))]
        [XmlElement("ListSets", typeof(ListSetsType))]
        [XmlElement("error", typeof(OAIPMHerrorType))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [Serializable()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://www.openarchives.org/OAI/2.0/")]
    public partial class requestType
    {

        private verbType verbField;

        private bool verbFieldSpecified;

        private string identifierField;

        private string metadataPrefixField;

        private string fromField;

        private string untilField;

        private string setField;

        private string resumptionTokenField;

        private string valueField;

        /// <remarks/>
        [XmlAttribute()]
        public verbType verb
        {
            get
            {
                return this.verbField;
            }
            set
            {
                this.verbField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool verbSpecified
        {
            get
            {
                return this.verbFieldSpecified;
            }
            set
            {
                this.verbFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(DataType = "anyURI")]
        public string identifier
        {
            get
            {
                return this.identifierField;
            }
            set
            {
                this.identifierField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string metadataPrefix
        {
            get
            {
                return this.metadataPrefixField;
            }
            set
            {
                this.metadataPrefixField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string from
        {
            get
            {
                return this.fromField;
            }
            set
            {
                this.fromField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string until
        {
            get
            {
                return this.untilField;
            }
            set
            {
                this.untilField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string set
        {
            get
            {
                return this.setField;
            }
            set
            {
                this.setField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string resumptionToken
        {
            get
            {
                return this.resumptionTokenField;
            }
            set
            {
                this.resumptionTokenField = value;
            }
        }

        /// <remarks/>
        [XmlText(DataType = "anyURI")]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [Serializable()]
    [XmlType(Namespace = "http://www.openarchives.org/OAI/2.0/")]
    public enum verbType
    {

        /// <remarks/>
        Identify,

        /// <remarks/>
        ListMetadataFormats,

        /// <remarks/>
        ListSets,

        /// <remarks/>
        GetRecord,

        /// <remarks/>
        ListIdentifiers,

        /// <remarks/>
        ListRecords,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [Serializable()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://www.openarchives.org/OAI/2.0/")]
    public partial class ListRecordsType
    {

        private recordType[] recordField;

        private resumptionTokenType resumptionTokenField;

        /// <remarks/>
        [XmlElement("record")]
        public recordType[] record
        {
            get
            {
                return this.recordField;
            }
            set
            {
                this.recordField = value;
            }
        }

        /// <remarks/>
        public resumptionTokenType resumptionToken
        {
            get
            {
                return this.resumptionTokenField;
            }
            set
            {
                this.resumptionTokenField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [Serializable()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://www.openarchives.org/OAI/2.0/")]
    public partial class recordType
    {

        private headerType headerField;

        private System.Xml.XmlElement metadataField;

        private System.Xml.XmlElement[] aboutField;

        /// <remarks/>
        public headerType header
        {
            get
            {
                return this.headerField;
            }
            set
            {
                this.headerField = value;
            }
        }

        /// <remarks/>
        public System.Xml.XmlElement metadata
        {
            get
            {
                return this.metadataField;
            }
            set
            {
                this.metadataField = value;
            }
        }

        /// <remarks/>
        [XmlElement("about")]
        public System.Xml.XmlElement[] about
        {
            get
            {
                return this.aboutField;
            }
            set
            {
                this.aboutField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [Serializable()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://www.openarchives.org/OAI/2.0/")]
    public partial class headerType
    {

        private string identifierField;

        private string datestampField;

        private string[] setSpecField;

        private statusType statusField;

        private bool statusFieldSpecified;

        /// <remarks/>
        [XmlElement(DataType = "anyURI")]
        public string identifier
        {
            get
            {
                return this.identifierField;
            }
            set
            {
                this.identifierField = value;
            }
        }

        /// <remarks/>
        public string datestamp
        {
            get
            {
                return this.datestampField;
            }
            set
            {
                this.datestampField = value;
            }
        }

        /// <remarks/>
        [XmlElement("setSpec")]
        public string[] setSpec
        {
            get
            {
                return this.setSpecField;
            }
            set
            {
                this.setSpecField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public statusType status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool statusSpecified
        {
            get
            {
                return this.statusFieldSpecified;
            }
            set
            {
                this.statusFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [Serializable()]
    [XmlType(Namespace = "http://www.openarchives.org/OAI/2.0/")]
    public enum statusType
    {

        /// <remarks/>
        deleted,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [Serializable()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://www.openarchives.org/OAI/2.0/")]
    public partial class resumptionTokenType
    {

        private System.DateTime expirationDateField;

        private bool expirationDateFieldSpecified;

        private string completeListSizeField;

        private string cursorField;

        private string valueField;

        /// <remarks/>
        [XmlAttribute()]
        public System.DateTime expirationDate
        {
            get
            {
                return this.expirationDateField;
            }
            set
            {
                this.expirationDateField = value;
            }
        }

        /// <remarks/>
        [XmlIgnore()]
        public bool expirationDateSpecified
        {
            get
            {
                return this.expirationDateFieldSpecified;
            }
            set
            {
                this.expirationDateFieldSpecified = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(DataType = "positiveInteger")]
        public string completeListSize
        {
            get
            {
                return this.completeListSizeField;
            }
            set
            {
                this.completeListSizeField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute(DataType = "nonNegativeInteger")]
        public string cursor
        {
            get
            {
                return this.cursorField;
            }
            set
            {
                this.cursorField = value;
            }
        }

        /// <remarks/>
        [XmlText()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [Serializable()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://www.openarchives.org/OAI/2.0/")]
    public partial class ListIdentifiersType
    {

        private headerType[] headerField;

        private resumptionTokenType resumptionTokenField;

        /// <remarks/>
        [XmlElement("header")]
        public headerType[] header
        {
            get
            {
                return this.headerField;
            }
            set
            {
                this.headerField = value;
            }
        }

        /// <remarks/>
        public resumptionTokenType resumptionToken
        {
            get
            {
                return this.resumptionTokenField;
            }
            set
            {
                this.resumptionTokenField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [Serializable()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://www.openarchives.org/OAI/2.0/")]
    public partial class GetRecordType
    {

        private recordType recordField;

        /// <remarks/>
        public recordType record
        {
            get
            {
                return this.recordField;
            }
            set
            {
                this.recordField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [Serializable()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://www.openarchives.org/OAI/2.0/")]
    public partial class setType
    {

        private string setSpecField;

        private string setNameField;

        private System.Xml.XmlElement[] setDescriptionField;

        /// <remarks/>
        public string setSpec
        {
            get
            {
                return this.setSpecField;
            }
            set
            {
                this.setSpecField = value;
            }
        }

        /// <remarks/>
        public string setName
        {
            get
            {
                return this.setNameField;
            }
            set
            {
                this.setNameField = value;
            }
        }

        /// <remarks/>
        [XmlElement("setDescription")]
        public System.Xml.XmlElement[] setDescription
        {
            get
            {
                return this.setDescriptionField;
            }
            set
            {
                this.setDescriptionField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [Serializable()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://www.openarchives.org/OAI/2.0/")]
    public partial class ListSetsType
    {

        private setType[] setField;

        private resumptionTokenType resumptionTokenField;

        /// <remarks/>
        [XmlElement("set")]
        public setType[] set
        {
            get
            {
                return this.setField;
            }
            set
            {
                this.setField = value;
            }
        }

        /// <remarks/>
        public resumptionTokenType resumptionToken
        {
            get
            {
                return this.resumptionTokenField;
            }
            set
            {
                this.resumptionTokenField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [Serializable()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://www.openarchives.org/OAI/2.0/")]
    public partial class ListMetadataFormatsType
    {

        private metadataFormatType[] metadataFormatField;

        /// <remarks/>
        [XmlElement("metadataFormat")]
        public metadataFormatType[] metadataFormat
        {
            get
            {
                return this.metadataFormatField;
            }
            set
            {
                this.metadataFormatField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [Serializable()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://www.openarchives.org/OAI/2.0/")]
    public partial class metadataFormatType
    {

        private string metadataPrefixField;

        private string schemaField;

        private string metadataNamespaceField;

        /// <remarks/>
        public string metadataPrefix
        {
            get
            {
                return this.metadataPrefixField;
            }
            set
            {
                this.metadataPrefixField = value;
            }
        }

        /// <remarks/>
        [XmlElement(DataType = "anyURI")]
        public string schema
        {
            get
            {
                return this.schemaField;
            }
            set
            {
                this.schemaField = value;
            }
        }

        /// <remarks/>
        [XmlElement(DataType = "anyURI")]
        public string metadataNamespace
        {
            get
            {
                return this.metadataNamespaceField;
            }
            set
            {
                this.metadataNamespaceField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [Serializable()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://www.openarchives.org/OAI/2.0/")]
    public partial class IdentifyType
    {

        private string repositoryNameField;

        private string baseURLField;

        private protocolVersionType protocolVersionField;

        private string[] adminEmailField;

        private string earliestDatestampField;

        private deletedRecordType deletedRecordField;

        private granularityType granularityField;

        private string[] compressionField;

        private System.Xml.XmlElement[] descriptionField;

        /// <remarks/>
        public string repositoryName
        {
            get
            {
                return this.repositoryNameField;
            }
            set
            {
                this.repositoryNameField = value;
            }
        }

        /// <remarks/>
        [XmlElement(DataType = "anyURI")]
        public string baseURL
        {
            get
            {
                return this.baseURLField;
            }
            set
            {
                this.baseURLField = value;
            }
        }

        /// <remarks/>
        public protocolVersionType protocolVersion
        {
            get
            {
                return this.protocolVersionField;
            }
            set
            {
                this.protocolVersionField = value;
            }
        }

        /// <remarks/>
        [XmlElement("adminEmail")]
        public string[] adminEmail
        {
            get
            {
                return this.adminEmailField;
            }
            set
            {
                this.adminEmailField = value;
            }
        }

        /// <remarks/>
        public string earliestDatestamp
        {
            get
            {
                return this.earliestDatestampField;
            }
            set
            {
                this.earliestDatestampField = value;
            }
        }

        /// <remarks/>
        public deletedRecordType deletedRecord
        {
            get
            {
                return this.deletedRecordField;
            }
            set
            {
                this.deletedRecordField = value;
            }
        }

        /// <remarks/>
        public granularityType granularity
        {
            get
            {
                return this.granularityField;
            }
            set
            {
                this.granularityField = value;
            }
        }

        /// <remarks/>
        [XmlElement("compression")]
        public string[] compression
        {
            get
            {
                return this.compressionField;
            }
            set
            {
                this.compressionField = value;
            }
        }

        /// <remarks/>
        [XmlElement("description")]
        public System.Xml.XmlElement[] description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [Serializable()]
    [XmlType(Namespace = "http://www.openarchives.org/OAI/2.0/")]
    public enum protocolVersionType
    {

        /// <remarks/>
        [XmlEnum("2.0")]
        Item20,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [Serializable()]
    [XmlType(Namespace = "http://www.openarchives.org/OAI/2.0/")]
    public enum deletedRecordType
    {

        /// <remarks/>
        no,

        /// <remarks/>
        persistent,

        /// <remarks/>
        transient,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [Serializable()]
    [XmlType(Namespace = "http://www.openarchives.org/OAI/2.0/")]
    public enum granularityType
    {

        /// <remarks/>
        [XmlEnum("YYYY-MM-DD")]
        YYYYMMDD,

        /// <remarks/>
        [XmlEnum("YYYY-MM-DDThh:mm:ssZ")]
        YYYYMMDDThhmmssZ,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [Serializable()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(TypeName = "OAI-PMHerrorType", Namespace = "http://www.openarchives.org/OAI/2.0/")]
    public partial class OAIPMHerrorType
    {

        private OAIPMHerrorcodeType codeField;

        private string valueField;

        /// <remarks/>
        [XmlAttribute()]
        public OAIPMHerrorcodeType code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
            }
        }

        /// <remarks/>
        [XmlText()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [Serializable()]
    [XmlType(TypeName = "OAI-PMHerrorcodeType", Namespace = "http://www.openarchives.org/OAI/2.0/")]
    public enum OAIPMHerrorcodeType
    {

        /// <remarks/>
        cannotDisseminateFormat,

        /// <remarks/>
        idDoesNotExist,

        /// <remarks/>
        badArgument,

        /// <remarks/>
        badVerb,

        /// <remarks/>
        noMetadataFormats,

        /// <remarks/>
        noRecordsMatch,

        /// <remarks/>
        badResumptionToken,

        /// <remarks/>
        noSetHierarchy,
    }

}
