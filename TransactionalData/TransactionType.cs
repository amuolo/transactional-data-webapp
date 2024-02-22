

using System.ComponentModel.DataAnnotations;

namespace Model;

public enum TransactionType : byte
{
    Food = 0,

    Drinks = 1,

    Travel = 2, 

    Leisure = 3, 

    Rent = 4, 

    Mortgage = 5, 

    Income = 6
}
