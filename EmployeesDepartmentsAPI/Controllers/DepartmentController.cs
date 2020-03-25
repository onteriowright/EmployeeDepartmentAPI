using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeesDepartmentsAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace EmployeesDepartmentsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IConfiguration _config;

        public DepartmentController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }


        // Get all departments from the database
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, DeptName
                        FROM Department"
                    ;

                    SqlDataReader reader = cmd.ExecuteReader();
                    var department = new List<Department>();

                    while (reader.Read())
                    {
                        var departments = new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            DeptName = reader.GetString(reader.GetOrdinal("DeptName"))
                        };

                        department.Add(departments);
                    }
                    reader.Close();

                    return Ok(department);
                }
            }
        }


        // Get a single employee from Id
        [HttpGet("{id}", Name = "GetDepartment")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT d.Id, d.DeptName
                        FROM Department d
                        WHERE d.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Department department = null;

                    if (reader.Read())
                    {
                        department = new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            DeptName = reader.GetString(reader.GetOrdinal("DeptName"))
                        };
                        reader.Close();

                        return Ok(department);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
        }

        private bool DepartmentExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, DeptName
                        FROM Department
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}