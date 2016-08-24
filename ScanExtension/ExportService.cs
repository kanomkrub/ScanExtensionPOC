using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ScanExtension
{
    public static class ExportService
    {
        public static void Export(string filePath, string token, string mediaType, string docType, Dictionary<string,string> properties, string serviceUri, string cabId, string folderId)
        {
            //var serviceUri = batch.ExportUri;
            //var cabId = batch.ExportRepository;
            //var folderId = batch.ExportFolder;
            var userName = "weblogic2";
            var password = "password";
            serviceUri = "http://192.168.1.71:1605/rest/cmis/browser";
            var uriCreate = string.Format("{0}/root?objectId={1}", cabId, folderId);
            var docName = Path.GetFileName(filePath);
            var bytes = File.ReadAllBytes(filePath);
            HttpClient client = new HttpClient() { BaseAddress = new Uri(serviceUri) };
            var basicAuthenValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(userName + ":" + password));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuthenValue);
            var values = new List<KeyValuePair<string,string>>
                    {
                        //new KeyValuePair<string, string>("token", token),
                        new KeyValuePair<string, string>("cmisaction", "createDocument"),
                        new KeyValuePair<string, string>("propertyId[0]", "cmis:name"),
                        new KeyValuePair<string, string>("propertyValue[0]", docName),
                        new KeyValuePair<string, string>("propertyId[1]", "cmis:objectTypeId"),
                        new KeyValuePair<string, string>("propertyValue[1]", "D:"+docType),
                    };

            for(int i =0;i < properties.Count; i++)
            {
                var propertyName = new KeyValuePair<string, string>(string.Format("propertyId[{0}]", i + 2), "kk:metaData:" + properties.Keys.ElementAt(i));
                var propertyValue = new KeyValuePair<string, string>(string.Format("propertyValue[{0}]", i + 2), properties.Values.ElementAt(i));
                values.Add(propertyName);
                values.Add(propertyValue);
            }

            var content = new MultipartFormDataContent();
            foreach (var keyValuePair in values)
            {
                HttpContent c = new StringContent(keyValuePair.Value);
                c.Headers.Remove("Content-Type");
                content.Add(c, string.Format("\"{0}\"", keyValuePair.Key));
            }
            var fileContent = new ByteArrayContent(bytes);
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = string.Format("\"{0}\"", "content"),
                FileName = string.Format("\"{0}\"", docName)
            };
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
            content.Add(fileContent);

            var result = client.PostAsync(uriCreate, content).Result.Content;
            var s = result.ReadAsStringAsync().Result;
        }
    }
}
