using System;
using System.Collections.Generic;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.IO;
using System.Data.OleDb;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web.Script.Serialization;

//run this command in the NuGet Package Manager Console
//Install-Packages Microsoft.IdentityModel.Clients.ActiveDirectory -Version 2.21.301221612

namespace PBIIncrementalLoadingTest
{
    class pbi
    {
        private DataSet dsIncrementalData = new DataSet();
        private StringBuilder[] JSONData = new StringBuilder[1]; //create an array with length 1. will redim later on in code.

        static void Main(string[] args)
        {
            pbi self = new pbi();
            //Add using Microsoft.IdentityModel.Clients.ActiveDirectory

            dataset[] datasets = self.GetDatasets();
            foreach (dataset d in datasets)
            {
                Console.WriteLine(d.Name);
            }
            string datasetID = (from d in datasets where d.Name == "Orders" select d).FirstOrDefault().Id;

            //The client id that Azure AD created when you registered your client app.
            string clientID = "204e31d2-1668-4775-98d1-80ed24588aed";

            //RedirectURI you used when you registered your app.
            //For a client app, a redirect uri gives Azure AD more details on the application that it will authenticate.
            //You can use this redirect url for your client app
            string redirectUri = "https://www.blue-granite.com";

            //Resource Uri for Power BI API
            string resourceUri = "https://analysis.windows.net/powerbi/api";

            //OAuth2 authority Uri
            string authorityUri = "https://login.windows.net/common/oauth2/authorize";

            string tableName = "Orders";
            string powerBIApiURl = String.Format("https://api.powerbi.com/v1.0/myorg/datasets/{0}/tables/{1}/rows", datasetID, tableName);

            //Get access token:
            //To call a Power BI REST operation, create an instance of AuthenticationContext and call AcquireToken
            //Authentication context is part of the Active Directory Authentication Library NuGet package

            //AcquireToken will acquire a new Azure access token
            //Call AcquireToken to get an Azure token from Azure Active Directory token issuance endpoint
            AuthenticationContext authContext = new AuthenticationContext(authorityUri);
            string token = authContext.AcquireToken(resourceUri, clientID, new Uri(redirectUri), PromptBehavior.Auto).AccessToken;

            //TODO - loop through dataset and build JSON string of 10000 rows
            //TODO - submit to PowerBI via POST
            //TODO - wait for response
            //TODO - loop through next 10000 rows (limit of 10000 rows per JSON request)

            //Console.WriteLine(token);

            Console.WriteLine(datasetID);
            Console.ReadLine();
            //self.GetIncrementalData();

            //if (self.dsIncrementalData.Tables[0].Rows.Count > 0)
            //{
            //    int batchCount = (int)Math.Ceiling((double)self.dsIncrementalData.Tables[0].Rows.Count / 10000);
            //    self.JSONData = new StringBuilder[batchCount];
            //    self.BuildJSONString(self, batchCount, self.dsIncrementalData.Tables[0].Columns.Count);

            //    Console.WriteLine("There are " + self.JSONData.Count() + " items to send to Power BI in the JSON array");
            //    Console.ReadLine();
            //    foreach (StringBuilder s in self.JSONData)
            //    {
            //        Console.WriteLine("---------------------------------------------");
            //        Console.WriteLine(s);
            //        Console.WriteLine("---------------------------------------------");
            //    }

            //    //start pushing rows to PowerBI
            //    HttpWebRequest request = System.Net.WebRequest.Create(powerBIApiURl) as System.Net.HttpWebRequest;
            //    request.KeepAlive = true;
            //    request.Method = "POST";
            //    request.ContentLength = 0;
            //    request.ContentType = "application/json";
            //    request.Headers.Add("Authorization", String.Format("Bearer {0}", token));

            //    Console.WriteLine(token);
            //    Console.ReadLine();

            //    byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(self.JSONData[0].ToString());
            //    request.ContentLength = byteArray.Length;

            //    Console.WriteLine(request.GetRequestStream());
            //    Console.ReadLine();

            //    using (Stream writer = request.GetRequestStream())
            //    {
            //        writer.Write(byteArray, 0, byteArray.Length);

            //        var response = (HttpWebResponse)request.GetResponse();
            //        Console.WriteLine(response.ToString());
            //    }
            //}
            //else //nothing to do
            //{

            //}

            //Console.WriteLine(self.dsIncrementalData.Tables[0].Rows.Count);
            Console.ReadLine();
        }

