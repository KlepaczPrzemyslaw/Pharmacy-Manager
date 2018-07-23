using System;

namespace Pharmacy_Manager
{
	public class Prescription
	{
		public int? ID { get; protected set; }
		public string CustomerName { get; protected set; }
		public long PESEL { get; protected set; }
		public long PrescriptionNumber { get; protected set; }

		public Prescription(int? id, string customerName, long pesel, long prescriptionNumber)
		{
			this.ID = id;
			this.CustomerName = customerName;
			this.PESEL = pesel;
			this.PrescriptionNumber = prescriptionNumber;
		}

		/// <summary>
		/// 	Pokazanie recepty na konsoli
		/// </summary>

		public void ShowPrescription()
		{
			Console.WriteLine($"Recepta: {PrescriptionNumber} -> {CustomerName} o numerze pesel: {PESEL}\n");
		}

		/// <summary>
		/// 	TYLKO DLA OrderSqlTransaction -> Aby ustawić ID obiektu po wywołaniu Inserta!!!!!
		/// </summary>

		public void SaveID(int? id)
		{
			this.ID = id;
		}
	}
}
