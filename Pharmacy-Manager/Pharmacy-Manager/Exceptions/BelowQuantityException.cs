using System;

namespace Pharmacy_Manager
{
	/// <summary>
	/// 	Aby wywołać zdarzenie, gdy ktoś kupi ten sam lek 2 razy - i za 2 razem zabraknie go w bazie
	/// </summary>

	public class BelowQuantityException : Exception
	{ }
}
