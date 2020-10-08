using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagementPortal.HelperClasses
{
    public class DataConstants
    {
        public static string ParamStoreJWTIssuer = "/mportal/Issuer";
        public static string ParamStoreJWTKey = "/mportal/JWTKey";
        public static string ParamStoreBaseServicePublicURL = "/mportal/RestServiceBaseURIExternal";
        public static string ParamStoreSSLCertificateName = "/mportal/SSLCertificateFileName";
        public static string ParamStoreSSLCertificatePassword = "/mportal/SSLCertificatePassword";
        public static string ParamStoreSSLServicePort = "/mportal/ServicePort";
        public static string ParamStoreBaseServicePrivateURL = "/mportal/RestServiceBaseURIInternal";
    }
}
