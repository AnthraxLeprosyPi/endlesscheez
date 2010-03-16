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
using System;

// 
// This source code was auto-generated by xsd, Version=2.0.50727.42.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class Assets {
    
    private string timeStampField;
    
    private CheezAsset[] assetField;
    
    private Assets[] assets1Field;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string TimeStamp {
        get {
            return this.timeStampField;
        }
        set {
            this.timeStampField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Asset", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public CheezAsset[] Asset {
        get {
            return this.assetField;
        }
        set {
            this.assetField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Assets")]
    public Assets[] Assets1 {
        get {
            return this.assets1Field;
        }
        set {
            this.assets1Field = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class CheezAsset {
    
    private string assetIdField;
    
    private string assetTypeField;
    
    private string contentUrlField;
    
    private string imageUrlField;
    
    private string titleField;
    
    private string descriptionField;
    
    private string fullTextField;
    
    private string timeStampField;
    
    private string permalinkField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string AssetId {
        get {
            return this.assetIdField;
        }
        set {
            this.assetIdField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string AssetType {
        get {
            return this.assetTypeField;
        }
        set {
            this.assetTypeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string ContentUrl {
        get {
            return this.contentUrlField;
        }
        set {
            this.contentUrlField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string ImageUrl {
        get {
            return this.imageUrlField;
        }
        set {
            this.imageUrlField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Title {
        get {
            return this.titleField;
        }
        set {
            this.titleField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Description {
        get {
            return this.descriptionField;
        }
        set {
            this.descriptionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string FullText {
        get {
            return this.fullTextField;
        }
        set {
            this.fullTextField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string TimeStamp {
        get {
            return this.timeStampField;
        }
        set {
            this.timeStampField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Permalink {
        get {
            return this.permalinkField;
        }
        set {
            this.permalinkField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="",ElementName= "CheezApiResponse", IsNullable=false)]
public partial class CheezApiResponse {

    private object[] itemsField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Assets", typeof(Assets))]
     public object[] Items {
        get {
            return this.itemsField;
        }
        set {
            this.itemsField = value;
        }
    }

    public CheezApiResponse(Fail fail) {
        this._responseFail = fail;
    }
    public CheezApiResponse() {
    }

    static public implicit operator Hai(CheezApiResponse cheezApiResponse) {
        return (cheezApiResponse != null) ? cheezApiResponse.Hai : null;
    }

    static public implicit operator Fail(CheezApiResponse cheezApiResponse) {
        return (cheezApiResponse != null) ? cheezApiResponse.Fail : null;
    }

    static public implicit operator List<CheezAsset>(CheezApiResponse cheezApiResponse) {
        if(cheezApiResponse != null) {
            return cheezApiResponse.CheezAssets;
        } else {
            return null;
        }
    }

    public List<CheezAsset> CheezAssets {
        get {
            try {
                return (((Assets)this.Items[0]).Assets1[0].Asset != null) ? new List<CheezAsset>(((Assets)this.Items[0]).Assets1[0].Asset) : null;
            } catch {
                return null;
            }
        }
    }
    private Fail _responseFail;

    [System.Xml.Serialization.XmlElementAttribute("Fail", typeof(Fail), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public Fail Fail {
        get {
            return this._responseFail;
        }
        set {
            this._responseFail = (Fail)value;
        }
    }
    private Hai _responseHai;

    [System.Xml.Serialization.XmlElementAttribute("Hai", typeof(Hai), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public Hai Hai {
        get {
            return this._responseHai;
        }
        set {
            this._responseHai = (Hai)value;
        }
    }

}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class Fail {
    
    private string failureMessageField;
    
    private string failureDetailField;
    
    private string failureEventIdField;

    

    public Fail(string failureMessage, string failureDetail, string failureId) {
        this.failureMessageField = failureMessage;
        this.failureDetailField = failureDetail;
        this.failureEventIdField = failureId;
    }

    public Fail(Exception e) {
        this.failureMessageField = e.Message;
        this.failureDetailField = e.StackTrace;
        this.failureEventIdField = e.Source;
    }

    public Fail() {
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string FailureMessage {
        get {
            return this.failureMessageField;
        }
        set {
            this.failureMessageField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string FailureDetail {
        get {
            return this.failureDetailField;
        }
        set {
            this.failureDetailField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string FailureEventId {
        get {
            return this.failureEventIdField;
        }
        set {
            this.failureEventIdField = value;
        }
    }

    public override string ToString() {
        return failureMessageField + " - " + failureDetailField + " - " + failureEventIdField;
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class Hai {
    
    private string greetingField;
    
    private string descriptionField;
    
    private string serverTimeField;
    
    private string versionField;
    
    private string environmentField;
    
    private string methodField;
    
    private string developerKeyInfoField;
    
    private string clientIdInfoField;
    
    private string authTokenInfoField;
    
    private CheezApiResponseHaiHeadersHttpHeader[] headersField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Greeting {
        get {
            return this.greetingField;
        }
        set {
            this.greetingField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Description {
        get {
            return this.descriptionField;
        }
        set {
            this.descriptionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string ServerTime {
        get {
            return this.serverTimeField;
        }
        set {
            this.serverTimeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Version {
        get {
            return this.versionField;
        }
        set {
            this.versionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Environment {
        get {
            return this.environmentField;
        }
        set {
            this.environmentField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Method {
        get {
            return this.methodField;
        }
        set {
            this.methodField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string DeveloperKeyInfo {
        get {
            return this.developerKeyInfoField;
        }
        set {
            this.developerKeyInfoField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string ClientIdInfo {
        get {
            return this.clientIdInfoField;
        }
        set {
            this.clientIdInfoField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string AuthTokenInfo {
        get {
            return this.authTokenInfoField;
        }
        set {
            this.authTokenInfoField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    [System.Xml.Serialization.XmlArrayItemAttribute("HttpHeader", typeof(CheezApiResponseHaiHeadersHttpHeader), Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
    public CheezApiResponseHaiHeadersHttpHeader[] Headers {
        get {
            return this.headersField;
        }
        set {
            this.headersField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class CheezApiResponseHaiHeadersHttpHeader {
    
    private string keyField;
    
    private string valueField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string key {
        get {
            return this.keyField;
        }
        set {
            this.keyField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string value {
        get {
            return this.valueField;
        }
        set {
            this.valueField = value;
        }
    }
}
