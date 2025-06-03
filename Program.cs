using System;
using Microsoft.Data.SqlClient;

internal class Program
{
    private static void Main(string[] args)
    {
        try
        {
            using SqlConnection conn = new SqlConnection("Server=172.16.7.236,1433;Database=DevTraining;User Id=Training;Password=Tran8889!@#Gtsc205;Encrypt=False;");
            conn.Open();
            Console.WriteLine("Connection established!");

            string answer;
            do
            {
                Console.WriteLine("\nSelect\n1. Create\n2. Update\n3. Delete");
                int choice = int.Parse(Console.ReadLine()!);

                switch (choice)
                {
                    case 1: // CREATE
                        Console.Write("Enter Employee ID (int): ");
                        int employee_id = int.Parse(Console.ReadLine()!);

                        Console.Write("Enter Employee Code (int): ");
                        int employee_code = int.Parse(Console.ReadLine()!);

                        Console.Write("Enter Employee Name: ");
                        string? employee_name = Console.ReadLine();

                        Console.Write("Enter Employee DOB (yyyy-mm-dd): ");
                        DateOnly employee_dob = DateOnly.Parse(Console.ReadLine()!);

                        string insertQuery = @"
                            INSERT INTO employee_info (employee_id, employee_code, employee_name, employee_dob)
                            VALUES (@id, @code, @name, @dob);";

                        using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", employee_id);
                            cmd.Parameters.AddWithValue("@code", employee_code);
                            cmd.Parameters.AddWithValue("@name", employee_name ?? "");
                            cmd.Parameters.AddWithValue("@dob", employee_dob);

                            int rows_affected = cmd.ExecuteNonQuery();
                            Console.WriteLine($"Inserted {rows_affected} row(s) successfully.");
                        }
                        break;

                    case 2: // UPDATE
                        Console.WriteLine("\nCurrent employee records:");
                        string selectQuery = "SELECT * FROM employee_info";
                        using (SqlCommand selectCmd = new SqlCommand(selectQuery, conn))
                        using (SqlDataReader reader = selectCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine($"ID: {reader["employee_id"]}, Code: {reader["employee_code"]}, Name: {reader["employee_name"]}, DOB: {reader["employee_dob"]}");
                            }
                        }

                        Console.Write("\nEnter the Employee ID you want to update: ");
                        int updateId = int.Parse(Console.ReadLine()!);

                        Console.Write("Enter new Employee Code (int): ");
                        int updateCode = int.Parse(Console.ReadLine()!);

                        Console.Write("Enter new Employee Name: ");
                        string? updateName = Console.ReadLine();

                        Console.Write("Enter new DOB (yyyy-mm-dd): ");
                        DateOnly updateDOB = DateOnly.Parse(Console.ReadLine()!);

                        string updateQuery = @"
                            UPDATE employee_info
                            SET employee_code = @code,
                                employee_name = @name,
                                employee_dob = @dob
                            WHERE employee_id = @id;";

                        using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                        {
                            updateCmd.Parameters.AddWithValue("@code", updateCode);
                            updateCmd.Parameters.AddWithValue("@name", updateName ?? "");
                            updateCmd.Parameters.AddWithValue("@dob", updateDOB);
                            updateCmd.Parameters.AddWithValue("@id", updateId);
                            int updated = updateCmd.ExecuteNonQuery();
                            Console.WriteLine($"Updated {updated} row(s).");
                        }
                        break;

                    case 3: // DELETE
                        int e_id;
                            Console.Write("Enter the Employee ID of the entry to be removed: ");
                            e_id = int.Parse(Console.ReadLine());
                            String deleteQuery = "DELETE FROM employee_info WHERE employee_id = " + e_id + "";
                            SqlCommand deleteCommand = new SqlCommand(deleteQuery, conn);
                            deleteCommand.ExecuteNonQuery();
                            Console.WriteLine("Successfully deleted");
                            break;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                Console.Write("\nDo you want to continue? (y/n): ");
                answer = Console.ReadLine()!.ToLower();

            } while (answer == "y");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception occurred: {e.Message} ({e.GetType()})");
        }
    }
}
