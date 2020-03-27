using System;

namespace Domain.Helper
{
    public class Exportable : Attribute
    {
        public override string ToString()
        {
            return "Exportable";
        }
    }
}
