using System;

namespace Pharmacy_Manager
{
	/// <summary>
	/// 	Aby wywołać wyjątek -> gdy transakcja nie zmieni żadnego wiersza | lub zmieni tylko 1 w bazie logów!!!!
	/// </summary>

	public class ZeroException : Exception
	{ }
}
