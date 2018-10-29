using UnHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main()
        {
            new ClassLibrary1.Configuration().Initialize();
            var db = SessionManager.Session;
            var users = db.Query<ClassLibrary1.IUser>().ToList();
        }
    }
}
