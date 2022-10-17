using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Api.Service;
using InfluxDB.Client.Core.Flux.Domain;
using InfluxDB.Client.Linq;
using InfluxDB.Client.Writes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMFW.Algorithm;
using VMFW.DB.Service.Iservice;
using VMFW.Entity.InfluxDBEntity;
using VMFW.Operate;
using VMFW.Operate.OperateObj;

namespace VMFW.DB.Service.ServiceImpl
{
    public class PointInfoService : IPointInfoService
    {

        public async Task WriteDatasAsync(List<PointInfo> pDatas)
        {
            //构建数据
            List<PointData> pointDatas = new List<PointData>();
            pDatas.ForEach(data =>
            {
                var properties = data.GetType().GetProperties();
                //var dInfo = new PointData("pointinfo");
                var dInfo = PointData.Measurement("pointinfo");

                foreach (var property in properties)
                {
                    if (property.Name.Equals("WellName"))
                    {
                        dInfo = dInfo.Tag(property.Name, property.GetValue(data).ToString());
                    }
                    else if (property.Name.Equals("TimeStamp"))
                    {
                        dInfo = dInfo.Timestamp(DateTime.SpecifyKind((DateTime)(property.GetValue(data)), DateTimeKind.Utc), WritePrecision.Ns);
                    }
                    else
                    {
                        dInfo = dInfo.Field(property.Name, property.GetValue(data));
                    }
                }
                pointDatas.Add(dInfo);

            });

            //写入influxdb
            var client = new InfluxClient<PointInfo>();
            await client.WriteDatasAsync(pointDatas);
        }

        public async Task WriteDataAsync(PointInfo data)
        {

            var properties = data.GetType().GetProperties();
            //var dInfo = new PointData("pointinfo");
            var dInfo = PointData.Measurement("pointinfo");

            foreach (var property in properties)
            {
                if (property.Name.Equals("WellName"))
                {
                    dInfo = dInfo.Tag(property.Name, property.GetValue(data).ToString());
                }
                else if (property.Name.Equals("TimeStamp"))
                {
                    dInfo = dInfo.Timestamp(DateTime.SpecifyKind((DateTime)(property.GetValue(data)), DateTimeKind.Utc), WritePrecision.Ns);
                }
                else
                {
                    dInfo = dInfo.Field(property.Name, property.GetValue(data));
                }
            }

            //写入influxdb
            var client = new InfluxClient<PointInfo>();
            await client.WriteDataAsync(dInfo);
        }

        public async Task<PointInfo> GetOneRecord(string wellName, int year, int month, int day)
        {
            var client = new InfluxClient<PointInfo>();
            var queryable = client.GetQueryable();
            var query = (from s in queryable
                         where s.WellName == wellName
                         && s.year == year
                         && s.month == month
                         && s.day == day
                         //&& s.a1!= DBNull
                         && s.TimeStamp > DateTime.UtcNow.AddYears(-1)
                         orderby s.TimeStamp descending
                         select s);

            var pointInfo = await query
                .ToInfluxQueryable()
                .GetAsyncEnumerator()
                .Where(n => n.a1.HasValue)
                .FirstOrDefaultAsync();

            return pointInfo;

        }

        public async Task<PointInfo> GetMaxBDRecord(string wellName)
        {
            var client = new InfluxClient<PointInfo>();
            var queryable = client.GetQueryable();
            var query = (from s in queryable
                         where s.WellName == wellName
                         && s.year != 0
                         && s.month != 0
                         && s.day != 0
                         //&& s.a1!= DBNull
                         && s.TimeStamp > DateTime.UtcNow.AddYears(-1)
                         orderby s.TimeStamp descending
                         select s);

            var pointInfo = await query
                .ToInfluxQueryable()
                .GetAsyncEnumerator()
                .Where(n => n.a1.HasValue)
                .FirstOrDefaultAsync();


            return pointInfo;

        }

