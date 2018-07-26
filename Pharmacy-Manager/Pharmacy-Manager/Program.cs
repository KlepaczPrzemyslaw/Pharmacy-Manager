﻿using System;
using System.Collections.Generic;

namespace Pharmacy_Manager
{
	class Program
	{
		static void Main(string[] args)
		{
			string command = "";
			ConsoleEx.WriteLineInYellow("------ Pharmacy Manager ------");

			while (true)
			{
				Console.Write("Podaj komendę (lub wpisz help): ");
				command = Console.ReadLine().Trim().ToLower();
				Console.Clear();

				if (command == "exit" || command == "e")
					break;

				switch (command)
				{
					case "help":
					case "h":
						HelpCommand();
						break;
					case "madd":
					case "ma":
						AddMedicineCommand();
						break;
					case "mshowall":
					case "ms":
						ShowAllMedicinesCommand();
						break;
					case "mshowselected":
					case "me":
						ShowChosenMedicinesCommand();
						break;
					case "mupdate":
					case "mu":
						UpdateMedicineCommand();
						break;
					case "mupdateamount":
					case "mm":
						UpdateMedicineAmountCommand();
						break;
					case "mdelete":
					case "md":
						DeleteMedicineCommand();
						break;
					case "order":
					case "o":
						AddToOrder();
						break;
					default:
						ConsoleEx.WriteLineInRed("Niepoprawna komenda !!");
						break;
				}

				ConsoleEx.WriteLineInYellow(" Nowa Komenda ".PadLeft(22, '-').PadRight(30, '-'));
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// "DRY" //
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// 	Pobiera lek od użytkownika
		/// 	Wykorzystuje: AddMedicineCommand() / UpdateMedicineCommand()
		/// </summary>
		/// <returns>
		/// 	Medicine / null
		/// </returns>

		private static Medicine GetMedicineFromUser()
		{
			try
			{
				Console.Write("Podaj nazwę leku: ");
				string name = Console.ReadLine().Trim();

				Console.Write("Podaj producenta leku: ");
				string manufacturer = Console.ReadLine().Trim();

				Console.Write("Podaj cenę leku: ");
				double price = double.Parse(Console.ReadLine().Trim().Replace('.', ','));

				Console.Write("Podaj dostępną ilość leku: ");
				int amount = int.Parse(Console.ReadLine().Trim());

				bool withPrescription;
				Console.Write("Czy to lek na receptę (Y/N): ");
				if (Console.ReadLine().Trim().ToUpper() == "Y")
					withPrescription = true;
				else
					withPrescription = false;

				return new Medicine(null, name, manufacturer, price, amount, withPrescription);
			}
			catch (FormatException)
			{
				ConsoleEx.WriteLineInRed($"Nastąpił błąd: Podałeś daną o nieprawidłowym formacie!!");
				return null;
			}
			catch (Exception e)
			{
				ConsoleEx.WriteLineInRed($"Nastąpił wyjątek w pobieraniu leku od użytkownika: {e.GetType().ToString()}: {e.Message}!!");
				return null;
			}
		}

		/// <summary>
		/// 	Pokazuje listę leków 
		/// 	Wykorzystuje: ShowChosenMedicinesCommand() / ShowAllMedicinesCommand()
		/// </summary>
		/// <param name="byNamePart"></param>

		private static void ShowMedicines(string byNamePart = "")
		{
			List<Medicine> medicines = MedicineDAO.SelectMedicines(byNamePart);

			if (medicines == null)
				return;

			foreach (Medicine medicine in medicines)
			{
				medicine.ShowMedicine();
			}

			ConsoleEx.WriteLineInGreen("Sukces!");
		}

		/// <summary>
		/// 	Zwraca listę leków z indeksami
		/// 	Wykorzystuje: GetMedicineByIndex()
		/// </summary>
		/// <param name="byNamePart"></param>

		private static List<Medicine> GetMedicinesForIndex(string byNamePart = "")
		{
			// Pobranie listy 
			List<Medicine> medicines = MedicineDAO.SelectMedicines(byNamePart);

			if (medicines == null)
				return null;

			return medicines;
		}

		/// <summary>
		/// 	Zwraca wybrany lek po indeksie bez upewniania się, czy rekord modyfikować 
		/// 	Wykorzystuje: GetMedicineForEditWithConfirmation() / AddToOrder()
		/// </summary>
		/// <returns>
		/// 	Medicine / null
		/// </returns>

		private static Medicine GetMedicineByIndex()
		{
			// Dodatkowa przeszukiwarka
			Console.Write("Jakiego leku szukasz (możesz zawęzić poszukiwania / wciśnij enter, aby pokazać wszystko): ");
			string text = Console.ReadLine().Trim();
			Console.WriteLine();
			List<Medicine> medList = GetMedicinesForIndex(text);

			if (medList == null)
				return null;

			// Wypisanie lekarstw z indeksami
			for (int i = 0; i < medList.Count; i++)
			{
				ConsoleEx.WriteInGreen($"Index [{i}]:");
				medList[i].ShowMedicine();
			}

			// Pobranie indeksu
			int index;
			Console.Write("Wybierz index: ");
			try
			{
				index = int.Parse(Console.ReadLine().Trim());
			}
			catch
			{
				ConsoleEx.WriteLineInRed("Niepoprawny index!");
				return null;
			}

			// Pokazanie leku
			ConsoleEx.WriteLineInRed("\nWybrany lek: ");

			try
			{
				medList[index].ShowMedicine();
			}
			catch(Exception e)
			{
				ConsoleEx.WriteLineInRed($"Niepoprawny index! Treść wyjątku: {e.GetType()}: {e.Message}");
				return null;
			}

			return medList[index];
		}

		/// <summary>
		/// 	Zwraca wybrany lek po indeksie, wraz z upewnieniem się czy chcesz zmiany dokonać 
		/// 	Wykorzystuje: UpdateMedicineCommand() / UpdateMedicineAmountCommand() / DeleteMedicineCommand()
		/// </summary>
		/// <returns>
		/// 	Medicine / null
		/// </returns>

		private static Medicine GetMedicineForEditWithConfirmation()
		{
			// Pobranie leku po indeksie
			Medicine medicine = GetMedicineByIndex();

			if (medicine == null)
				return null;

			// Upewnienie się 
			ConsoleEx.ImportantQuestion("Na pewno? (Y/N): ");
			if (Console.ReadLine().Trim().ToUpper() != "Y")
			{
				ConsoleEx.WriteLineInGreen("\nAkcji nie wykonano!");
				return null;
			}

			return medicine;
		}

		/// <summary>
		/// 	Pobiera receptę od użytkownika
		/// 	Wykorzystuje: AddToOrder()
		/// </summary>
		/// <returns>
		/// 	Prescription / null
		/// </returns>

		private static Prescription GetPrescriptionFromUser()
		{
			try
			{
				Console.Write("\nPodaj imię i nazwisko klienta: ");
				string customerName = Console.ReadLine().Trim();

				Console.Write("Podaj PESEL klienta (11 znaków): ");
				long pesel = long.Parse(Console.ReadLine().Trim());

				// Sprawdzenie peselu
				if (pesel < 9999999999 || pesel > 100000000000)
					throw new FormatException();

				Console.Write("Podaj numer recepty: ");
				long prescriptionNumber = long.Parse(Console.ReadLine().Trim());

				return new Prescription(null, customerName, pesel, prescriptionNumber);
			}
			catch (FormatException)
			{
				ConsoleEx.WriteLineInRed($"Nastąpił błąd: Podałeś daną o nieprawidłowym formacie!!");
				return null;
			}
			catch (Exception e)
			{
				ConsoleEx.WriteLineInRed($"Nastąpił wyjątek w pobieraniu leku od użytkownika: {e.GetType().ToString()}: {e.Message}!!");
				return null;
			}
		}

		/// <summary>
		/// 	Pobiera receptę po indeksie
		/// 	Wykorzystuje: AddToOrder()
		/// </summary>
		/// <returns>
		/// 	Prescription / null
		/// </returns>

		private static Prescription GetPrescriptionWithConfirmation()
		{
			// Dodatkowa przeszukiwarka
			Console.Write("Wpisz fragment numeru recepty (możesz zawęzić poszukiwania / wciśnij enter, aby pokazać wszystko): ");
			string text = Console.ReadLine().Trim();
			Console.WriteLine();

			// Pobranie listy 
			List<Prescription> prescriptions = PrescriptionDAO.SelectPrescriptions(text);

			if (prescriptions == null)
				return null;

			// Wypisanie recept z indeksami
			for (int i = 0; i < prescriptions.Count; i++)
			{
				ConsoleEx.WriteInGreen($"Index [{i}]:");
				prescriptions[i].ShowPrescription();
			}

			// Pobranie indeksu
			int index;
			Console.Write("Wybierz index: ");
			try
			{
				index = int.Parse(Console.ReadLine().Trim());
			}
			catch
			{
				ConsoleEx.WriteLineInRed("Niepoprawny index!");
				return null;
			}

			// Pokazanie leku
			ConsoleEx.WriteLineInRed("\nWybrana recepta: ");

			try
			{
				prescriptions[index].ShowPrescription();
			}
			catch (Exception e)
			{
				ConsoleEx.WriteLineInRed($"Niepoprawny index! Treść wyjątku: {e.GetType()}: {e.Message}");
				return null;
			}

			// Upewnienie się 
			ConsoleEx.ImportantQuestion("Na pewno? (Y/N): ");
			if (Console.ReadLine().Trim().ToUpper() != "Y")
			{
				ConsoleEx.WriteLineInGreen("\nAkcji nie wykonano!");
				return null;
			}

			return prescriptions[index];

		}

		/// <summary>
		/// 	Finalizuje zamówienie - dla recepty
		/// 	Wykorzystuje: AddToOrder()
		/// </summary>

		public static void OrderFinalization(OrderSqlTransaction orderSqlTransaction, Prescription prescription, Medicine medicine, int quantity)
		{
			try
			{
				// Dla nowej recepty bez ID
				if (prescription != null && prescription.ID == null)
					orderSqlTransaction.InsertPrescription(prescription);

				// Update - odjęcie sprzedanej ilości
				orderSqlTransaction.UpdateMedicineAmount(medicine, (medicine.Amount - quantity));

				// Dla leku bez recepty
				if (prescription == null)
				{
					orderSqlTransaction.InsertOrderWithoutPrescription(new Order(null, null, medicine.ID, DateTime.Now, quantity));
				}
				// Dla leku z receptą
				else
				{
					orderSqlTransaction.InsertOrderWithPrescription(new Order(null, prescription.ID, medicine.ID, DateTime.Now, quantity));
				}

				// Upewnienie się
				Console.WriteLine($"\nSuma zamówienia: {medicine.Price * quantity}zł. Czy klient zapłacił? (Y/N): ");
				if (Console.ReadLine().Trim().ToUpper() != "Y")
				{
					ConsoleEx.WriteLineInRed("Anulowano akcję!");
					orderSqlTransaction.TransactionRollback();
					return;
				}

				orderSqlTransaction.TransactionCommit();
				ConsoleEx.WriteLineInGreen("Sukces!");
			}
			catch (Exception e)
			{
				ConsoleEx.WriteLineInRed($"Wyjątek przy finalizacji: {e.GetType()}: {e.Message}");
				orderSqlTransaction.TransactionRollback();
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// "Funkcje" //
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// 	Help - pomoc dla użytkownika
		/// </summary>

		private static void HelpCommand()
		{
			ConsoleEx.WriteLineInCyan(" Help ".PadLeft(18, '-').PadRight(30, '-'));

			Console.WriteLine("Oto Lista komend:");
			Console.WriteLine("Komenda: help          -> skrót: h  -> Wyświetla spis komend wraz z wyjaśnieniami.");
			Console.WriteLine("Komenda: exit          -> skrót: e  -> Wychodzi z programu.\n");

			Console.WriteLine("Komenda: mshowall      -> skrót: ms  -> Pokazuje listę wszystkich dostępnych leków.");
			Console.WriteLine("Komenda: mshowselected -> skrót: me  -> Pokazuje listę leków, które można przeszukać po nazwie.");
			Console.WriteLine("Komenda: madd          -> skrót: ma  -> Dodaje lek.");
			Console.WriteLine("Komenda: mupdate       -> skrót: mu  -> Pokazuje listę leków i pozwala jeden z nich zmienić.");
			Console.WriteLine("Komenda: mupdateamount -> skrót: mr  -> Pokazuje listę leków i pozwala w jednym z nich zmienić ilość.");
			Console.WriteLine("Komenda: mdelete       -> skrót: md  -> Pokazuje listę leków i pozwala jeden z nich usunąć.\n");

			Console.WriteLine("Komenda: order         -> skrót: o   -> Przygotowuje zamówienie.\n");

			ConsoleEx.WriteLineInGreen("Sukces!");
		}

		/// <summary>
		/// 	Dodawanie leku do bazy
		/// </summary>

		private static void AddMedicineCommand()
		{
			ConsoleEx.WriteLineInCyan(" Dodawanie Leku ".PadLeft(23, '-').PadRight(30, '-'));

			Medicine medicine = GetMedicineFromUser();

			if (medicine == null)
				return;

			// Insert
			MedicineDAO.InsertMedicineByTransaction(medicine);
		}

		/// <summary>
		/// 	Pokazuje dostępne lekarstwa z filtrem po nazwie
		/// </summary>

		private static void ShowChosenMedicinesCommand()
		{
			ConsoleEx.WriteLineInCyan(" Lista Lekarstw ".PadLeft(23, '-').PadRight(30, '-'));

			Console.Write("Jakiego leku szukasz: ");
			string text = Console.ReadLine().Trim();
			// Format wyświetlania
			Console.WriteLine();

			// Odpala metodę szukającą z podanym parametrem
			ShowMedicines(text);
		}

		/// <summary>
		/// 	Pokazuje wszyskie dostępne lekarstwa
		/// </summary>

		public static void ShowAllMedicinesCommand()
		{
			ConsoleEx.WriteLineInCyan(" Lista Lekarstw ".PadLeft(23, '-').PadRight(30, '-'));

			// Odpala metodę szukającą bez parametru
			ShowMedicines();
		}

		/// <summary>
		/// 	Zmiana całego leku z bazy
		/// </summary>

		private static void UpdateMedicineCommand()
		{
			ConsoleEx.WriteLineInCyan(" Zmiana Lekarstwa ".PadLeft(24, '-').PadRight(30, '-'));

			// Pobranie leku do zmiany
			Medicine oldMedicine = GetMedicineForEditWithConfirmation();
			// Format wyświetlania
			Console.WriteLine();

			if (oldMedicine == null)
			{
				ConsoleEx.WriteLineInRed("Anulowano akcję!");
				return;
			}

			// Pobranie nowego leku
			Medicine newMedicine = GetMedicineFromUser();

			if (newMedicine == null)
				return;

			// Update - Transakcja
			MedicineDAO.UpdateMedicineByTransaction(newMedicine, oldMedicine);
		}

		/// <summary>
		/// 	Zmiana ilości leku
		/// </summary>

		private static void UpdateMedicineAmountCommand()
		{
			ConsoleEx.WriteLineInCyan(" Zmiana Ilości Leku ".PadLeft(25, '-').PadRight(30, '-'));

			// Pobranie leku
			Medicine oldMedicine = GetMedicineForEditWithConfirmation();
			Console.WriteLine();

			if (oldMedicine == null)
			{
				ConsoleEx.WriteLineInRed("Anulowano akcję!");
				return;
			}

			// Pobranie ilości
			int newAmount;
			Console.Write("Podaj nową ilość: ");
			try
			{
				newAmount = int.Parse(Console.ReadLine().Trim());
			}
			catch
			{
				ConsoleEx.WriteLineInRed("Niepoprawny index!");
				return;
			}

			// Update Amount - Transakcja
			MedicineDAO.UpdateMedicineAmountByTransaction(oldMedicine, newAmount);
		}

		/// <summary>
		/// 	Usuwanie leku z bazy
		/// </summary>

		private static void DeleteMedicineCommand()
		{
			ConsoleEx.WriteLineInCyan(" Usuwanie Lekarstwa ".PadLeft(25, '-').PadRight(30, '-'));

			// Pobranie leku
			Medicine medicine = GetMedicineForEditWithConfirmation();

			if (medicine == null)
			{
				ConsoleEx.WriteLineInRed("Anulowano akcję!");
				return;
			}

			// Delete - Transakcja
			MedicineDAO.DeleteMedicineByTransaction(medicine);
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// "Zamówienie" //
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// 	Dodaje lek do zamówienia
		/// </summary>

		private static void AddToOrder()
		{
			OrderSqlTransaction orderSqlTransaction = new OrderSqlTransaction();

			ConsoleEx.WriteLineInCyan(" Zamówienie ".PadLeft(21, '-').PadRight(30, '-'));

			// Pobranie leku
			Medicine medicine = GetMedicineByIndex();

			if (medicine == null)
			{
				ConsoleEx.WriteLineInRed("Anulowano akcję!");
				return;
			}

			// Pobranie ilości
			int quantity;
			Console.Write("Podaj ilość: ");
			try
			{
				quantity = int.Parse(Console.ReadLine().Trim());

				if (quantity > medicine.Amount)
					throw new QuantityException();
			}
			catch (QuantityException)
			{
				ConsoleEx.WriteLineInRed($"Brak wystarczającej ilości leku w bazie!!");
				return;
			}
			catch (Exception e)
			{
				ConsoleEx.WriteLineInRed($"Nastąpił wyjątek w pobieraniu ilości: {e.GetType().ToString()}: {e.Message}!!");
				return;
			}

			// Scieżka recepty
			if (medicine.WithPrescription == true)
			{
				ConsoleEx.WriteLineInRed($"\nTen lek jest na receptę!");

				Console.Write("Czy recepta jest już w systemie? (Y/N): ");
				// Scieżka dla recepty w systemie
				if (Console.ReadLine().Trim().ToUpper() == "Y")
				{
					// Pod wyświetlanie 
					Console.WriteLine();

					// Pobranie recepty
					Prescription prescription = GetPrescriptionWithConfirmation();

					if (prescription == null)
						return;

					// Finalizacja zamówienia z receptą
					OrderFinalization(orderSqlTransaction, prescription, medicine, quantity);
				}
				// Scieżka dla nowej recepty
				else
				{
					// musimy takową pobrać
					Console.Write("\nCzy klient posiada receptę? (Y/N): ");
					if (Console.ReadLine().Trim().ToUpper() != "Y")
					{
						ConsoleEx.WriteLineInRed("Anulowano akcję!");
						return;
					}

					Prescription prescription = GetPrescriptionFromUser();
					if (prescription == null)
						return;

					// Finalizacja zamówienia z receptą
					OrderFinalization(orderSqlTransaction, prescription, medicine, quantity);
				}
			}
			// Bez recepty
			else
			{
				// Finalizacja zamówienia z receptą
				OrderFinalization(orderSqlTransaction, null, medicine, quantity);
			}
		}
	}
}
