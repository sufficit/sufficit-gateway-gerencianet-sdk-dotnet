using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace GerencianetSDK.Acknowledgment
{
    public static class Extensions
    {
        public static AckRow ToAckRow(this string source)
        {
            if (!string.IsNullOrWhiteSpace(source))
            {
                if (source.Length == 130)
                {
                    var row = new AckRow();

                    string index = source.Substring(0, 10);                    
                    row.Index = int.Parse(index);
                    
                    string account = source.Substring(10, 20);                    
                    row.Account = Int64.Parse(account);                    

                    string code = source.Substring(30, 2);
                    row.Code = int.Parse(code);
                    
                    string date = source.Substring(32, 6);
                    row.Date = DateTime.ParseExact(date, APIConstants.DATEFORMAT, CultureInfo.InvariantCulture);
                    
                    string charge = source.Substring(38, 20);
                    row.Charge = Int64.Parse(charge);
                    
                    string total = source.Substring(58, 13);
                    row.Total = decimal.Parse($"{ total.Substring(0, 11) }.{ total.Substring(11, 2) }", CultureInfo.InvariantCulture);
                    
                    string amount = source.Substring(71, 13);
                    row.Amount = decimal.Parse($"{ amount.Substring(0, 11) }.{ amount.Substring(11, 2) }", CultureInfo.InvariantCulture);

                    string type = source.Substring(84, 1);
                    row.Type = int.Parse(type);

                    row.Custom = source.Substring(85, 20);
                    
                    string expire = source.Substring(105, 6);
                    row.Expire = DateTime.ParseExact(expire, APIConstants.DATEFORMAT, CultureInfo.InvariantCulture);

                    string payed = source.Substring(111, 6);
                    row.Payed = DateTime.ParseExact(payed, APIConstants.DATEFORMAT, CultureInfo.InvariantCulture);

                    string tariff = source.Substring(117, 13);
                    row.Tariff = decimal.Parse($"{ tariff.Substring(0, 11) }.{ tariff.Substring(11, 2) }", CultureInfo.InvariantCulture);

                    return row;
                }
                else { throw new Exception("invalid length row"); }
            }
            return default;
        }

        public static string ToAckRowString(this AckRow source)
        {
            return
                source.Index.ToString().PadLeft(10, '0') +
                source.Account.ToString().PadLeft(20, '0') +
                source.Code.ToString().PadLeft(2, '0') +
                source.Date.ToString(APIConstants.DATEFORMAT) +
                source.Charge.ToString().PadLeft(20, '0') +
                source.Total.ToString("#.##").Replace(".", "").PadLeft(13, '0') +
                source.Amount.ToString("#.##").Replace(".", "").PadLeft(13, '0') +
                source.Type.ToString() +
                source.Custom +
                source.Expire.ToString(APIConstants.DATEFORMAT) +
                source.Payed.ToString(APIConstants.DATEFORMAT) +
                source.Tariff.ToString("#.##").Replace(".", "").PadLeft(13, '0');
        }
    }
}