        public async Task<Dictionary<string, double>> ComputeHDayOutput(string wellName, int year, int month, int day)
        {
            var client = new InfluxClient<PointInfo>();
            #region  获取当前日实时产量
            var startDT = new DateTime(year, month, day, 0, 0, 0);
            var endDT = new DateTime(year, month, day, 23, 59, 59);
            var rangeS = GetUTCFormatString(startDT);
            var rangeE = GetUTCFormatString(endDT);
            var paramList = new List<string>() { "Dol", "Dwl", "Dgl", "Dl" };
            var retDic = new Dictionary<string, double>();
            foreach (var param in paramList)
            {
                var query = $"from(bucket: \"{client.GetBucket() }\")" +
               $"|> range(start:{rangeS},stop:{rangeE})" +
                 $"|> filter(fn:(r)=>r._measurement==\"pointinfo\" and r.WellName==\"{wellName}\" and r._field==\"{param}\" )" +
               "|> last()";
                var tables = await client.BuildClient().GetQueryApi().QueryAsync(query, client.GetOrg());
                if (tables.Count > 0 && tables[0].Records.Count > 0)
                {
                    var record = tables[0].Records[0];
                    retDic.Add(record.GetField().Replace("D", ""), (double)record.GetValue());
                }
            }
            return retDic;
            #endregion
        }

        public async Task<Dictionary<string, double>> ComputeHHEverageOutput(string wellName, int year, int month, int day)
        {
            var client = new InfluxClient<PointInfo>();
            #region  获取当前日实时产量
            var startDT = new DateTime(year, month, day, 0, 0, 0);
            var endDT = new DateTime(year, month, day, 23, 59, 59);
            var rangeS = GetUTCFormatString(startDT);
            var rangeE = GetUTCFormatString(endDT);
            var paramList = new List<string>() { "Dol", "Dwl", "Dgl", "Dl" };
            var retDic = new Dictionary<string, double>();
            foreach (var param in paramList)
            {
                var query = $"from(bucket: \"{client.GetBucket() }\")" +
               $"|> range(start:{rangeS},stop:{rangeE})" +
                 $"|> filter(fn:(r)=>r._measurement==\"pointinfo\" and r.WellName==\"{wellName}\" and r._field==\"{param}\" )" +
               "|> last()";
                var tables = await client.BuildClient().GetQueryApi().QueryAsync(query, client.GetOrg());
                if (tables.Count > 0 && tables[0].Records.Count > 0)
                {
                    var record = tables[0].Records[0];
                    var hour = (record.GetTimeInDateTime()?.ToLocalTime().Subtract(startDT).TotalHours).GetValueOrDefault();
                    retDic.Add(record.GetField().Replace("D", ""), (double)record.GetValue() / hour);
                }
            }
            return retDic;
            #endregion
        }

        public async Task<Dictionary<string, double>> ComputeIDayOutput(string wellName, int year, int month, int day)
        {
            var client = new InfluxClient<PointInfo>();
            #region  获取当前日实时产量
            var startDT = new DateTime(year, month, day, 0, 0, 0);
            var endDT = new DateTime(year, month, day, 23, 59, 59);
            var rangeS = GetUTCFormatString(startDT);
            var rangeE = GetUTCFormatString(endDT);
            var paramList = new List<string>() { "DolI", "DwlI", "DglI", "DlI" };
            var retDic = new Dictionary<string, double>();
            foreach (var param in paramList)
            {
                var query = $"from(bucket: \"{client.GetBucket() }\")" +
               $"|> range(start:{rangeS},stop:{rangeE})" +
                 $"|> filter(fn:(r)=>r._measurement==\"pointinfo\" and r.WellName==\"{wellName}\" and r._field==\"{param}\" )" +
               "|> last()";
                var tables = await client.BuildClient().GetQueryApi().QueryAsync(query, client.GetOrg());
                if (tables.Count > 0 && tables[0].Records.Count > 0)
                {
                    var record = tables[0].Records[0];
                    retDic.Add(record.GetField().Replace("D", "").Replace("I", ""), (double)record.GetValue());
                }
            }
            return retDic;
            #endregion
        }

