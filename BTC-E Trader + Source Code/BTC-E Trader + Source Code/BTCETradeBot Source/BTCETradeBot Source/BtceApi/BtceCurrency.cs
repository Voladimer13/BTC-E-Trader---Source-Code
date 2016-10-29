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
	public enum BtceCurrency
	{
		btc,
		ltc,
		nmc,
		nvc,
		trc,
		ppc,
		ftc,
		usd,
		rur,
		eur,
        xpm,
		Unknown
	}
	class BtceCurrencyHelper
	{
		public static BtceCurrency FromString(string s) {
			BtceCurrency ret = BtceCurrency.Unknown;
			Enum.TryParse<BtceCurrency>(s.ToLower(), out ret);
			return ret;
		}
		public static string ToString(BtceCurrency v) {
			return Enum.GetName(typeof(BtceCurrency), v);
		}
	}
}
