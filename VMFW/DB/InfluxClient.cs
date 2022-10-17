using InfluxDB.Client;
using InfluxDB.Client.Linq;
using InfluxDB.Client.Writes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMFW.Entity.InfluxDBEntity;

namespace VMFW.DB
{
    public class InfluxClient<T>
    {
        private InfluxDBClient influxdbClient;
        private string token;
        private string bucket;
        private string org;
        private string url;
        private string username;
        private string password;

        public InfluxClient()
        {
            token = ConfigurationManager.AppSettings.Get("influxToken");
            bucket = ConfigurationManager.AppSettings.Get("influxBucket");
            org = ConfigurationManager.AppSettings.Get("influxOrg");
            url = ConfigurationManager.AppSettings.Get("influxUrl");
            username = ConfigurationManager.AppSettings.Get("influxUsername");
            password = ConfigurationManager.AppSettings.Get("influxPassword");
        }

        public InfluxDBClient BuildClient()
        {
            if (influxdbClient == null)
            {
                influxdbClient = InfluxDBClientFactory.Create(url, token);
            }

            return influxdbClient;
        }

        public string GetBucket()
        {
            return this.bucket;
        }

        public string GetOrg()
        {
            return this.org;
        }

        public async Task<bool> WriteDatasAsync(List<PointData> pDatas)
        {
            try
            {
                using (var client = BuildClient())
                {
                    var writeApi = client.GetWriteApiAsync();
                    await writeApi.WritePointsAsync(pDatas, bucket, org);
                }
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }

        public async Task<bool> WriteDataAsync(PointData pData)
        {
            try
            {
                using (var client = BuildClient())
                {
                    var writeApi = client.GetWriteApiAsync();
                    await writeApi.WritePointAsync(pData, bucket, org);
                }
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }

        public InfluxDBQueryable<T> GetQueryable()
        {
            return InfluxDBQueryable<T>.Queryable(bucket, org, this.BuildClient().GetQueryApi());
        }


        public InfluxDBQueryable<T> GetQueryableSync()
        {
            return InfluxDBQueryable<T>.Queryable(bucket, org, this.BuildClient().GetQueryApiSync());
        }
    }
}
