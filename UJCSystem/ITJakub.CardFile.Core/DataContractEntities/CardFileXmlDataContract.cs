﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18033
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;

namespace ITJakub.CardFile.Core.DataContractEntities
{ // 
// This source code was auto-generated by xsd, Version=4.0.30319.17929.
// 


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType=true)]
    [XmlRoot(Namespace="", IsNullable=false)]
    public partial class files {
    
        private file[] fileField;
    
        /// <remarks/>
        [XmlElement("file")]
        public file[] file {
            get {
                return this.fileField;
            }
            set {
                this.fileField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType=true)]
    [XmlRoot(Namespace="", IsNullable=false)]
    public partial class file {
    
        private string nameField;
    
        private string descriptionField;
    
        private field[] fieldsField;
    
        private buckets[] bucketsField;
    
        private string idField;
    
        /// <remarks/>
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
    
        /// <remarks/>
        public string description {
            get {
                return this.descriptionField;
            }
            set {
                this.descriptionField = value;
            }
        }
    
        /// <remarks/>
        [XmlArrayItem("field", IsNullable=false)]
        public field[] fields {
            get {
                return this.fieldsField;
            }
            set {
                this.fieldsField = value;
            }
        }
    
        /// <remarks/>
        [XmlElement("buckets")]
        public buckets[] buckets {
            get {
                return this.bucketsField;
            }
            set {
                this.bucketsField = value;
            }
        }
    
        /// <remarks/>
        [XmlAttribute(DataType="integer")]
        public string id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType=true)]
    [XmlRoot(Namespace="", IsNullable=false)]
    public partial class field {
    
        private string nameField;
    
        private string parameternameField;
    
        private string typeField;
    
        private value[] enumField;
    
        /// <remarks/>
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
    
        /// <remarks/>
        [XmlElement("parameter-name", DataType="NCName")]
        public string parametername {
            get {
                return this.parameternameField;
            }
            set {
                this.parameternameField = value;
            }
        }
    
        /// <remarks/>
        public string type {
            get {
                return this.typeField;
            }
            set {
                this.typeField = value;
            }
        }
    
        /// <remarks/>
        [XmlArrayItem("value", IsNullable=false)]
        public value[] @enum {
            get {
                return this.enumField;
            }
            set {
                this.enumField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType=true)]
    [XmlRoot(Namespace="", IsNullable=false)]
    public partial class value {
    
        private string idField;
    
        private string valueField;
    
        /// <remarks/>
        [XmlAttribute(DataType="integer")]
        public string id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
    
        /// <remarks/>
        [XmlText(DataType="NCName")]
        public string Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType=true)]
    [XmlRoot(Namespace="", IsNullable=false)]
    public partial class buckets {
    
        private bucket[] bucketField;
    
        private string countField;
    
        /// <remarks/>
        [XmlElement("bucket")]
        public bucket[] bucket {
            get {
                return this.bucketField;
            }
            set {
                this.bucketField = value;
            }
        }
    
        /// <remarks/>
        [XmlAttribute(DataType="integer")]
        public string count {
            get {
                return this.countField;
            }
            set {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType=true)]
    [XmlRoot(Namespace="", IsNullable=false)]
    public partial class bucket {
    
        private string nameField;
    
        private cards cardsField;
    
        private string idField;
    
        /// <remarks/>
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
    
        /// <remarks/>
        public cards cards {
            get {
                return this.cardsField;
            }
            set {
                this.cardsField = value;
            }
        }
    
        /// <remarks/>
        [XmlAttribute(DataType="integer")]
        public string id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType=true)]
    [XmlRoot(Namespace="", IsNullable=false)]
    public partial class cards {
    
        private card[] cardField;
    
        private string countField;
    
        /// <remarks/>
        [XmlElement("card")]
        public card[] card {
            get {
                return this.cardField;
            }
            set {
                this.cardField = value;
            }
        }
    
        /// <remarks/>
        [XmlAttribute(DataType="integer")]
        public string count {
            get {
                return this.countField;
            }
            set {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType=true)]
    [XmlRoot(Namespace="", IsNullable=false)]
    public partial class card {
    
        private string positionField;
    
        private image[] imageField;
    
        private string headwordField;
    
        private string warningField;
    
        private string noteField;
    
        private string idField;
    
        /// <remarks/>
        [XmlElement(DataType="integer")]
        public string position {
            get {
                return this.positionField;
            }
            set {
                this.positionField = value;
            }
        }
    
        /// <remarks/>
        [XmlElement("image")]
        public image[] image {
            get {
                return this.imageField;
            }
            set {
                this.imageField = value;
            }
        }
    
        /// <remarks/>
        [XmlElement(DataType="NCName")]
        public string headword {
            get {
                return this.headwordField;
            }
            set {
                this.headwordField = value;
            }
        }
    
        /// <remarks/>
        public string warning {
            get {
                return this.warningField;
            }
            set {
                this.warningField = value;
            }
        }
    
        /// <remarks/>
        public string note {
            get {
                return this.noteField;
            }
            set {
                this.noteField = value;
            }
        }
    
        /// <remarks/>
        [XmlAttribute(DataType="integer")]
        public string id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType=true)]
    [XmlRoot(Namespace="", IsNullable=false)]
    public partial class image {
    
        private string idField;
    
        /// <remarks/>
        [XmlAttribute(DataType="integer")]
        public string id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType=true)]
    [XmlRoot(Namespace="", IsNullable=false)]
    public partial class fields {
    
        private field[] fieldField;
    
        /// <remarks/>
        [XmlElement("field")]
        public field[] field {
            get {
                return this.fieldField;
            }
            set {
                this.fieldField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType=true)]
    [XmlRoot(Namespace="", IsNullable=false)]
    public partial class @enum {
    
        private value[] valueField;
    
        /// <remarks/>
        [XmlElement("value")]
        public value[] value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
}