        public async Task<Dictionary<string, double>> ComputeIHEverageOutput(string wellName, int year, int month, int day)
        {
            var client = new InfluxClient<PointInfo>();
            #region  获取当前日实时产量
            var startDT = new DateTime(year, month, day, 0, 0, 0);
            var endDT = new DateTime(year, month, day, 23, 59, 59);
            var rangeS = GetUTCFormatString(startDT);
            var rangeE = GetUTCFormatString(endDT);
            var paramList = new List<string>() { "DolI", "DwlI", "DglI", "DlI" };
            var retDic = new Dictionary<string, double>();
            foreach (var param in paramList)
            {
                var query = $"from(bucket: \"{client.GetBucket() }\")" +
               $"|> range(start:{rangeS},stop:{rangeE})" +
                 $"|> filter(fn:(r)=>r._measurement==\"pointinfo\" and r.WellName==\"{wellName}\" and r._field==\"{param}\" )" +
               "|> last()";
                var tables = await client.BuildClient().GetQueryApi().QueryAsync(query, client.GetOrg());
                if (tables.Count > 0 && tables[0].Records.Count > 0)
                {
                    var record = tables[0].Records[0];
                    var hour = (record.GetTimeInDateTime()?.ToLocalTime().Subtract(startDT).TotalHours).GetValueOrDefault();
                    retDic.Add(record.GetField().Replace("D", "").Replace("I", ""), (double)record.GetValue() / hour);
                }
            }
            return retDic;
            #endregion
        }

        //public async Task<List<double>> GetAverageOutput(string wellName, int year, int month, int day)
        //{
        //    var client = new InfluxClient<PointInfo>();
        //    //获取指定年月日当天的平均产量
        //    var startDT = new DateTime(year, month, day, 0, 0, 0);
        //    var endDT = new DateTime(year, month, day, 23, 59, 59);
        //    var rangeS = GetUTCFormatString(startDT);
        //    var rangeE = GetUTCFormatString(endDT);
        //    var sParam = new List<string>() { "Qol", "Qgl", "Qwl", "Ql" };
        //    var retList = new List<double>();

        //    var query = $"from(bucket: \"{client.GetBucket() }\")" +
        //    $"|> range(start:{rangeS},stop:{rangeE})" +
        //    $"|> filter(fn:(r)=>r._measurement==\"pointinfo\" and r.WellName==\"{wellName}\" and (r._field==\"{sParam[0]}\" or r._field==\"{sParam[1]}\" or r._field==\"{sParam[2]}\" or r._field==\"{sParam[3]}\"))" +
        //    "|> mean()";

        //    var tables = await client.BuildClient().GetQueryApi().QueryAsync(query, client.GetOrg());
        //    var records = tables.SelectMany(table => table.Records);
        //    retList.Add(Convert.ToDouble(records.Where(n => n.GetField().Equals(sParam[0])).Select(n => n.GetValue()).FirstOrDefault()));
        //    retList.Add(Convert.ToDouble(records.Where(n => n.GetField().Equals(sParam[1])).Select(n => n.GetValue()).FirstOrDefault()));
        //    retList.Add(Convert.ToDouble(records.Where(n => n.GetField().Equals(sParam[2])).Select(n => n.GetValue()).FirstOrDefault()));
        //    retList.Add(Convert.ToDouble(records.Where(n => n.GetField().Equals(sParam[3])).Select(n => n.GetValue()).FirstOrDefault()));

        //    return retList;
        //}

        private void FillEntity(ref PointInfo pInfo, OutputParam outputParam)
        {
            pInfo.rl = outputParam.rl;
            pInfo.r = outputParam.r;
            pInfo.x = outputParam.x;
            pInfo.ul = outputParam.ul;
            pInfo.u = outputParam.u;
            pInfo.E = outputParam.E;
            pInfo.C = outputParam.C;
            pInfo.a1 = outputParam.a1;
            pInfo.a2 = outputParam.a2;
            pInfo.a3 = outputParam.a3;
            pInfo.a4 = outputParam.a4;
            pInfo.Qol = outputParam.QO;
            pInfo.Qgl = outputParam.QG;
            pInfo.Qwl = outputParam.QW;
            pInfo.Ql = outputParam.QL;
            pInfo.QolI = outputParam.QOI;
            pInfo.QglI = outputParam.QGI;
            pInfo.QwlI = outputParam.QWI;
            pInfo.QlI = outputParam.QLI;
        }

