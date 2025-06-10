using System;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
internal class Program
{
    private static int CalculateYearsofService(DateOnly start_date, DateOnly end_date)
    {
        int years = end_date.Year - start_date.Year;
        if (end_date < start_date.AddYears(years))
            years--;
        return years;
    }
    private static int CalculateAnnualLeave(int yearsOfService)
    {
        int baseDays  = yearsOfService * 12;   // 12 days times number of completed years
        int extraDays = yearsOfService / 5;    // +1 day for each 5 years 
        int total     = baseDays + extraDays;
        return Math.Min(total, 36);
    }

    private static void CallUpdateAnnualLeaveProcedure(int employee_id, SqlConnection conn)
{
    try
    {
            // Create the SqlCommand to call the stored procedure
            using SqlCommand command = new SqlCommand("dbo.UpdateAnnualLeave", conn);
            // Set the command type to stored procedure
            command.CommandType = System.Data.CommandType.StoredProcedure;

            // Add the input parameter for the employee_id
            command.Parameters.Add(new SqlParameter("@employee_id", System.Data.SqlDbType.Int) { Value = employee_id });

            // Execute the stored procedure
            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                Console.WriteLine("Annual Leave updated successfully.");
            }
            else
            {
                Console.WriteLine("No record updated. Check the employee ID.");
            }
        }
    catch (Exception ex)
    {
        Console.WriteLine($"Error calling stored procedure: {ex.Message}");
    }
}


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
            Console.WriteLine("\nSelect\n1. Create\n2. Update Employee info\n3. Delete\n4. Update Years of Service");
            int choice = int.Parse(Console.ReadLine()!);

            switch (choice)
            {
                case 1: // CREATE
                    Console.Write("Enter Employee ID (int): ");
                    int employee_id_create = int.Parse(Console.ReadLine()!);

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
                        cmd.Parameters.AddWithValue("@id", employee_id_create);
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
                            Console.WriteLine($"ID: {reader["employee_id"]}, Code: {reader["employee_code"]}, Name: {reader["employee_name"]}, DOB: {reader["employee_dob"]}, Remaining Annual Leave: {reader["remaining_annual_leave"]}");
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
                    Console.Write("Enter the Employee ID to be removed: ");
                    e_id = int.Parse(Console.ReadLine());
                    string deleteQuery = "DELETE FROM employee_info WHERE employee_id = " + e_id + "";
                    SqlCommand deleteCommand = new SqlCommand(deleteQuery, conn);
                    deleteCommand.ExecuteNonQuery();
                    Console.WriteLine("Successfully deleted");
                    break;

                case 4: // CALL STORED PROCEDURE TO UPDATE ANNUAL LEAVE
                    Console.Write("Enter Employee ID (int): ");
                    string? employeeIdInput = Console.ReadLine();
                    if (int.TryParse(employeeIdInput, out int employee_id_update))
                    {
                        Console.Write("Enter Year of Service (int): ");
                        string? yearInput = Console.ReadLine();
                        if (int.TryParse(yearInput, out int year))
                        {
                            CallUpdateAnnualLeaveProcedure(employee_id_update, conn);
                        }
                        else
                        {
                            Console.WriteLine("Invalid year input. Please enter a valid integer.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid Employee ID input. Please enter a valid integer.");
                    }
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


    private static int CalculateYearsOfService(DateOnly start_date, DateOnly? end_date)
    {
        throw new NotImplementedException();
    }
}
