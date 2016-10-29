// copyright (c) 2014 Sayyid Mohammad Saleh Samimi
// Donate: 1NBakuExebh2M9atfS3QuSmRPPtYU398VN
// 
// this file is part of BTCETradeBot.
// 
// BTCETradeBot is free software: you can redistribute it and/or modify
// it under the terms of the gnu general public license as published by
// the free software foundation, either version 3 of the license, or
// (at your option) any later version.
// 
// BTCETradeBot is distributed in the hope that it will be useful,
// but without any warranty; without even the implied warranty of
// merchantability or fitness for a particular purpose.  see the
// gnu general public license for more details.
// 
// you should have received a copy of the gnu general public license
// along with BTCETradeBot.  if not, see <http://www.gnu.org/licenses/>.

using System.Threading;
using BtcE.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Windows.Forms;
namespace BtcE
{
    public class Filtering
    {
        public enum RequestModes
        {
            SimpleHttpRequest,
            WebBrowser,
            SmoothHttpRequest
        }
        public static RequestModes AppRequestMode;
    }

  
    class WebApi
    {
        public static string Query(string url)
        {
            if (Filtering.AppRequestMode == Filtering.RequestModes.WebBrowser)
            {
                var sms = new SMSRequestWebBrowser();
                return sms.GetResponse(url);
                
            }
            else if(Filtering.AppRequestMode==Filtering.RequestModes.SmoothHttpRequest)
            {
                var sms = new SMSRequestSmoothHttpRequest();
                return sms.GetResponse(url);
            }
            else //SimpleHTTPRequest
            {
                var sms = new SMSSimpleHttpRequest();
                return sms.GetResponse(url);
            }
        }
    }

    public class BtceApi
    {
        readonly string _key;
        readonly HMACSHA512 _hashMaker;
        public static UInt32 Nonce = UnixTime.Now;
        Random _random = new Random();

        public BtceApi(string key, string secret)
        {
            this._key = key;
            _hashMaker = new HMACSHA512(Encoding.ASCII.GetBytes(secret));
        }

        private static uint _lastGetNonce;
        public static UInt32 GetNonce()
        {
            while (UnixTime.Now - _lastGetNonce <= 1);
            _lastGetNonce = UnixTime.Now;
            Nonce = UnixTime.Now;
            return Nonce;
        }

        string Query(Dictionary<string, string> args)
        {
            var nonce = GetNonce().ToString();
            args.Add("nonce", nonce);
            var dataStr = BuildPostData(args);
            var data = Encoding.ASCII.GetBytes(dataStr);

            if (Filtering.AppRequestMode == Filtering.RequestModes.WebBrowser)
            {
                var headers = string.Format("key: {0}\r\nSign: {1}\r\n", _key, ByteArrayToString(_hashMaker.ComputeHash(data)).ToLower());
                var sms = new SMSRequestWebBrowser();
                return sms.GetResponse("https://btc-e.com/tapi", data, headers);
            }
            else if (Filtering.AppRequestMode == Filtering.RequestModes.SmoothHttpRequest)
            {
                var collection = new WebHeaderCollection { { "Key", _key }, { "Sign", ByteArrayToString(_hashMaker.ComputeHash(data)).ToLower() } };
                var sms = new SMSRequestSmoothHttpRequest();
                return sms.GetResponse("https://btc-e.com/tapi", data, collection);
            }
            else //SimpleHttpRequest
            {
                var collection = new WebHeaderCollection { { "Key", _key }, { "Sign", ByteArrayToString(_hashMaker.ComputeHash(data)).ToLower() } };
                var sms = new SMSSimpleHttpRequest();
                return sms.GetResponse("https://btc-e.com/tapi", data, collection);
            }
        }

        public UserInfo GetInfo()
        {
            var resultStr = Query(new Dictionary<string, string>()
            {
                { "method", "getInfo" }
            });
            var result = JObject.Parse(resultStr);
            if (result.Value<int>("success") == 0)
                throw new Exception(result.Value<string>("error"));
            return UserInfo.ReadFromJObject(result["return"] as JObject);
        }

        public TransHistory GetTransHistory(
            int? from = null,
            int? count = null,
            int? fromId = null,
            int? endId = null,
            bool? orderAsc = null,
            DateTime? since = null,
            DateTime? end = null
            )
        {
            var args = new Dictionary<string, string>()
            {
                { "method", "TransHistory" }
            };

            if (from != null) args.Add("from", from.Value.ToString());
            if (count != null) args.Add("count", count.Value.ToString());
            if (fromId != null) args.Add("from_id", fromId.Value.ToString());
            if (endId != null) args.Add("end_id", endId.Value.ToString());
            if (orderAsc != null) args.Add("order", orderAsc.Value ? "ASC" : "DESC");
            if (since != null) args.Add("since", UnixTime.GetFromDateTime(since.Value).ToString());
            if (end != null) args.Add("end", UnixTime.GetFromDateTime(end.Value).ToString());
            var result = JObject.Parse(Query(args));
            if (result.Value<int>("success") == 0)
                throw new Exception(result.Value<string>("error"));
            return TransHistory.ReadFromJObject(result["return"] as JObject);
        }

