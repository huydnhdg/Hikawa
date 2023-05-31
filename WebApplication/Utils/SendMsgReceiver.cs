using System.Diagnostics;
using System.Web.Services;
using System.ComponentModel;
using System.Web.Services.Protocols;
using System;
using System.Xml.Serialization;


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.1433")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Web.Services.WebServiceBindingAttribute(Name = "MTReceiverPort", Namespace = "MTService")]
public class SendMsgReceiver : System.Web.Services.Protocols.SoapHttpClientProtocol
{

    #region ***** Fields & Properties ***** 

    private string _UserName;
    public string UserName
    {
        get
        {
            return _UserName;
        }
        set
        {
            _UserName = value;
        }
    }
    private string _Password;
    public string Password
    {
        get
        {
            return _Password;
        }
        set
        {
            _Password = value;
        }
    }
    #endregion
    /// <remarks/>
    public SendMsgReceiver()
    {
        //this.Url = "http://203.162.70.231/mt-services/MTService";
        this.Url = "http://sms.8x77.vn:8077/mt-services/MTService";
        if ((this.IsLocalFileSystemWebService(this.Url) == true))
        {
            this.UseDefaultCredentials = true;
            this.useDefaultCredentialsSetExplicitly = false;
        }
        else
        {
            this.useDefaultCredentialsSetExplicitly = true;
        }
    }
    public SendMsgReceiver(string serviceURL)
    {
        this.Url = serviceURL;
        if ((this.IsLocalFileSystemWebService(this.Url) == true))
        {
            this.UseDefaultCredentials = true;
            this.useDefaultCredentialsSetExplicitly = false;
        }
        else
        {
            this.useDefaultCredentialsSetExplicitly = true;
        }
    }


    private System.Threading.SendOrPostCallback sendMTOperationCompleted;

    private System.Threading.SendOrPostCallback sendMTExOperationCompleted;

    private System.Threading.SendOrPostCallback sendMTsOperationCompleted;

    private System.Threading.SendOrPostCallback sendRequestOperationCompleted;

    private System.Threading.SendOrPostCallback smsserviceMOOperationCompleted;

    private System.Threading.SendOrPostCallback ReceiveMOOperationCompleted;

    private System.Threading.SendOrPostCallback getLatestUsersOperationCompleted;

    private bool useDefaultCredentialsSetExplicitly;

    public new string Url
    {
        get
        {
            return base.Url;
        }
        set
        {
            if ((((this.IsLocalFileSystemWebService(base.Url) == true)
                        && (this.useDefaultCredentialsSetExplicitly == false))
                        && (this.IsLocalFileSystemWebService(value) == false)))
            {
                base.UseDefaultCredentials = false;
            }
            base.Url = value;
        }
    }

    public new bool UseDefaultCredentials
    {
        get
        {
            return base.UseDefaultCredentials;
        }
        set
        {
            base.UseDefaultCredentials = value;
            this.useDefaultCredentialsSetExplicitly = true;
        }
    }

    /// <remarks/>
    public event sendMTCompletedEventHandler sendMTCompleted;

    /// <remarks/>
    public event sendMTExCompletedEventHandler sendMTExCompleted;

    /// <remarks/>
    public event sendMTsCompletedEventHandler sendMTsCompleted;

    /// <remarks/>
    public event sendRequestCompletedEventHandler sendRequestCompleted;

    /// <remarks/>
    public event smsserviceMOCompletedEventHandler smsserviceMOCompleted;

    /// <remarks/>
    public event ReceiveMOCompletedEventHandler ReceiveMOCompleted;

    /// <remarks/>
    public event getLatestUsersCompletedEventHandler getLatestUsersCompleted;

