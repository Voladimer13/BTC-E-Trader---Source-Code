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

using System;

namespace BtcE
{
	public enum BtcePair
	{
btc_usd,
btc_rur,
btc_eur,
ltc_btc,
ltc_usd,
ltc_rur,
ltc_eur,
nmc_btc,
nmc_usd,
nvc_btc,
nvc_usd,
usd_rur,
eur_usd,
trc_btc,
ppc_btc,
ppc_usd,
ftc_btc,
xpm_btc,
Unknown
	}

	public class BtcePairHelper
	{
		public static BtcePair FromString(string s) {
			BtcePair ret = BtcePair.Unknown;
			Enum.TryParse<BtcePair>(s.ToLowerInvariant(), out ret);
			return ret;
		}

		public static string ToString(BtcePair v) {
			return Enum.GetName(typeof(BtcePair), v).ToLowerInvariant();
		}

        public static string ToBetterString(BtcePair v)
        {
            return Enum.GetName(typeof(BtcePair), v).ToLowerInvariant().ToUpper().Replace('_','/');
        }

        public static string ToBetterString(string v)
        {
            return v.ToLowerInvariant().ToUpper().Replace('_', '/');
        }

        public static BtcePair FromBetterString(string s)
        {
            BtcePair ret = BtcePair.Unknown;
            Enum.TryParse<BtcePair>(s.ToLowerInvariant().Replace('/','_'), out ret);
            return ret;
        }


	}
}
