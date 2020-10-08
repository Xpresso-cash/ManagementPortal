using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagementPortal.HelperClasses
{
    public class AWSParamStore
    {
        private static JObject AppParams { get; set; }
        public AWSParamStore()
        {
        }

        internal async Task<string> GetValueFromParamStoreNoKey(string name)
        {

            if (AppParams.ContainsKey(name))
            {
                return AppParams[name].ToString();
            }
            else
            {
                var request = new GetParameterRequest()
                {
                    Name = name,
                    WithDecryption = true
                };

                using (var client = new AmazonSimpleSystemsManagementClient())
                {
                    try
                    {
                        var response = await client.GetParameterAsync(request);
                        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                        {
                            AppParams.Add(name, response.Parameter.Value);
                            return response.Parameter.Value;
                        }
                        else
                        {
                            throw new Exception("An error occured in getting value from Parameter Store, the remote service returned HTTP " + response.HttpStatusCode.ToString());
                        }
                    }
                    catch (Exception exception)
                    {
                        throw exception;
                    }
                }
            }

        }

        internal async Task<string> GetValueFromParamStore(string name)
        {
            if (AppParams != null && AppParams.ContainsKey(name))
            {
                return AppParams[name].ToString();
            }
            else
            {
                var request = new GetParameterRequest()
                {
                    Name = name,
                    WithDecryption = true
                };

                using (var client = new AmazonSimpleSystemsManagementClient())
                {
                    try
                    {

                        var response = await client.GetParameterAsync(request);
                        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                        {
                            if (AppParams == null)
                            {
                                AppParams = new JObject();
                            }
                            AppParams.Add(name, response.Parameter.Value);
                            return response.Parameter.Value;
                        }
                        else
                        {
                            throw new Exception("An error occured in getting value from Parameter Store, the remote service returned HTTP " + response.HttpStatusCode.ToString());
                        }
                    }
                    catch (Exception exception)
                    {
                        throw exception;
                    }
                }
            }


        }

        internal async Task<List<string>> GetValueListFromParamStore(string name)
        {
            var request = new GetParametersByPathRequest()
            {
                Path = name,
                WithDecryption = true,
                Recursive = true
            };

            using (var client = new AmazonSimpleSystemsManagementClient())
            {
                try
                {

                    var response = await client.GetParametersByPathAsync(request);
                    if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return response.Parameters.Select(x => x.Value).ToList();
                    }
                    else
                    {
                        throw new Exception("An error occured in getting value from Parameter Store, the remote service returned HTTP " + response.HttpStatusCode.ToString());
                    }
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
        }

        internal async Task LoadAllParamValues()
        {
            List<string> lstParamNames = new List<string>(
                new string[] {
                     DataConstants.ParamStoreJWTIssuer,
        DataConstants.ParamStoreJWTKey,
        DataConstants.ParamStoreSSLCertificateName,
        DataConstants.ParamStoreSSLCertificatePassword,
        DataConstants.ParamStoreSSLServicePort,
    });

            while (lstParamNames.Count > 0)
            {
                var request = new GetParametersRequest()
                {
                    Names = lstParamNames.Count > 10 ? lstParamNames.Take(10).ToList() : lstParamNames,
                    WithDecryption = true
                };

                using (var client = new AmazonSimpleSystemsManagementClient())
                {
                    try
                    {

                        var response = await client.GetParametersAsync(request);
                        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                        {
                            if (AppParams == null)
                            {
                                AppParams = new JObject();
                            }

                            foreach (var items in response.Parameters)
                            {
                                if (!AppParams.ContainsKey(items.Name))
                                {
                                    AppParams.Add(items.Name, items.Value);
                                }
                            }

                        }
                        else
                        {
                            throw new Exception("An error occured in getting value from Parameter Store, the remote service returned HTTP " + response.HttpStatusCode.ToString());
                        }
                    }
                    catch (Exception exception)
                    {
                        throw exception;
                    }
                }

                if (lstParamNames.Count > 10)
                {
                    lstParamNames.RemoveRange(0, 10);
                }
                else
                {
                    lstParamNames.Clear();
                }
            }

        }
    }
}
