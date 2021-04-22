using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinTimesheet2021.Models
{
    public class Operation
    {
        public int EmployeeID { get; set; }
        public int? CustomerID { get; set; }
        public int WorkAssignmentID { get; set; }
        public string OperationType { get; set; }

        public string Comment { get; set; }
    }
}
