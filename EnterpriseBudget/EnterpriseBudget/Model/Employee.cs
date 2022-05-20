using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace EnterpriseBudget.Model
{
    /// <summary>
    /// The different type of jobs that are available
    /// </summary>
    public enum JobTypes
    {
        /// <summary>
        /// No job specified
        /// </summary>
        Nothing = 0,

        /// <summary>
        /// Teacher (faculty)
        /// </summary>
        Faculty = 1,

        /// <summary>
        /// chair person
        /// </summary>
        Chair = 2,

        /// <summary>
        /// job with absolute power
        /// </summary>
        Admin = 3
    } 
    
    /// <summary>
    /// Employee class
    /// </summary>
    public class Employee
    {
        JobTypes _jobType;
        String _jobName;
        int _deptId;
        String _departmentName;
        String _userName;
        
        /// <summary>
        /// what job type does the employee have?
        /// </summary>
        public JobTypes jobType { get { return _jobType; } }

        /// <summary>
        /// what is the department name of the employee
        /// </summary>
        public String departmentName { get { return _departmentName; } }

        /// <summary>
        /// what is the name of the employee's job?
        /// </summary>
        public String jobName { get { return _jobName; } }

        /// <summary>
        /// employee name
        /// </summary>
        public String userName { get { return _userName; } }

        /// <summary>
        /// employee department id
        /// </summary>
        public int deptartmentID { get { return _deptId; } }

        /// <summary>
        /// Verify that there is an employee with username/password
        /// </summary>
        /// <param name="username">username</param>
        /// <param name="password">password</param>
        /// <returns>an Employee object if such a user exists, null otherwise</returns>
        public static Employee validateUser(String username, String password)
        {
            Employee person = null;
            try
            {
                int deptId = 0, jobId = 0;

                //Employees table
                SqlCommand verifyUser =  Connection.cnn.CreateCommand();

                verifyUser.CommandText = "SELECT id, name, password, departmentId, jobId FROM employees WHERE name = @name and password = @password";
                verifyUser.Parameters.AddWithValue("@name", username);
                verifyUser.Parameters.AddWithValue("@password", password);

                var rdr = verifyUser.ExecuteReader();

                if(rdr.HasRows)
                {

                    int id = 0;
                    String name = "", pass = "";
                    while (rdr.Read())
                    {
                        id = rdr.GetInt32(0);
                        name = rdr.GetString(1);
                        pass = rdr.GetString(2);
                        deptId = rdr.GetInt32(3);
                        jobId = rdr.GetInt32(4);
                    }

                    person = new Employee();

                    person._deptId = deptId;
                    person._userName = name;
                    person._jobType = (JobTypes)jobId;
                }

                verifyUser.Dispose();

                rdr.Close();

                //Department table
                SqlCommand verifyDept = Connection.cnn.CreateCommand();

                verifyDept.CommandText = "SELECT name FROM departments WHERE id = @id";
                verifyDept.Parameters.AddWithValue("@id", deptId);

                var rdr2 = verifyDept.ExecuteReader();

                string nameDept = "";

                if (rdr2.HasRows)
                {
                    while (rdr2.Read())
                    {
                        nameDept = rdr2.GetString(0);
                    }

                    person._departmentName = nameDept;
                }

                verifyDept.Dispose();

                rdr2.Close();

                //Job table
                SqlCommand verifyJob = Connection.cnn.CreateCommand();

                verifyJob.CommandText = "SELECT name FROM jobTitles WHERE id = @id";
                verifyJob.Parameters.AddWithValue("@id", jobId);

                var rdr3 = verifyJob.ExecuteReader();

                string nameJob = "";

                if (rdr2.HasRows)
                {
                    while (rdr3.Read())
                    {
                        nameJob = rdr3.GetString(0);
                    }

                    person._jobName = nameJob;
                }

                verifyJob.Dispose();

                rdr3.Close();

                //sqlcredential
            }
            catch (Exception e) {
                var txt = e.Message;
            }
            //if (rdr != null)
           // {
            //    try { rdr.Close(); } catch { }
           // }
            return person;
        }

        private Employee()
        {

        }

    }
}
