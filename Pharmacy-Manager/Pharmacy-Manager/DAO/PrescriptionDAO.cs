using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Pharmacy_Manager
{
	public static class PrescriptionDAO
	{
		/// <summary>
		/// 	SELECT na bazie recept
		/// </summary>
		/// <param name="byNamePart"></param>
		/// <returns>
		/// 	Zwraca listę recept
		/// </returns>

		public static List<Prescription> SelectPrescriptions(string byNumberPart = "")
		{
			long? NumberPart;
			List<Prescription> list = new List<Prescription>();

			// Parsowanie
			NumberPart = string.IsNullOrWhiteSpace(byNumberPart) ? NumberPart = null : NumberPart = long.Parse(byNumberPart);
				
			try
			{
				using (SqlConnection sqlConnection = new SqlConnection(ConnectionString.connectionString))
				{
					SqlCommand sqlCommand = new SqlCommand();
					sqlCommand.Connection = sqlConnection;

					// Szuaknie bez parametru
					if (NumberPart == null)
					{
						sqlCommand.CommandText = $"SELECT * FROM Prescriptions";
					}
					// Szuaknie z parametrem
					else
					{
						sqlCommand.CommandText = $"SELECT * FROM Prescriptions WHERE PrescriptionNumber LIKE @PrescriptionNumber;";
						SqlParameter sqlNumberParam = new SqlParameter
						{
							ParameterName = "@PrescriptionNumber",
							DbType = System.Data.DbType.String,
							Value = $"%{NumberPart}%"
						};
						sqlCommand.Parameters.Add(sqlNumberParam);
					}

					sqlConnection.Open();

					SqlDataReader sqlReader = sqlCommand.ExecuteReader();

					while (sqlReader.Read())
					{
						list.Add(new Prescription(sqlReader.GetInt32(0), sqlReader.GetString(1), sqlReader.GetInt64(2),
							sqlReader.GetInt64(3)));
					}
				}
			}
			catch (Exception e)
			{
				ConsoleEx.WriteLineWithColor($"Nastąpił wyjątek w pobieraniu recepty: {e.GetType().ToString()}: {e.Message}!!", ConsoleColor.Red);
				return null;
			}

			return list;
		}
	}
}
