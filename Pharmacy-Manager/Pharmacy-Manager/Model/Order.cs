using System;

namespace Pharmacy_Manager
{
	public class Order
	{
		public int? ID { get; protected set; }
		public int? PrescriptionID { get; protected set; }
		public int? MedicineID { get; protected set; }
		public DateTime Date { get; protected set; }
		public int Amount { get; protected set; }

		public Order(int? id, int? prescriptionID, int? medicineID, DateTime date, int amount)
		{
			this.ID = id;
			this.PrescriptionID = prescriptionID;
			this.MedicineID = medicineID;
			this.Date = date;
			this.Amount = amount;
		}
	}
}