        public List<PointInfo> FillIntervalPInfo(List<GroupPt> gPts, OutputParam outputParam)
        {
            List<PointInfo> pInfos = new List<PointInfo>();
            for (var i = 0; i < gPts.Count; i++)
            {
                PointInfo pInfo = new PointInfo();
                pInfo.TimeStamp = gPts[i].recDt;
                pInfo.WellName = gPts[i].WellName;
                var type = pInfo.GetType();
                var properities = type.GetProperties();
                int index = 2;
                for (var j = 0; j < gPts[i].GetCount() - 1; j++, index++)//gPts[i].GetCount() - 1，减去1，是因为读取数据的最后一个是单位切换按钮，该值不需要写入数据库
                {
                    if (properities[index].PropertyType == typeof(int))
                    {
                        properities[index].SetValue(pInfo, gPts[i].GetIntValue(j));
                    }
                    else if (properities[index].PropertyType == typeof(double))
                    {
                        properities[index].SetValue(pInfo, gPts[i].GetDoubleValue(j));
                    }
                    else
                    {
                        properities[index].SetValue(pInfo, gPts[i].GetNullableDoubleValue(j));
                    }
                }

                if (i == gPts.Count - 1)//最后一组数据需要插入计算数据
                {
                    FillEntity(ref pInfo, outputParam);
                    //this.FillEveryMiniteOutput(pInfo);
                }

                pInfos.Add(pInfo);

            }

            return pInfos;
        }

        /// <summary>
        /// 将dt时间转为UTC时间，然后返回yyyy-mm-ddThh:mm:ssZ格式的字符串
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private string GetUTCFormatString(DateTime dt)
        {
            var dtUTC = DateTime.SpecifyKind(dt.ToUniversalTime(), DateTimeKind.Utc);
            return $"{dtUTC.Year}-{dtUTC.Month.ToString("D2")}-{dtUTC.Day.ToString("D2")}T{dtUTC.Hour.ToString("D2")}:{dtUTC.Minute.ToString("D2")}:{dtUTC.Second.ToString("D2")}Z";
        }

        /// <summary>
        /// 获取指定日期第一条记录前的日产量，单位为 (吨)
        /// </summary>
        /// <param name="wellName"></param>
        /// <returns></returns>
        private async Task<Dictionary<string, double>> GetBFristRecordDayOutput(string wellName, DateTime dt)
        {
            var client = new InfluxClient<PointInfo>();
            var startDT = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
            var rangeS = GetUTCFormatString(startDT);
            var paramList = new List<string>() { "Qgl", "Qol", "Qwl", "Ql", /*"QglI", "QolI", "QwlI",*/ "QlI" };
            var query = $"from(bucket: \"{client.GetBucket() }\")" +
            $"|> range(start:{rangeS})" +
              $"|> filter(fn:(r)=>r._measurement==\"pointinfo\" and r.WellName==\"{wellName}\" and (r._field==\"{paramList[0]}\" or r._field==\"{paramList[1]}\" or r._field==\"{paramList[2]}\" or r._field==\"{paramList[3]}\"))" +
            "|> first()";
            var tables = await client.BuildClient().GetQueryApi().QueryAsync(query, client.GetOrg());

            var dic = new Dictionary<string, double>();
            foreach (var record in tables.SelectMany(table => table.Records))
            {
                //minites从0点到第一条记录的分钟数
                var minites = (record.GetTimeInDateTime()?.ToLocalTime().Subtract(startDT).TotalMinutes).GetValueOrDefault();
                dic.Add(record.GetField(), (double)record.GetValue() * minites / (24 * 60));
            }
            return dic;

        }

        public async Task<DateTime?> GetLastRecordTime(string wellName, string param)
        {
            var client = new InfluxClient<PointInfo>();
            var startDT = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            var rangeS = GetUTCFormatString(startDT);
            var query = $"from(bucket: \"{client.GetBucket() }\")" +
            $"|> range(start:{rangeS})" +
              $"|> filter(fn:(r)=>r._measurement==\"pointinfo\" and r.WellName==\"{wellName}\" and (r._field==\"{param}\"))" +
            "|> last()";
            var tables = await client.BuildClient().GetQueryApi().QueryAsync(query, client.GetOrg());

            var dic = new Dictionary<string, double>();
            var records = tables.SelectMany(table => table.Records);
            if (records.Count() > 0)
            {
                var record = records.FirstOrDefault();
                //minites从0点到第一条记录的分钟数
                return record.GetTimeInDateTime()?.ToLocalTime();
            }
            return null;
        }