        public TradeHistory GetTradeHistory(
            int? from = null,
            int? count = null,
            int? fromId = null,
            int? endId = null,
            bool? orderAsc = null,
            DateTime? since = null,
            DateTime? end = null
            )
        {
            var args = new Dictionary<string, string>()
            {
                { "method", "TradeHistory" }
            };

            if (from != null) args.Add("from", from.Value.ToString());
            if (count != null) args.Add("count", count.Value.ToString());
            if (fromId != null) args.Add("from_id", fromId.Value.ToString());
            if (endId != null) args.Add("end_id", endId.Value.ToString());
            if (orderAsc != null) args.Add("order", orderAsc.Value ? "ASC" : "DESC");
            if (since != null) args.Add("since", UnixTime.GetFromDateTime(since.Value).ToString());
            if (end != null) args.Add("end", UnixTime.GetFromDateTime(end.Value).ToString());

            var result = JObject.Parse(Query(args));
            if (result.Value<int>("success") == 0)
                //throw new Exception(result.Value<string>("error"));
                return null;
            return TradeHistory.ReadFromJObject(result["return"] as JObject);
        }


        public OrderList GetOrderList(
            int? from = null,
            int? count = null,
            int? fromId = null,
            int? endId = null,
            bool? orderAsc = null,
            DateTime? since = null,
            DateTime? end = null,
            BtcePair? pair = null,
            bool? active = null
            )
        {
            var args = new Dictionary<string, string>()
            {
                { "method", "OrderList" }
            };

            if (from != null) args.Add("from", from.Value.ToString());
            if (count != null) args.Add("count", count.Value.ToString());
            if (fromId != null) args.Add("from_id", fromId.Value.ToString());
            if (endId != null) args.Add("end_id", endId.Value.ToString());
            if (orderAsc != null) args.Add("order", orderAsc.Value ? "ASC" : "DESC");
            if (since != null) args.Add("since", UnixTime.GetFromDateTime(since.Value).ToString());
            if (end != null) args.Add("end", UnixTime.GetFromDateTime(end.Value).ToString());
            if (pair != null) args.Add("pair", BtcePairHelper.ToString(pair.Value));
            if (active != null) args.Add("active", active.Value ? "1" : "0");
            var result = JObject.Parse(Query(args));
            if (result.Value<int>("success") == 0)
                throw new Exception(result.Value<string>("error"));
            return OrderList.ReadFromJObject(result["return"] as JObject);
        }

        public OrderList GetActiveOrders( BtcePair? pair = null )
        {
            var args = new Dictionary<string, string>()
            {
                { "method", "ActiveOrders" }
            };

            if (pair != null) args.Add("pair", BtcePairHelper.ToString(pair.Value));
            var result = JObject.Parse(Query(args));
            if (result.Value<int>("success") == 0)
                throw new Exception(result.Value<string>("error"));
            return OrderList.ReadFromJObject(result["return"] as JObject);
        }

        public TradeAnswer Trade(BtcePair pair, TradeType type, decimal rate, decimal amount)
        {
            var args = new Dictionary<string, string>()
            {
                { "method", "Trade" },
                { "pair", BtcePairHelper.ToString(pair) },
                { "type", TradeTypeHelper.ToString(type) },
                { "rate", DecimalToString(rate) },
                { "amount", DecimalToString(amount) }
            };
            var result = JObject.Parse(Query(args));
            if (result.Value<int>("success") == 0)
                throw new Exception(result.Value<string>("error"));
            return TradeAnswer.ReadFromJObject(result["return"] as JObject);
        }

        public CancelOrderAnswer CancelOrder(int orderId)
        {
            var args = new Dictionary<string, string>()
            {
                { "method", "CancelOrder" },
                { "order_id", orderId.ToString() }
            };
            var result = JObject.Parse(Query(args));
            if (result.Value<int>("success") == 0)
                throw new Exception(result.Value<string>("error"));
            return CancelOrderAnswer.ReadFromJObject(result["return"] as JObject);
        }

        static string ByteArrayToString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }

        static string BuildPostData(Dictionary<string, string> d)
        {
            StringBuilder s = new StringBuilder();
            foreach (var item in d)
            {
                s.AppendFormat("{0}={1}", item.Key, HttpUtility.UrlEncode(item.Value));
                s.Append("&");
            }
            if (s.Length > 0) s.Remove(s.Length - 1, 1);
            return s.ToString();
        }