    /// <remarks/>
    [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "MTService", ResponseNamespace = "MTService")]
    [return: System.Xml.Serialization.SoapElementAttribute("result")]
    public int sendMT(string @string, string string0, string string1, string string2, string string3, string string4, string string5, string string6, string string7, string string8)
    {
        object[] results = this.Invoke("sendMT", new object[] {
                        @string,
                        string0,
                        string1,
                        string2,
                        string3,
                        string4,
                        string5,
                        string6,
                        string7,
                        string8});
        return ((int)(results[0]));
    }

    /// <remarks/>
    public void sendMTAsync(string @string, string string0, string string1, string string2, string string3, string string4, string string5, string string6, string string7, string string8)
    {
        this.sendMTAsync(@string, string0, string1, string2, string3, string4, string5, string6, string7, string8, null);
    }

    /// <remarks/>
    public void sendMTAsync(string @string, string string0, string string1, string string2, string string3, string string4, string string5, string string6, string string7, string string8, object userState)
    {
        if ((this.sendMTOperationCompleted == null))
        {
            this.sendMTOperationCompleted = new System.Threading.SendOrPostCallback(this.OnsendMTOperationCompleted);
        }
        this.InvokeAsync("sendMT", new object[] {
                        @string,
                        string0,
                        string1,
                        string2,
                        string3,
                        string4,
                        string5,
                        string6,
                        string7,
                        string8}, this.sendMTOperationCompleted, userState);
    }

    private void OnsendMTOperationCompleted(object arg)
    {
        if ((this.sendMTCompleted != null))
        {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.sendMTCompleted(this, new sendMTCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }

    /// <remarks/>
    [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "MTService", ResponseNamespace = "MTService")]
    [return: System.Xml.Serialization.SoapElementAttribute("result")]
    public int sendMTEx(string mobile, string message, string serviceId, string commandCode, string messageType, string receiveId, string totalMsg, string msgIndex, string isMore, string contentType, string port)
    {
        object[] results = this.Invoke("sendMTEx", new object[] {
                        mobile,
                        message,
                        serviceId,
                        commandCode,
                        messageType,
                        receiveId,
                        totalMsg,
                        msgIndex,
                        isMore,
                        contentType,
                        port});
        return ((int)(results[0]));
    }

    /// <remarks/>
    public void sendMTExAsync(string mobile, string message, string serviceId, string commandCode, string messageType, string receiveId, string totalMsg, string msgIndex, string isMore, string contentType, string port)
    {
        this.sendMTExAsync(mobile, message, serviceId, commandCode, messageType, receiveId, totalMsg, msgIndex, isMore, contentType, port, null);
    }

    /// <remarks/>
    public void sendMTExAsync(string mobile, string message, string serviceId, string commandCode, string messageType, string receiveId, string totalMsg, string msgIndex, string isMore, string contentType, string port, object userState)
    {
        if ((this.sendMTExOperationCompleted == null))
        {
            this.sendMTExOperationCompleted = new System.Threading.SendOrPostCallback(this.OnsendMTExOperationCompleted);
        }
        this.InvokeAsync("sendMTEx", new object[] {
                        mobile,
                        message,
                        serviceId,
                        commandCode,
                        messageType,
                        receiveId,
                        totalMsg,
                        msgIndex,
                        isMore,
                        contentType,
                        port}, this.sendMTExOperationCompleted, userState);
    }

    private void OnsendMTExOperationCompleted(object arg)
    {
        if ((this.sendMTExCompleted != null))
        {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.sendMTExCompleted(this, new sendMTExCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }

    /// <remarks/>
    [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "MTService", ResponseNamespace = "MTService")]
    [return: System.Xml.Serialization.SoapElementAttribute("result")]
    public string sendMTs(string mobileList, string message, string serviceId, string commandCode, string contentType)
    {
        object[] results = this.Invoke("sendMTs", new object[] {
                        mobileList,
                        message,
                        serviceId,
                        commandCode,
                        contentType});
        return ((string)(results[0]));
    }

    /// <remarks/>
    public void sendMTsAsync(string mobileList, string message, string serviceId, string commandCode, string contentType)
    {
        this.sendMTsAsync(mobileList, message, serviceId, commandCode, contentType, null);
    }

    /// <remarks/>
    public void sendMTsAsync(string mobileList, string message, string serviceId, string commandCode, string contentType, object userState)
    {
        if ((this.sendMTsOperationCompleted == null))
        {
            this.sendMTsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnsendMTsOperationCompleted);
        }
        this.InvokeAsync("sendMTs", new object[] {
                        mobileList,
                        message,
                        serviceId,
                        commandCode,
                        contentType}, this.sendMTsOperationCompleted, userState);
    }

    private void OnsendMTsOperationCompleted(object arg)
    {
        if ((this.sendMTsCompleted != null))
        {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.sendMTsCompleted(this, new sendMTsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }

    /// <remarks/>
    [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "MTService", ResponseNamespace = "MTService")]
    [return: System.Xml.Serialization.SoapElementAttribute("result")]
    public int sendRequest(string serviceId, string userId, string info, string commandCode, string requestId)
    {
        object[] results = this.Invoke("sendRequest", new object[] {
                        serviceId,
                        userId,
                        info,
                        commandCode,
                        requestId});
        return ((int)(results[0]));
    }

    /// <remarks/>
    public void sendRequestAsync(string serviceId, string userId, string info, string commandCode, string requestId)
    {
        this.sendRequestAsync(serviceId, userId, info, commandCode, requestId, null);
    }

    /// <remarks/>
    public void sendRequestAsync(string serviceId, string userId, string info, string commandCode, string requestId, object userState)
    {
        if ((this.sendRequestOperationCompleted == null))
        {
            this.sendRequestOperationCompleted = new System.Threading.SendOrPostCallback(this.OnsendRequestOperationCompleted);
        }
        this.InvokeAsync("sendRequest", new object[] {
                        serviceId,
                        userId,
                        info,
                        commandCode,
                        requestId}, this.sendRequestOperationCompleted, userState);
    }

    private void OnsendRequestOperationCompleted(object arg)
    {
        if ((this.sendRequestCompleted != null))
        {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.sendRequestCompleted(this, new sendRequestCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }

    /// <remarks/>
    [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "MTService", ResponseNamespace = "MTService")]
    [return: System.Xml.Serialization.SoapElementAttribute("result")]
    public int smsserviceMO(string @string, string string0, string string1, string string2)
    {
        object[] results = this.Invoke("smsserviceMO", new object[] {
                        @string,
                        string0,
                        string1,
                        string2});
        return ((int)(results[0]));
    }

    /// <remarks/>
    public void smsserviceMOAsync(string @string, string string0, string string1, string string2)
    {
        this.smsserviceMOAsync(@string, string0, string1, string2, null);
    }

    /// <remarks/>
    public void smsserviceMOAsync(string @string, string string0, string string1, string string2, object userState)
    {
        if ((this.smsserviceMOOperationCompleted == null))
        {
            this.smsserviceMOOperationCompleted = new System.Threading.SendOrPostCallback(this.OnsmsserviceMOOperationCompleted);
        }
        this.InvokeAsync("smsserviceMO", new object[] {
                        @string,
                        string0,
                        string1,
                        string2}, this.smsserviceMOOperationCompleted, userState);
    }

    private void OnsmsserviceMOOperationCompleted(object arg)
    {
        if ((this.smsserviceMOCompleted != null))
        {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.smsserviceMOCompleted(this, new smsserviceMOCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }

    /// <remarks/>
    [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "MTService", ResponseNamespace = "MTService")]
    [return: System.Xml.Serialization.SoapElementAttribute("result")]
    public int ReceiveMO(string @string, string string0, string string1, string string2, string string3, string string4, string string5, string string6)
    {
        object[] results = this.Invoke("ReceiveMO", new object[] {
                        @string,
                        string0,
                        string1,
                        string2,
                        string3,
                        string4,
                        string5,
                        string6});
        return ((int)(results[0]));
    }

    /// <remarks/>
    public void ReceiveMOAsync(string @string, string string0, string string1, string string2, string string3, string string4, string string5, string string6)
    {
        this.ReceiveMOAsync(@string, string0, string1, string2, string3, string4, string5, string6, null);
    }

    /// <remarks/>
    public void ReceiveMOAsync(string @string, string string0, string string1, string string2, string string3, string string4, string string5, string string6, object userState)
    {
        if ((this.ReceiveMOOperationCompleted == null))
        {
            this.ReceiveMOOperationCompleted = new System.Threading.SendOrPostCallback(this.OnReceiveMOOperationCompleted);
        }
        this.InvokeAsync("ReceiveMO", new object[] {
                        @string,
                        string0,
                        string1,
                        string2,
                        string3,
                        string4,
                        string5,
                        string6}, this.ReceiveMOOperationCompleted, userState);
    }

    private void OnReceiveMOOperationCompleted(object arg)
    {
        if ((this.ReceiveMOCompleted != null))
        {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.ReceiveMOCompleted(this, new ReceiveMOCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }

    /// <remarks/>
    [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "MTService", ResponseNamespace = "MTService")]
    [return: System.Xml.Serialization.SoapElementAttribute("result")]
    public string getLatestUsers(string GuessX, string GuessY, string CommandCode, string ServiceId, int Top)
    {
        object[] results = this.Invoke("getLatestUsers", new object[] {
                        GuessX,
                        GuessY,
                        CommandCode,
                        ServiceId,
                        Top});
        return ((string)(results[0]));
    }

    /// <remarks/>
    public void getLatestUsersAsync(string GuessX, string GuessY, string CommandCode, string ServiceId, int Top)
    {
        this.getLatestUsersAsync(GuessX, GuessY, CommandCode, ServiceId, Top, null);
    }

    /// <remarks/>
    public void getLatestUsersAsync(string GuessX, string GuessY, string CommandCode, string ServiceId, int Top, object userState)
    {
        if ((this.getLatestUsersOperationCompleted == null))
        {
            this.getLatestUsersOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetLatestUsersOperationCompleted);
        }
        this.InvokeAsync("getLatestUsers", new object[] {
                        GuessX,
                        GuessY,
                        CommandCode,
                        ServiceId,
                        Top}, this.getLatestUsersOperationCompleted, userState);
    }

    private void OngetLatestUsersOperationCompleted(object arg)
    {
        if ((this.getLatestUsersCompleted != null))
        {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.getLatestUsersCompleted(this, new getLatestUsersCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }

    /// <remarks/>
    public new void CancelAsync(object userState)
    {
        base.CancelAsync(userState);
    }

    private bool IsLocalFileSystemWebService(string url)
    {
        if (((url == null)
                    || (url == string.Empty)))
        {
            return false;
        }
        System.Uri wsUri = new System.Uri(url);
        if (((wsUri.Port >= 1024)
                    && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0)))
        {
            return true;
        }
        return false;
    }

    protected override System.Net.WebRequest
               GetWebRequest(Uri uri)
    {
        System.Net.HttpWebRequest request =
            (System.Net.HttpWebRequest)base.GetWebRequest(uri);
        if (this.PreAuthenticate)
        {
            byte[] credBuf = new System.Text.UTF8Encoding().GetBytes(_UserName + ":" + _Password);
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(credBuf);
        }
        return request;
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.1433")]
public delegate void sendMTCompletedEventHandler(object sender, sendMTCompletedEventArgs e);

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.1433")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class sendMTCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
{

    private object[] results;

    internal sendMTCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
    {
        this.results = results;
    }

    /// <remarks/>
    public int Result
    {
        get
        {
            this.RaiseExceptionIfNecessary();
            return ((int)(this.results[0]));
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.1433")]
public delegate void sendMTExCompletedEventHandler(object sender, sendMTExCompletedEventArgs e);

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.1433")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class sendMTExCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
{

    private object[] results;

    internal sendMTExCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
    {
        this.results = results;
    }

    /// <remarks/>
    public int Result
    {
        get
        {
            this.RaiseExceptionIfNecessary();
            return ((int)(this.results[0]));
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.1433")]
public delegate void sendMTsCompletedEventHandler(object sender, sendMTsCompletedEventArgs e);

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.1433")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class sendMTsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
{

    private object[] results;

    internal sendMTsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
    {
        this.results = results;
    }

    /// <remarks/>
    public string Result
    {
        get
        {
            this.RaiseExceptionIfNecessary();
            return ((string)(this.results[0]));
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.1433")]
public delegate void sendRequestCompletedEventHandler(object sender, sendRequestCompletedEventArgs e);

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.1433")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class sendRequestCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
{

    private object[] results;

    internal sendRequestCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
    {
        this.results = results;
    }

    /// <remarks/>
    public int Result
    {
        get
        {
            this.RaiseExceptionIfNecessary();
            return ((int)(this.results[0]));
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.1433")]
public delegate void smsserviceMOCompletedEventHandler(object sender, smsserviceMOCompletedEventArgs e);

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.1433")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class smsserviceMOCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
{

    private object[] results;

    internal smsserviceMOCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
    {
        this.results = results;
    }

    /// <remarks/>
    public int Result
    {
        get
        {
            this.RaiseExceptionIfNecessary();
            return ((int)(this.results[0]));
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.1433")]
public delegate void ReceiveMOCompletedEventHandler(object sender, ReceiveMOCompletedEventArgs e);

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.1433")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class ReceiveMOCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
{

    private object[] results;

    internal ReceiveMOCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
    {
        this.results = results;
    }

    /// <remarks/>
    public int Result
    {
        get
        {
            this.RaiseExceptionIfNecessary();
            return ((int)(this.results[0]));
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.1433")]
public delegate void getLatestUsersCompletedEventHandler(object sender, getLatestUsersCompletedEventArgs e);

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.1433")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class getLatestUsersCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
{

    private object[] results;

    internal getLatestUsersCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
    {
        this.results = results;
    }

    /// <remarks/>
    public string Result
    {
        get
        {
            this.RaiseExceptionIfNecessary();
            return ((string)(this.results[0]));
        }
    }
}