        public async Task<DateTime?> GetFirstRecordTime(string wellName)
        {
            var client = new InfluxClient<PointInfo>();
            var startDT = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            var rangeS = GetUTCFormatString(startDT);
            var query = $"from(bucket: \"{client.GetBucket() }\")" +
            $"|> range(start:{rangeS})" +
              $"|> filter(fn:(r)=>r._measurement==\"pointinfo\" and r.WellName==\"{wellName}\")" +
            "|> first()";
            var tables = await client.BuildClient().GetQueryApi().QueryAsync(query, client.GetOrg());

            var dic = new Dictionary<string, double>();
            var records = tables.SelectMany(table => table.Records);
            if (records.Count() > 0)
            {
                var record = records.FirstOrDefault();
                //minites从0点到第一条记录的分钟数
                return record.GetTimeInDateTime()?.ToLocalTime();
            }
            return null;
        }

        /// <summary>
        /// 获取今天第一条记录前的日产量和时间，单位分别为 (吨)，分钟
        /// </summary>
        /// <param name="wellName"></param>
        /// <returns>返回的字典中，key为产量类型，values为（产量，时间）</returns>
        //private async Task<Dictionary<string, (double, double)>> GetBFristRecordDayOutputInfo(string wellName)
        //{
        //    var client = new InfluxClient<PointInfo>();
        //    var startDT = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
        //    var rangeS = GetUTCFormatString(startDT);
        //    var paramList = new List<string>() { "Qgl", "Qol", "Qwl", "Ql" };
        //    var query = $"from(bucket: \"{client.GetBucket() }\")" +
        //    $"|> range(start:{rangeS})" +
        //      $"|> filter(fn:(r)=>r._measurement==\"pointinfo\" and r.WellName==\"{wellName}\" and (r._field==\"{paramList[0]}\" or r._field==\"{paramList[1]}\" or r._field==\"{paramList[2]}\" or r._field==\"{paramList[3]}\"))" +
        //    "|> first()";
        //    var tables = await client.BuildClient().GetQueryApi().QueryAsync(query, client.GetOrg());

        //    var dic = new Dictionary<string, (double, double)>();
        //    foreach (var record in tables.SelectMany(table => table.Records))
        //    {
        //        //minites从0点到第一条记录的分钟数
        //        var minites = (record.GetTimeInDateTime()?.ToLocalTime().Subtract(startDT).TotalMinutes).GetValueOrDefault();
        //        dic.Add(record.GetField(), ((double)record.GetValue() * minites / (24 * 60), minites));
        //    }
        //    return dic;

        //}

        /// <summary>
        /// 计算实时日产量
        /// </summary>
        public async Task ComputeRealDayOutput(PointInfo pInfo)
        {
            var client = new InfluxClient<PointInfo>();
            #region  获取当前日实时产量
            var startDT = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            var rangeS = GetUTCFormatString(startDT);

            var dic = await GetBFristRecordDayOutput(pInfo.WellName, DateTime.Now);

            var paramList = new List<string>() { "Qgl", "Qol", "Qwl", "Ql", /*"QglI", "QolI", "QwlI",*/ "QlI" };
            foreach (var param in paramList)
            {
                var query = $"from(bucket: \"{client.GetBucket() }\")" +
               $"|> range(start:{rangeS})" +
                 $"|> filter(fn:(r)=>r._measurement==\"pointinfo\" and r.WellName==\"{pInfo.WellName}\" and r._field==\"{param}\" )" +
               "|> integral(unit: 1m)";
                var tables = await client.BuildClient().GetQueryApi().QueryAsync(query, client.GetOrg());
                if (tables.Count > 0 && tables[0].Records.Count > 0)
                {
                    var record = tables[0].Records[0];
                    var type = pInfo.GetType();
                    var firstValue = 0.0;
                    dic.TryGetValue(record.GetField(), out firstValue);
                    var value = (double)record.GetValue() / (24 * 60) + firstValue;
                    type.GetProperties().Where(p => p.Name.Equals(record.GetField().Replace('Q', 'D'))).FirstOrDefault()?.SetValue(pInfo, value);
                }
            }
            #endregion
        }

