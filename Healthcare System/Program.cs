using System;
using System.Collections.Generic;
using System.Linq;

public class Repository<T>
{
    private List<T> items = new List<T>();

    public void Add(T item)
    {
        items.Add(item);
    }

    public List<T> GetAll()
    {
        return new List<T>(items);
    }

    public T GetById(Func<T, bool> predicate)
    {
        return items.FirstOrDefault(predicate);
    }

    public bool Remove(Func<T, bool> predicate)
    {
        var item = items.FirstOrDefault(predicate);
        if (item != null)
        {
            items.Remove(item);
            return true;
        }
        return false;
    }
}

public class Patient
{
    public int Id { get; }
    public string Name { get; }
    public int Age { get; }
    public string Gender { get; }

    public Patient(int id, string name, int age, string gender)
    {
        Id = id;
        Name = name;
        Age = age;
        Gender = gender;
    }
}

public class Prescription
{
    public int Id { get; } 
    public int PatientId { get; }
    public string Medication { get; }
    public DateTime DateIssued { get; }

    public Prescription(int id, int patientId, string medication, DateTime dateIssued)
    {
        Id = id;
        PatientId = patientId;
        Medication = medication;
        DateIssued = dateIssued;
    }
}

public class HealthSystemApp
{
    private Repository<Patient> _patientRepo = new Repository<Patient>();
    private Repository<Prescription> _prescriptionRepo = new Repository<Prescription>();
    private Dictionary<int, List<Prescription>> _prescriptionMap = new Dictionary<int, List<Prescription>>();

    public void SeedData()
    {
        // Patients
        _patientRepo.Add(new Patient(0001, "Bruno Bingo", 8, "Male"));
        _patientRepo.Add(new Patient(0002, "John James", 29, "Male"));
        _patientRepo.Add(new Patient(0003, "Ama Adu", 30, "Female"));

        // Prescriptions
        _prescriptionRepo.Add(new Prescription(01, 0001, "Paracetamol", DateTime.Now));
        _prescriptionRepo.Add(new Prescription(02, 0003, "Ibuprofen", DateTime.Now.AddDays(-4)));
        _prescriptionRepo.Add(new Prescription(03, 0003, "Amoxicillin", DateTime.Now.AddDays(-2)));
        _prescriptionRepo.Add(new Prescription(04, 0002, "Ginseng", DateTime.Now.AddDays(-1)));
        _prescriptionRepo.Add(new Prescription(05, 0001, "Vitamin C", DateTime.Now.AddDays(-3)));
    }

    public void BuildPrescriptionMap()
    {
        _prescriptionMap.Clear();
        foreach (var prescription in _prescriptionRepo.GetAll())
        {
            if (!_prescriptionMap.ContainsKey(prescription.PatientId))
            {
                _prescriptionMap[prescription.PatientId] = new List<Prescription>();
            }
            _prescriptionMap[prescription.PatientId].Add(prescription);
        }
    }

    public void PrintAllPatients()
    {
        foreach (var patient in _patientRepo.GetAll())
        {
            Console.WriteLine($"Patient ID: {patient.Id}, Name: {patient.Name}, Age: {patient.Age}, Gender: {patient.Gender}");
        }
    }

    public List<Prescription> GetPrescriptionsByPatientId(int patientId)
    {
        return _prescriptionMap.ContainsKey(patientId) ? _prescriptionMap[patientId] : new List<Prescription>();
    }

    public void PrintPrescriptionsForPatient(int patientId)
    {
        var prescriptions = GetPrescriptionsByPatientId(patientId);
        if (prescriptions.Count == 0)
        {
            Console.WriteLine($"No prescriptions found for this patient (ID: {patientId})");
        }
        else
        {
            foreach (var p in prescriptions.OrderBy(p => p.DateIssued))
            {
                Console.WriteLine($"Prescription ID: {p.Id}, Medication: {p.Medication}, Date Issued: {p.DateIssued}");
            }
        }
    }
}

public class Program
{
    public static void Main(String[] args)
    {
        var app = new HealthSystemApp();

        //Populate data
        app.SeedData();
        app.BuildPrescriptionMap();

        // Print all patients
        Console.WriteLine("All Patients:");
        app.PrintAllPatients();

        while (true)
        {
            // Ask user for patient ID
            Console.WriteLine("Enter Patient ID to view prescriptions:");
            if (int.TryParse(Console.ReadLine(), out int patientId))
            {
                app.PrintPrescriptionsForPatient(patientId);
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid patient");
                continue;
            }

            Console.WriteLine("Press 1 to check another patient or press Enter to exit...");
            string choice = Console.ReadLine();

            if (choice != "1")
            {
                Console.WriteLine("Exiting the application...");
                break;
            }
        }
    }
}
