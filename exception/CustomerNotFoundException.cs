using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exception
{
    public class CustomerNotFoundException:Exception
    {
    
            public CustomerNotFoundException() : base("User not found.") { }

            public CustomerNotFoundException(string message) : base(message) { }
        
    

}
}
