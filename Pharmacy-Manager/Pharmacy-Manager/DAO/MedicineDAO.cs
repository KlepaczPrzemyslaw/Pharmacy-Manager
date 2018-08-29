using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Pharmacy_Manager
{
	public static class MedicineDAO
	{
		/// <summary>
		/// 	SELECT na bazie lekarstw
		/// </summary>
		/// <param name="byNamePart"></param>
		/// <returns>
		/// 	Zwraca listę leków
		/// </returns>

		public static List<Medicine> SelectMedicines(string byNamePart = "")
		{
			List<Medicine> list = new List<Medicine>();

			try
			{
				using (SqlConnection sqlConnection = new SqlConnection(ConnectionString.connectionString))
				{
					SqlCommand sqlCommand = new SqlCommand();
					sqlCommand.Connection = sqlConnection;

					// Szuaknie bez parametru
					if (string.IsNullOrWhiteSpace(byNamePart))
					{
						sqlCommand.CommandText = $"SELECT * FROM Medicines";
					}
					// Szuaknie z parametrem
					else
					{
						sqlCommand.CommandText = $"SELECT * FROM Medicines WHERE Name LIKE @Name;";
						SqlParameter sqlNameParam = new SqlParameter
						{
							ParameterName = "@Name",
							Value = $"%{byNamePart}%",
							DbType = System.Data.DbType.String
						};
						sqlCommand.Parameters.Add(sqlNameParam);
					}

					sqlConnection.Open();

					SqlDataReader sqlReader = sqlCommand.ExecuteReader();

					while (sqlReader.Read())
					{
						list.Add(new Medicine(sqlReader.GetInt32(0), sqlReader.GetString(1), sqlReader.GetString(2),
							Convert.ToDouble(sqlReader.GetDecimal(3)), sqlReader.GetInt32(4), sqlReader.GetBoolean(5)));
					}
				}
			}
			catch (Exception e)
			{
				ConsoleEx.WriteLineWithColor($"Nastąpił wyjątek w pobieraniu leku: {e.GetType().ToString()}: {e.Message}!!", ConsoleColor.Red);
				return null;
			}

			return list;
		}

		/// <summary>
		/// 	INSERT na lekarstwie
		/// </summary>
		/// <param name="medicineToInsert"></param>

		public static void InsertMedicineByTransaction(Medicine medicineToInsert)
		{
			SqlCommand sqlCommand = new SqlCommand();

			// Zapytanie 
			sqlCommand.CommandText = $"INSERT INTO Medicines VALUES (@Name, @Manufacturer, @Price, @Amount, @WithPrescription);";

			// Dodanie parametrów w prywatnej funkcji
			AddParameters(sqlCommand, medicineToInsert);

			// Transakcja
			SqlTransactionTool.Transaction(sqlCommand);
		}

		/// <summary>
		/// 	UPDATE na lekarstwie
		/// </summary>
		/// <param name="newMedicine"></param>
		/// <param name="oldMedicine"></param>

		public static void UpdateMedicineByTransaction(Medicine newMedicine, Medicine oldMedicine)
		{
			SqlCommand sqlCommand = new SqlCommand();

			// Zapytanie 
			sqlCommand.CommandText = $"UPDATE Medicines SET Name = @Name, Manufacturer = @Manufacturer, Price = @Price, Amount = @Amount, " +
				$"WithPrescription = @WithPrescription WHERE ID = @ID;";

			// Dodanie parametrów w prywatnych funkcjach
			// Dodanie wszystkich parametrów (oprócz ID z nowego leku)
			AddParameters(sqlCommand, newMedicine);
			// Dodanie parametru ID (starego leku)
			// Lek zyska nowe wartości na starym ID
			AddParameterID(sqlCommand, oldMedicine);

			// Transakcja
			SqlTransactionTool.Transaction(sqlCommand);
		}

		/// <summary>
		/// 	UPDATE na lekarstwie TYLKO Amount
		/// </summary>
		/// <param name="medicine"></param>

		public static void UpdateMedicineAmountByTransaction(Medicine oldMedicine, int newAmount)
		{
			SqlCommand sqlCommand = new SqlCommand();

			// Zapytanie 
			sqlCommand.CommandText = $"UPDATE Medicines SET Amount = @Amount WHERE ID = @ID;";

			// Dodanie parametrów w prywatnych funkcjach
			AddParameterAmount(sqlCommand, newAmount);
			// Dodanie ID pobranego leku
			AddParameterID(sqlCommand, oldMedicine);

			// Transakcja
			SqlTransactionTool.Transaction(sqlCommand);
		}

		/// <summary>
		/// 	DELETE na lekarstwie
		/// </summary>
		/// <param name="medicineToDelete"></param>

		public static void DeleteMedicineByTransaction(Medicine medicineToDelete)
		{
			SqlCommand sqlCommand = new SqlCommand();

			// Zapytanie 
			sqlCommand.CommandText = $"DELETE FROM Medicines WHERE ID = @ID;";

			// Dodanie parametrów w prywatnych funkcjach
			AddParameterID(sqlCommand, medicineToDelete);

			// Transakcja
			SqlTransactionTool.Transaction(sqlCommand);
		}

		////////////////////////////////////////////////////////////////
		// Prywatnie metody //
		////////////////////////////////////////////////////////////////

		/// <summary>
		/// 	Prywana funkcja, która dodaje do zapytania paczkę parametrów
		/// </summary>
		/// <param name="sqlCommand"></param>
		/// <param name="medicine"></param>

		private static void AddParameters(SqlCommand sqlCommand, Medicine medicine)
		{
			// Parametry 
			SqlParameter sqlNameParam = new SqlParameter
			{
				ParameterName = "@Name",
				Value = medicine.Name,
				DbType = System.Data.DbType.String
			};
			sqlCommand.Parameters.Add(sqlNameParam);

			SqlParameter sqlManufacturerParam = new SqlParameter
			{
				ParameterName = "@Manufacturer",
				Value = medicine.Manufacturer,
				DbType = System.Data.DbType.String
			};
			sqlCommand.Parameters.Add(sqlManufacturerParam);

			SqlParameter sqlPriceParam = new SqlParameter
			{
				ParameterName = "@Price",
				Value = medicine.Price,
				DbType = System.Data.DbType.Double
			};
			sqlCommand.Parameters.Add(sqlPriceParam);

			SqlParameter sqlAmountParam = new SqlParameter
			{
				ParameterName = "@Amount",
				Value = medicine.Amount,
				DbType = System.Data.DbType.Int32
			};
			sqlCommand.Parameters.Add(sqlAmountParam);

			SqlParameter sqlWithPrescriptionParam = new SqlParameter
			{
				ParameterName = "@WithPrescription",
				Value = medicine.WithPrescription,
				DbType = System.Data.DbType.Boolean
			};
			sqlCommand.Parameters.Add(sqlWithPrescriptionParam);
		}

		/// <summary>
		/// 	Prywana funkcja, która dodaje ID do zapytania
		/// </summary>
		/// <param name="sqlCommand"></param>
		/// <param name="medicine"></param>

		private static void AddParameterID(SqlCommand sqlCommand, Medicine medicine)
		{
			// Parametr 
			SqlParameter sqlIDParam = new SqlParameter
			{
				ParameterName = "@ID",
				Value = medicine.ID,
				DbType = System.Data.DbType.Int32
			};
			sqlCommand.Parameters.Add(sqlIDParam);
		}

		/// <summary>
		/// 	Prywana funkcja, która dodaje Amount do zapytania
		/// </summary>
		/// <param name="sqlCommand"></param>
		/// <param name="newAmount"></param>

		private static void AddParameterAmount(SqlCommand sqlCommand, int newAmount)
		{
			// Parametr 
			SqlParameter sqlAmountParam = new SqlParameter
			{
				ParameterName = "@Amount",
				Value = newAmount,
				DbType = System.Data.DbType.Int32
			};
			sqlCommand.Parameters.Add(sqlAmountParam);
		}
	}
}
