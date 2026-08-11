using System;
using System.Collections.Generic;
using System.Text;

namespace testing_web
{
    public class Product
    {
        public int Id { get; set; }=0;
        public string Name { get; set; }=string.Empty;
        public DateTime Date { get; set; }
        public string Desc { get; set; }=string.Empty;
        public int Price{get;set;}=0;
        public bool IsActive { get; set; }=false;
    }
}
