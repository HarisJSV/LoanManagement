﻿using entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public interface ILoanEfRepository
    {
        List<Loan> GetLoansByCustomerId(int customerId);
    }

}
