using System;
using System.Collections.Generic;
using System.IO;

public class Student
{
    public int Id { get; }
    public string FullName { get; }
    public int Score { get; }

    public Student(int id, string fullName, int score)
    {
        Id = id;
        FullName = fullName;
        Score = score;
    }

    public string GetGrade()
    {
        if (Score >= 80 && Score <= 100) return "A";
        else if (Score >= 70) return "B";
        else if (Score >= 60) return "C";
        else if (Score >= 50) return "D";
        else return "F";
    }
}

public class InvalidScoreFormatException : Exception
{
    public InvalidScoreFormatException(string message) : base(message) { }
}

public class MissingFieldException : Exception
{
    public MissingFieldException(string message) : base(message) { }
}

public class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string inputFilePath)
    {
        var students = new List<Student>();

        using (var reader = new StreamReader(inputFilePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Split(',');

                if (parts.Length != 3)
                    throw new MissingFieldException($"Missing field(s) in line: {line}");

                if (!int.TryParse(parts[0], out int id))
                    throw new FormatException($"Invalid ID format in line: {line}");

                string name = parts[1].Trim();

                if (!int.TryParse(parts[2], out int score))
                    throw new InvalidScoreFormatException($"Invalid score format in line: {line}");

                students.Add(new Student(id, name, score));
            }
        }

        return students;
    }

    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using (var writer = new StreamWriter(outputFilePath))
        {
            foreach (var student in students)
            {
                writer.WriteLine($"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}");
            }
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        var processor = new StudentResultProcessor();

        while (true)
        {
            Console.WriteLine("\nSchool Grading System");
            Console.WriteLine("1. Process Student File");
            Console.WriteLine("Press Enter to Exit");
            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();

            if (string.IsNullOrEmpty(choice)) break;

            if (choice == "1")
            {
                try
                {
                    Console.Write("Enter input file path: ");
                    string inputPath = Console.ReadLine();

                    Console.Write("Enter output file path: ");
                    string outputPath = Console.ReadLine();

                    var students = processor.ReadStudentsFromFile(inputPath);
                    processor.WriteReportToFile(students, outputPath);

                    Console.WriteLine($"Report successfully written to {outputPath}");
                }
                catch (FileNotFoundException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                catch (InvalidScoreFormatException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                catch (MissingFieldException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid option.");
            }
        }

        Console.WriteLine("Exiting application...");
    }
}