        public async Task ComputeRealMonthOutput(PointInfo pInfo)
        {
            var client = new InfluxClient<PointInfo>();
            #region  获取当前月实时产量
            var startDT = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
            var rangeS = GetUTCFormatString(startDT);
            var paramList = new List<string>() { "Dgl", "Dol", "Dwl", "Dl",/* "DglI", "DolI", "DwlI",*/ "DlI" };
            foreach (var param in paramList)
            {
                var query = $"from(bucket: \"{client.GetBucket() }\")" +
                $"|> range(start:{rangeS})" +
                $"|> filter(fn:(r)=>r._measurement==\"pointinfo\" and r.WellName==\"{pInfo.WellName}\" and r._field==\"{param}\")" +
                $"|> window(every:1d) |>last() |>group(columns: [\"{param}\"]) |> sum()";

                var tables = await client.BuildClient().GetQueryApi().QueryAsync(query, client.GetOrg());
                if (tables.Count > 0)
                {
                    var type = pInfo.GetType();
                    type.GetProperties().Where(n => n.Name.Equals(param.Replace('D', 'M'))).FirstOrDefault().SetValue(pInfo, tables[0].Records[0].GetValue());
                }
            }

            #endregion
        }

        public async Task ComputeRealYearOutput(PointInfo pInfo)
        {
            var client = new InfluxClient<PointInfo>();
            #region  获取当前年实时产量
            var startDT = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0);
            var rangeS = GetUTCFormatString(startDT);
            var paramList = new List<string>() { "Mgl", "Mol", "Mwl", "Ml", /*"MglI", "MolI", "MwlI",*/ "MlI" };
            foreach (var param in paramList)
            {
                var query = $"from(bucket: \"{client.GetBucket() }\")" +
                $"|> range(start:{rangeS})" +
                $"|> filter(fn:(r)=>r._measurement==\"pointinfo\" and r.WellName==\"{pInfo.WellName}\" and r._field==\"{param}\")" +
                $"|> window(every:1mo) |>last() |>group(columns: [\"{param}\"]) |> sum()";

                var tables = await client.BuildClient().GetQueryApi().QueryAsync(query, client.GetOrg());
                if (tables.Count > 0)
                {
                    var type = pInfo.GetType();
                    type.GetProperties().Where(n => n.Name.Equals(param.Replace('M', 'Y'))).FirstOrDefault()?.SetValue(pInfo, tables[0].Records[0].GetValue());
                }
            }
            #endregion
        }

        //private void FillEveryMiniteOutput(PointInfo pInfo)
        //{
        //    //pInfo.EMgl = pInfo.Qgl / 1440;
        //    //pInfo.EMol = pInfo.Qol / 1440;
        //    //pInfo.EMwl = pInfo.Qwl / 1440;
        //    //pInfo.EMl = pInfo.Ql / 1440;
        //    //pInfo.EMglI = pInfo.QglI / 1440;
        //    //pInfo.EMolI = pInfo.QolI / 1440;
        //    //pInfo.EMwlI = pInfo.QwlI / 1440;
        //    //pInfo.EMlI = pInfo.QlI / 1440;
        //}

        //public async Task ComputeEHourOutput(PointInfo pInfo)
        //{
        //    var client = new InfluxClient<PointInfo>();
        //    #region  获取每小时平均产量

        //    var query = $"from(bucket: \"{client.GetBucket() }\")" +
        //        $"|> range(start:-1h)" +
        //        $"|> filter(fn:(r)=>r._measurement==\"pointinfo\" and r.WellName==\"{pInfo.WellName}\" and (r._field==\"Dgl\" or r._field==\"Dol\" or r._field==\"Dwl\" or r._field==\"Dl\"))" +
        //        "|> mean()";

        //    var tables = await client.BuildClient().GetQueryApi().QueryAsync(query, client.GetOrg());
        //    foreach (var record in tables.SelectMany(table => table.Records))
        //    {
        //        var type = pInfo.GetType();
        //        type.GetProperties().Where(p => p.Name.Equals(record.GetField().Replace("D", "EH"))).FirstOrDefault()?.SetValue(pInfo, (double)record.GetValue() / 24);
        //    }

        //    #endregion
        //}

        //public async Task ComputeEDayOutput(PointInfo pInfo)
        //{
        //    var client = new InfluxClient<PointInfo>();
        //    #region  获取每天平均产量

        //    var query = $"from(bucket: \"{client.GetBucket() }\")" +
        //        $"|> range(start:-1d)" +
        //        $"|> filter(fn:(r)=>r._measurement==\"pointinfo\" and r.WellName==\"{pInfo.WellName}\" and (r._field==\"Dgl\" or r._field==\"Dol\" or r._field==\"Dwl\" or r._field==\"Dl\"))" +
        //        "|> mean()";

