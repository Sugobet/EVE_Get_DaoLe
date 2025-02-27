﻿

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EveMagic.Data.Ocr
{
    public interface ICloudOcr
    {
        public string Request(byte[] byteFile);
    }


    public enum CloudOcrType
    {
        IinsideOcr,
        EveMagicOcr,
        CnOcr,
    }


    public class CloudOcr: IDisposable
    {
        HttpClient _client;
        ICloudOcr _ocr;


        public CloudOcr(HttpClient httpClient, ICloudOcr ocr)
        {
            this._client = httpClient;
            this._ocr = ocr;
        }


        public string GetResponse(string fileName)
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                return "NotInternet";
            }

            if (File.Exists(fileName))
            {
                byte[] data = File.ReadAllBytes(fileName);
                string res = this._ocr.Request(data);
                if (res == "")
                {
                    return "NotResponse";
                }
                return res;
            }

            return "FileNotFound";
        }

        public JToken GetResponse(byte[] fileByte)
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                return null;
            }

            string res = this._ocr.Request(fileByte);

            if (res == "")
            {
                return null;
            }

            JObject json = (JObject)JsonConvert.DeserializeObject(res);
            
            return json["results"];
        }


        public void Dispose()
        {
            this._client.Dispose();
            this._client = null;
        }
    }
}
