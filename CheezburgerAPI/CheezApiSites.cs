﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:2.0.50727.1873
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System;

// 
// This source code was auto-generated by xsd, Version=2.0.50727.42.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", ElementName = "Sites", IsNullable = false)]
public partial class CheezSites {

    private SitesSites[] itemsField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Sites", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public SitesSites[] Items {
        get {
            return this.itemsField;
        }
        set {
            this.itemsField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class SitesSites {

    private CheezSite[] siteField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Site", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public CheezSite[] Site {
        get {
            return this.siteField;
        }
        set {
            this.siteField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class CheezSite : IComparable {

    private string siteIdField;

    private string nameField;

    private string urlField;

    private string descriptionField;

    private string shortDescriptionField;

    private string siteCategoryField;

    private string isNSFWField;

    private string isNewField;

    private string isFeatureSiteField;

    private int selectedCheezItemID;

    public int SelectedCheezItemID {
        get {
            try {
                return this.selectedCheezItemID;
            } catch {
                return 0;
            }
        }
        set {
            this.selectedCheezItemID = value;
        }
    }

    public string CheezSiteID {
        get {
            string[] tmp = SiteId.Split(new Char[] { '/' });
            try {
                return tmp.Last();
            } catch {
                return string.Empty;
            }
        }
    }
    public int CheezSiteIntID {
        get {
            return int.Parse(CheezSiteID);
        }
    }
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string SiteId {
        get {
            return this.siteIdField;
        }
        set {
            this.siteIdField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Url {
        get {
            return this.urlField;
        }
        set {
            this.urlField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Description {
        get {
            return this.descriptionField;
        }
        set {
            this.descriptionField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string ShortDescription {
        get {
            return this.shortDescriptionField;
        }
        set {
            this.shortDescriptionField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string SiteCategory {
        get {
            return this.siteCategoryField;
        }
        set {
            this.siteCategoryField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string IsNSFW {
        get {
            return this.isNSFWField;
        }
        set {
            this.isNSFWField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string IsNew {
        get {
            return this.isNewField;
        }
        set {
            this.isNewField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string IsFeatureSite {
        get {
            return this.isFeatureSiteField;
        }
        set {
            this.isFeatureSiteField = value;
        }
    }

    public override string ToString() {
        return String.Format("[{0}]: {1} ({2})", CheezSiteID, nameField, descriptionField);
    }

    #region IComparable Member

    public int CompareTo(object obj) {
        if(obj is CheezSite) {
            return (int.Parse(this.CheezSiteID).CompareTo(int.Parse(((CheezSite)obj).CheezSiteID)));
        } else
            throw new ArgumentException(obj.ToString() + " is not a CheezSite - therefore nothing can be compared!");
    }
    #endregion
}