        public dataset[] GetDatasets()
        {
            //This is sample code to illustrate a Power BI operation. 
            //In a production application, refactor code into specific methods and use appropriate exception handling.

            //The client id that Azure AD creates when you register your client app.
            //  To learn how to register a client app, see https://msdn.microsoft.com/en-US/library/dn877542(Azure.100).aspx         
            string clientID = "204e31d2-1668-4775-98d1-80ed24588aed";

            //RedirectUri you used when you register your app.
            //For a client app, a redirect uri gives Azure AD more details on the application that it will authenticate.
            // You can use this redirect uri for your client app
            string redirectUri = "https://www.blue-granite.com";

            //Resource Uri for Power BI API
            string resourceUri = "https://analysis.windows.net/powerbi/api";

            //OAuth2 authority Uri
            string authorityUri = "https://login.windows.net/common/oauth2/authorize";

            string powerBIApiUrl = "https://api.powerbi.com/v1.0/myorg/datasets";

            //Get access token: 
            // To call a Power BI REST operation, create an instance of AuthenticationContext and call AcquireToken
            // AuthenticationContext is part of the Active Directory Authentication Library NuGet package
            // To install the Active Directory Authentication Library NuGet package in Visual Studio, 
            //  run "Install-Package Microsoft.IdentityModel.Clients.ActiveDirectory" from the nuget Package Manager Console.

            // AcquireToken will acquire an Azure access token
            // Call AcquireToken to get an Azure token from Azure Active Directory token issuance endpoint
            AuthenticationContext authContext = new AuthenticationContext(authorityUri);
            string token = authContext.AcquireToken(resourceUri, clientID, new Uri(redirectUri)).AccessToken;

            //GET web request to list all datasets.
            //To get a datasets in a group, use the Groups uri: https://api.powerbi.com/v1.0/myorg/groups/{group_id}/datasets
            HttpWebRequest request = System.Net.WebRequest.Create(powerBIApiUrl) as System.Net.HttpWebRequest;
            request.KeepAlive = true;
            request.Method = "GET";
            request.ContentLength = 0;
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", String.Format("Bearer {0}", token));

            //Get HttpWebResponse from GET request
            using (HttpWebResponse httpResponse = request.GetResponse() as System.Net.HttpWebResponse)
            {
                //Get StreamReader that holds the response stream
                using (StreamReader reader = new System.IO.StreamReader(httpResponse.GetResponseStream()))
                {
                    string responseContent = reader.ReadToEnd();

                    JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                    Datasets datasets = (Datasets)jsonSerializer.Deserialize(responseContent, typeof(Datasets));

                    return datasets.value;
                }
            }
        }

        //fills a private dataset with incremental data rows to be loaded
        //to the Power BI Data Set
        private void GetIncrementalData()
        {
            OleDbConnection conData = new OleDbConnection("Provider=SQLNCLI11;Server=localhost;Database=AdventureWorks2014;Integrated Security=SSPI;");
            OleDbCommand cmdData = new OleDbCommand("SELECT * FROM New_Orders;");

            OleDbDataAdapter daData = new OleDbDataAdapter(cmdData);
            conData.Open();
            cmdData.Connection = conData;
            daData.Fill(dsIncrementalData);
            conData.Close();
        }

        public void BuildJSONString(pbi self, int batchCount, int columnCount)
        {
            //int i = 0;
            int maxBatchSize = 9999;
            
            for (int a = 0; a < self.JSONData.Count(); a++) //loop through each batch and build a JSON string to pop to the array
            {
                StringBuilder JSONRows = new StringBuilder(); //initialize a new StringBuilder for the batch
                JSONRows.Append("{\"rows\": [");
                    for (int i = 0; i <= maxBatchSize && i < self.dsIncrementalData.Tables[0].Rows.Count; i++)
                    {
                        //intialize the rows record
                        JSONRows.Append("{");

                        //loop through records of the dataset and build the JSON rows object
                        for (int c = 0; c < columnCount; c++)
                        {
                            string columnName = self.dsIncrementalData.Tables[0].Columns[c].ColumnName;
                            if (c > 0) { JSONRows.Append(","); } //append a comma between fields if not on the first row of the batch
                            JSONRows.Append("\"" + columnName + "\":"); //append the column name to the JSON string
                            JSONRows.Append("\"" + self.dsIncrementalData.Tables[0].Rows[i].ItemArray[c] + "\""); //append the column value to the JSON string
                        }

                        //close the row
                        JSONRows.Append("}");

                        //add a comma if not the last row
                        if (i < self.dsIncrementalData.Tables[0].Rows.Count - 1 && i < 9999) { JSONRows.Append(","); }
                        //add the rows objects to the JSONData array  
                    }
                JSONRows.Append("]}");
                //Console.WriteLine(JSONRows.ToString());
                self.JSONData[a] = JSONRows;
            }
        }


        public class Datasets
        {
            public dataset[] value { get; set; }
        }

        public class dataset
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public class Tables
        {
            public table[] value { get; set; }
        }

        public class table
        {
            public string Name { get; set; }
        }

        public class Groups
        {
            public group[] value { get; set; }
        }

        public class group
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}