        static string DecimalToString(decimal d)
        {
            return d.ToString(CultureInfo.InvariantCulture);
        }
        public static Depth GetDepth(BtcePair pair)
        {
            string queryStr = string.Format("https://btc-e.com/api/2/{0}/depth", BtcePairHelper.ToString(pair));
            try
            {
                return Depth.ReadFromJObject(JObject.Parse(WebApi.Query(queryStr)));
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public static Ticker GetTicker(BtcePair pair)
        {
            string queryStr = string.Format("https://btc-e.com/api/2/{0}/ticker", BtcePairHelper.ToString(pair));
            return Ticker.ReadFromJObject(JObject.Parse(WebApi.Query(queryStr))["ticker"] as JObject);
        }
        public static List<TradeInfo> GetTrades(BtcePair pair)
        {
            string queryStr = string.Format("https://btc-e.com/api/2/{0}/trades", BtcePairHelper.ToString(pair));
            return JArray.Parse(WebApi.Query(queryStr)).OfType<JObject>().Select(TradeInfo.ReadFromJObject).ToList();
        }
        public static decimal GetFee(BtcePair pair)
        {
            string queryStr = string.Format("https://btc-e.com/api/2/{0}/fee", BtcePairHelper.ToString(pair));
            return JObject.Parse(WebApi.Query(queryStr)).Value<decimal>("trade");
        }
    }

    public class BtceApiV3
    {
        private static string MakePairListString(BtcePair[] pairlist)
        {
            return string.Join("-", pairlist.Select(x => BtcePairHelper.ToString(x)).ToArray());
        }

        private static string Query(string method, BtcePair[] pairlist, Dictionary<string, string> args = null)
        {
            var pairliststr = MakePairListString(pairlist);
            StringBuilder sb = new StringBuilder();
            sb.Append("https://btc-e.com/api/3/");
            sb.Append(method);
            sb.Append("/");
            sb.Append(pairliststr);
            if (args != null && args.Count > 0)
            {
                sb.Append("?");
                var arr = args.Select(x => string.Format("{0}={1}", HttpUtility.UrlEncode(x.Key), HttpUtility.UrlEncode(x.Value))).ToArray();
                sb.Append(string.Join("&", arr));
            }
            var queryStr = sb.ToString();
            return WebApi.Query(queryStr);
        }

        private static string QueryIgnoreInvalid(string method, BtcePair[] pairlist, Dictionary<string, string> args = null)
        {
            var newargs = new Dictionary<string,string>() { {"ignore_invalid", "1"} };
            if (args != null)
                newargs.Concat(args);
            return Query(method, pairlist, newargs);
        }

        private static Dictionary<BtcePair, T> ReadPairDict<T>(JObject o, Func<JContainer, T> valueReader)
        {
            return o.OfType<JProperty>().Select(x => new KeyValuePair<BtcePair, T>(BtcePairHelper.FromString(x.Name), valueReader(x.Value as JContainer))).ToDictionary(x => x.Key, x => x.Value);
        }

        private static Dictionary<BtcePair, T> MakeRequest<T>(string method, BtcePair[] pairlist, Func<JContainer, T> valueReader, Dictionary<string, string> args = null, bool ignoreInvalid = true)
        {
            string queryresult;
            if (ignoreInvalid)
                queryresult = QueryIgnoreInvalid(method, pairlist, args);
            else
                queryresult = Query(method, pairlist, args);
            var resobj = JObject.Parse(queryresult);

            if (resobj["success"] != null && resobj.Value<int>("success") == 0)
                throw new Exception(resobj.Value<string>("error"));

            var r = ReadPairDict<T>(resobj, valueReader);
            return r;
        }

        public static Dictionary<BtcePair, Depth> GetDepth(BtcePair[] pairlist, int limit = 150)
        {
            return MakeRequest<Depth>("depth", pairlist, new Func<JContainer, Depth>(x => Depth.ReadFromJObject(x as JObject)), new Dictionary<string, string>() { { "limit", limit.ToString() } }, true);
        }

        public static Dictionary<BtcePair, Ticker> GetTicker(BtcePair[] pairlist)
        {
            return MakeRequest<Ticker>("ticker", pairlist, x => Ticker.ReadFromJObject(x as JObject), null, true);
        }

        public static Dictionary<BtcePair, List<TradeInfoV3>> GetTrades(BtcePair[] pairlist, int limit = 150)
        {
            Func<JContainer, List<TradeInfoV3>> tradeInfoListReader = (x => x.OfType<JObject>().Select(TradeInfoV3.ReadFromJObject).ToList());
            return MakeRequest<List<TradeInfoV3>>("trades", pairlist, tradeInfoListReader, new Dictionary<string, string>() { { "limit", limit.ToString() } }, true);
        }


    }
}