        //    var tables = await client.BuildClient().GetQueryApi().QueryAsync(query, client.GetOrg());
        //    foreach (var record in tables.SelectMany(table => table.Records))
        //    {
        //        var type = pInfo.GetType();
        //        type.GetProperties().Where(p => p.Name.Equals(record.GetField().Replace("D", "ED"))).FirstOrDefault()?.SetValue(pInfo, (double)record.GetValue());
        //    }

        //    #endregion
        //}

        public async Task<HourAverageOutput> ComputeWellHourAverageOutput(string wellName)
        {
            #region 方法一
            //var client = new InfluxClient<PointInfo>();
            //#region  获取当前日实时产量
            //var startDT = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            //var rangeS = GetUTCFormatString(startDT);

            //var dic = await GetBFristRecordDayOutput(wellName, DateTime.Now);

            //var paramList = new List<string>() { "Qgl", "Qol", "Qwl", "Ql" };
            //var ret = new HourAverageOutput();
            //foreach (var param in paramList)
            //{
            //    var query = $"from(bucket: \"{client.GetBucket() }\")" +
            //   $"|> range(start:{rangeS})" +
            //     $"|> filter(fn:(r)=>r._measurement==\"pointinfo\" and r.WellName==\"{wellName}\" and r._field==\"{param}\" )" +
            //   "|> integral(unit: 1m)";
            //    var tables = await client.BuildClient().GetQueryApi().QueryAsync(query, client.GetOrg());
            //    if (tables.Count > 0 && tables[0].Records.Count > 0)
            //    {
            //        var record = tables[0].Records[0];
            //        var type = ret.GetType();
            //        var firstValue = 0.0;
            //        dic.TryGetValue(record.GetField(), out firstValue);
            //        var value = 0.0;
            //        //var totalHour = record.GetTimeInDateTime()?.ToLocalTime().Subtract(startDT).TotalHours;
            //        var totalHour = (await GetLastRecordTime(wellName, param))?.Subtract(startDT).TotalHours;
            //        if (totalHour.HasValue)
            //        {
            //            value = ((double)record.GetValue() / (24 * 60) + firstValue) / (totalHour.Value);
            //        }
            //        type.GetProperties()
            //            .Where(p => p.Name.Equals(record.GetField().Replace("Q", "ED")))
            //            .FirstOrDefault()?.SetValue(ret, value);
            //    }
            //}
            //return ret;
            //#endregion
            #endregion 方法一

            #region 方法二
            var dt = DateTime.Now;
            var hDay = await ComputeHHEverageOutput(wellName, dt.Year, dt.Month, dt.Day);
            var iDay = await ComputeIHEverageOutput(wellName, dt.Year, dt.Month, dt.Day);
            var ret = new HourAverageOutput();
            ret.SetHOutput(hDay);
            ret.SetIOutput(iDay);
            return ret;
            #endregion 方法二
        }

