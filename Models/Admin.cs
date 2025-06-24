using System.Collections.Generic;
using ChickenF.Models;
namespace ChickenF.Models
{
    public class Admin : User
    {
        public override string RoleName => "Admin";
    }
}