        public async Task<DayAverageOutput> ComputeWellDayAverageOutput(string wellName)
        {
            #region 方法一
            //var client = new InfluxClient<PointInfo>();
            //#region  获取当前月至前一天的实时累计产量
            //var startDT = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
            //var endDT = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            //var days = DateTime.Now.Day;
            //if (DateTime.Now.Day != 1)
            //{
            //    endDT = endDT.AddDays(-1);
            //    days = days - 1;
            //}
            //var rangeS = GetUTCFormatString(startDT);
            //var rangeE = GetUTCFormatString(endDT);
            //var paramList = new List<string>() { "Dgl", "Dol", "Dwl", "Dl", "DglI", "DolI", "DwlI", "DlI" };
            //var ret = new DayAverageOutput();
            //foreach (var param in paramList)
            //{
            //    var query = $"from(bucket: \"{client.GetBucket() }\")" +
            //    $"|> range(start:{rangeS},stop:{rangeE})" +
            //    $"|> filter(fn:(r)=>r._measurement==\"pointinfo\" and r.WellName==\"{wellName}\" and r._field==\"{param}\")" +
            //    $"|> window(every:1d) |>last() |>group(columns: [\"{param}\"]) |> sum()";

            //    var tables = await client.BuildClient().GetQueryApi().QueryAsync(query, client.GetOrg());
            //    if (tables.Count > 0)
            //    {
            //        var type = ret.GetType();
            //        type.GetProperties().Where(n => n.Name.Equals(param.Replace("D", "ED"))).FirstOrDefault().SetValue(ret, (double)tables[0].Records[0].GetValue() / days);
            //    }
            //}
            //return ret;
            //#endregion
            #endregion 方法一

            #region 方法二
            var client = new InfluxClient<PointInfo>();
            //获取当前一天的月计产量
            var day = DateTime.Now.Day;
            var ret = new DayAverageOutput();
            if (DateTime.Now.Day == 1)
            {
                return ret;
            }
            var dt = DateTime.Now;
            day = day - 1;
            var startDT = new DateTime(dt.Year, dt.Month, 1, 0, 0, 0);
            var endDT = new DateTime(dt.Year, dt.Month, day, 23, 59, 59);

            endDT = endDT.AddDays(-1);
            var rangeS = GetUTCFormatString(startDT);
            var rangeE = GetUTCFormatString(endDT);

            var query = $"from(bucket: \"{client.GetBucket() }\")" +
                $"|> range(start:{rangeS},stop:{rangeE})" +
                 $"|> filter(fn:(r)=>r._measurement==\"pointinfo\" and r.WellName==\"{wellName}\" and (r._field==\"Mgl\" or r._field==\"Mol\" or r._field==\"Mwl\" or r._field==\"Ml\" or r._field==\"MglI\" or r._field==\"MolI\" or r._field==\"MwlI\" or r._field==\"MlI\"))" +
                "|> last()";

            var tables = await client.BuildClient().GetQueryApi().QueryAsync(query, client.GetOrg());
            foreach (var record in tables.SelectMany(table => table.Records))
            {

                ret.GetType().GetProperties().Where(p => p.Name.Equals(record.GetField().Replace("M", "ED"))).FirstOrDefault()?.SetValue(ret, (double)record.GetValue() / day);
            }
            return ret;
            #endregion 方法二
        }

        public async Task ComputeTotalOutput(PointInfo pInfo)
        {
            var client = new InfluxClient<PointInfo>();
            #region  获取该口井的第一条记录的时间并以该条时间的元旦作为查询计算的起始时间
            var dt = await GetFirstRecordTime(pInfo.WellName);
            if (!dt.HasValue)
            {
                return;
            }
            var startDT = new DateTime(dt.Value.Year, 1, 1, 0, 0, 0);

            var rangeS = GetUTCFormatString(startDT);
            var paramList = new List<string>() { "Ygl", "Yol", "Ywl", "Yl"/*, "YglI", "YolI", "YwlI"*/, "YlI" };
            //var ret = new DayAverageOutput();
            foreach (var param in paramList)
            {
                var query = $"from(bucket: \"{client.GetBucket() }\")" +
                $"|> range(start:{rangeS})" +
                $"|> filter(fn:(r)=>r._measurement==\"pointinfo\" and r.WellName==\"{pInfo.WellName}\" and r._field==\"{param}\")" +
                $"|> window(every:1y) |>last() |>group(columns: [\"{param}\"]) |> sum()";

                var tables = await client.BuildClient().GetQueryApi().QueryAsync(query, client.GetOrg());
                if (tables.Count > 0)
                {
                    var type = pInfo.GetType();
                    type.GetProperties().Where(n => n.Name.Equals(param.Replace("Y", "T"))).FirstOrDefault().SetValue(pInfo, (double)tables[0].Records[0].GetValue());
                }
            }

            #endregion
        }

        //public async Task<double> GetAverageDensity(string wellName, int year, int month, int day)
        //{
        //    var client = new InfluxClient<PointInfo>();
        //    var startDT = new DateTime(year, month, day, 0, 0, 0);
        //    var endDT = new DateTime(year, month, day, 23, 59, 59);

        //    var rangeS = GetUTCFormatString(startDT);
        //    var rangeE = GetUTCFormatString(endDT);

        //    var query = $"from(bucket: \"{client.GetBucket() }\")" +
        //        $"|> range(start:{rangeS},stop:{rangeE})" +
        //        $"|> filter(fn:(r)=>r._measurement==\"pointinfo\" and r.WellName==\"{wellName}\" and r._field==\"rl\")" +
        //        $"|>  mean()";

        //    var tables = await client.BuildClient().GetQueryApi().QueryAsync(query, client.GetOrg());
        //    if (tables.Count > 0)
        //    {
        //        return (double)tables[0].Records[0].GetValue();
        //    }
        //    return 0.0;
        //}
    }
}

